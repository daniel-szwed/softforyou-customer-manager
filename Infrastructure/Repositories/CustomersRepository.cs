using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class CustomersRepository : ICustomerRepository
    {
        private readonly ILogger logger;
        private readonly IDbConnectionProvider dbConnectionProvider;
        private readonly ISqlExecutor sqlExecutor;

        public CustomersRepository(
            ILogger logger,
            IDbConnectionProvider dbConnectionProvider,
            ISqlExecutor sqlExecutor)
        {
            this.logger = logger;
            this.dbConnectionProvider = dbConnectionProvider;
            this.sqlExecutor = sqlExecutor;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (customer.Address == null)
                throw new ArgumentNullException(nameof(customer.Address));

            if (customer.Id == Guid.Empty)
                customer.Id = Guid.NewGuid();

            if (customer.Address != null)
            {
                if (customer.Address.Id == Guid.Empty)
                    customer.Address.Id = customer.Id;
            }

            using (IDbConnection connection = dbConnectionProvider.GetDbConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sqlCustomer = @"
                            INSERT INTO Customers (Id, Name, TaxId, PhoneNumber, EmailAddress)
                            VALUES (@Id, @Name, @TaxId, @PhoneNumber, @EmailAddress);
                        ";

                        await sqlExecutor.ExecuteAsync(
                            sqlCustomer,
                            new
                            {
                                customer.Id,
                                customer.Name,
                                customer.TaxId,
                                customer.PhoneNumber,
                                customer.EmailAddress,
                                AddressId = customer.Address.Id
                            },
                            transaction: transaction
                        );

                        if (customer.Address != null)
                        {
                            var sqlAddress = @"
                                INSERT INTO Addresses (Id, PostCode, City, Street, StreetNumber, ApartmentNumber)
                                VALUES (@Id, @PostCode, @City, @Street, @StreetNumber, @ApartmentNumber);
                            ";

                            await sqlExecutor.ExecuteAsync(
                                sqlAddress,
                                new
                                {
                                    customer.Address.Id,
                                    customer.Address.PostCode,
                                    customer.Address.City,
                                    customer.Address.Street,
                                    customer.Address.StreetNumber,
                                    customer.Address.ApartmentNumber
                                },
                                transaction: transaction
                            );
                        }

                        transaction.Commit();

                        return customer;
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        logger.Error(ex, "Cannot add customer.");

                        throw;
                    }
                }
            }
        }

        public async Task DeleteCustomerAsync(Guid customerId)
        {
            using (IDbConnection connection = dbConnectionProvider.GetDbConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = "DELETE FROM Customers WHERE Id = @Id";
                        await sqlExecutor.ExecuteAsync(sql, new { Id = customerId }, transaction);
                        transaction.Commit();
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        logger.Error(ex, $"Deleting customer with id: {customerId} failed.");

                        throw;
                    }
                }
            }
        }

        public async Task<Customer[]> GetAllCustomersAsync()
        {
            var customers = new List<Customer>();

            var sql = @"
                    SELECT
                        c.Id, c.Name, c.TaxId, c.PhoneNumber, c.EmailAddress,
                        a.Id, a.PostCode, a.City, a.Street, a.StreetNumber, a.ApartmentNumber
                    FROM Customers c
                    LEFT JOIN Addresses a ON a.Id = c.Id
                    ORDER BY c.Name;
                    ";

            using (IDbConnection connection = dbConnectionProvider.GetDbConnection())
            {
                connection.Open();

                try
                {
                    var result = await connection.QueryAsync(
                        sql,
                        (Func<Customer, Address, Customer>)((c, a) =>
                        {
                            c.Address = a;
                            customers.Add(c);
                            return c;
                        }),
                        splitOn: "Id"
                    );

                    return customers.ToArray();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Fetching cusomers from database failed.");
                    throw;
                }
            }
        }

        public async Task<bool> IsTaxIdAlreadyTakenAsync(string taxId, Guid excludedCustomerId)
        {
            const string sql = @"
                        SELECT 1
                        FROM Customers
                        WHERE TaxId = @TaxId
                          AND Id <> @ExcludedCustomerId";

            using (var connection = dbConnectionProvider.GetDbConnection())
            {
                connection.Open();

                var result = await connection.QuerySingleOrDefaultAsync<int?>(
                    sql,
                    new { TaxId = taxId, ExcludedCustomerId = excludedCustomerId });

                return result.HasValue;
            }

        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            if (customer.Id == Guid.Empty)
                throw new ArgumentException("Customer Id cannot be empty", nameof(customer));

            using (IDbConnection connection = dbConnectionProvider.GetDbConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Update Customer table
                        var sqlCustomer = @"
                            UPDATE Customers
                            SET Name = @Name,
                                TaxId = @TaxId,
                                PhoneNumber = @PhoneNumber,
                                EmailAddress = @EmailAddress
                            WHERE Id = @Id;
                        ";

                        await sqlExecutor.ExecuteAsync(
                            sqlCustomer,
                            new
                            {
                                customer.Id,
                                customer.Name,
                                customer.TaxId,
                                customer.PhoneNumber,
                                customer.EmailAddress
                            },
                            transaction: transaction
                        );

                        // Update Address table if exists
                        if (customer.Address != null)
                        {
                            var sqlAddress = @"
                                UPDATE Addresses
                                SET PostCode = @PostCode,
                                    City = @City,
                                    Street = @Street,
                                    StreetNumber = @StreetNumber,
                                    ApartmentNumber = @ApartmentNumber
                                WHERE Id = @Id;
                            ";

                            await sqlExecutor.ExecuteAsync(
                                sqlAddress,
                                new
                                {
                                    customer.Address.Id,
                                    customer.Address.PostCode,
                                    customer.Address.City,
                                    customer.Address.Street,
                                    customer.Address.StreetNumber,
                                    customer.Address.ApartmentNumber
                                },
                                transaction: transaction
                            );
                        }

                        transaction.Commit();
                        return customer;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        logger.Error(ex, $"Cannot update customer with id: {customer.Id}");

                        throw;
                    }
                }
            }
        }
    }
}
