using Dapper;
using Domain.Entities;
using Domain.Repositories;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Linq;
using System.Data.SqlClient;
using System;

namespace Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbConnectionProvider dbConnectionProvider;

        public CustomerRepository(IDbConnectionProvider dbConnectionProvider)
        {
            this.dbConnectionProvider = dbConnectionProvider;
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Ensure customer has an Id
            if (customer.Id == Guid.Empty)
                customer.Id = Guid.NewGuid();

            // If customer has Address, set the same Id (1-to-1)
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
                        // Insert Customer
                        var sqlCustomer = @"
                            INSERT INTO Customers (Id, Name, TaxId, PhoneNumber, EmailAddress, AddressId)
                            VALUES (@Id, @Name, @TaxId, @PhoneNumber, @EmailAddress, @AddressId);
                        ";

                        await connection.ExecuteAsync(
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

                        // Insert Address if exists
                        if (customer.Address != null)
                        {
                            var sqlAddress = @"
                                INSERT INTO Addresses (Id, PostCode, City, Street, StreetNumber, ApartmentNumber)
                                VALUES (@Id, @PostCode, @City, @Street, @StreetNumber, @ApartmentNumber);
                            ";

                            await connection.ExecuteAsync(
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
                    catch
                    {
                        transaction.Rollback();
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
                        // Delete customer (will cascade delete address)
                        var sql = "DELETE FROM Customers WHERE Id = @Id";
                        await connection.ExecuteAsync(sql, new { Id = customerId }, transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }


        public async Task<PagedResult<Customer>> GetCustomersAsync(int pageNumber, int pageSize)
        {
            var lookup = new List<Customer>();
            int totalCount = 0;

            var sql = @"
                    SELECT
                        c.Id, c.Name, c.TaxId, c.PhoneNumber, c.EmailAddress, c.AddressId,
                        a.Id, a.PostCode, a.City, a.Street, a.StreetNumber, a.ApartmentNumber,
                        COUNT(*) OVER() AS TotalCount
                    FROM Customers c
                    LEFT JOIN Addresses a ON a.Id = c.Id
                    ORDER BY c.Name
                    OFFSET @Offset ROWS
                    FETCH NEXT @PageSize ROWS ONLY;
                    ";

            using (IDbConnection connection = dbConnectionProvider.GetDbConnection())
            {
                connection.Open();

                try
                {
                    var result = await connection.QueryAsync<Customer, Address, int, Customer>(
                        sql,
                        (c, a, count) =>
                        {
                            c.Address = a;
                            totalCount = count;
                            lookup.Add(c);
                            return c;
                        },
                        new
                        {
                            Offset = (pageNumber - 1) * pageSize,
                            PageSize = pageSize
                        },
                        splitOn: "Id,TotalCount"
                    );

                    return new PagedResult<Customer>
                    {
                        Result = lookup.ToArray(),
                        TotalCount = totalCount
                    };
                }
                catch
                {
                    throw;
                }
            }
        }

        public async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            // Ensure customer has an Id
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

                        await connection.ExecuteAsync(
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

                            await connection.ExecuteAsync(
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
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
