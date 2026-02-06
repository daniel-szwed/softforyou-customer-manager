using Domain.Entities;
using Infrastructure.Repositories;
using NSubstitute;
using System.Data;
using Domain.Interfaces;
using System.Threading.Tasks;
using System;
using Xunit;

namespace UnitTests
{
    public class CustomersRepository_AddCustomerAsync_Tests
    {
        private readonly CustomersRepository _sut;
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly ILogger _logger;

        public CustomersRepository_AddCustomerAsync_Tests()
        {
            _connectionProvider = Substitute.For<IDbConnectionProvider>();
            _sqlExecutor = Substitute.For<ISqlExecutor>();
            _connection = Substitute.For<IDbConnection>();
            _transaction = Substitute.For<IDbTransaction>();
            _logger = Substitute.For<ILogger>();

            _connectionProvider.GetDbConnection().Returns(_connection);
            _connection.BeginTransaction().Returns(_transaction);

            _sut = new CustomersRepository(_logger, _connectionProvider, _sqlExecutor);
        }

        [Fact]
        public async Task AddCustomerAsync_NullCustomer_Throws()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddCustomerAsync(null));
        }

        [Fact]
        public async Task AddCustomerAsync_NullAddress_Throws()
        {
            var customer = new Customer { Name = "Test" };

            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddCustomerAsync(customer));
        }

        [Fact]
        public async Task AddCustomerAsync_EmptyCustomerId_GeneratesId()
        {
            var customer = new Customer
            {
                Id = Guid.Empty,
                Name = "Test",
                Address = new Address
                {
                    PostCode = "00-001",
                    City = "Warsaw",
                    Street = "Main",
                    StreetNumber = "1"
                }
            };

            var result = await _sut.AddCustomerAsync(customer);

            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task AddCustomerAsync_EmptyAddressId_SetsSameAsCustomer()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Address = new Address
                {
                    Id = Guid.Empty,
                    PostCode = "00-001",
                    City = "Warsaw",
                    Street = "Main",
                    StreetNumber = "1"
                }
            };

            await _sut.AddCustomerAsync(customer);

            Assert.Equal(customer.Id, customer.Address.Id);
        }

        [Fact]
        public async Task AddCustomerAsync_ValidCustomer_InsertsCustomerAndAddress_AndCommits()
        {
            var customer = new Customer
            {
                Name = "Test",
                Address = new Address
                {
                    PostCode = "00-001",
                    City = "Warsaw",
                    Street = "Main",
                    StreetNumber = "1"
                }
            };

            await _sut.AddCustomerAsync(customer);

            await _sqlExecutor.Received(2).ExecuteAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                _transaction);

            _transaction.Received(1).Commit();
        }

        [Fact]
        public async Task AddCustomerAsync_WhenSqlFails_RollsBack()
        {
            _sqlExecutor.ExecuteAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>())
                .Returns<Task<int>>(x => throw new Exception("DB error"));

            var customer = new Customer
            {
                Name = "Test",
                Address = new Address
                {
                    PostCode = "00-001",
                    City = "Warsaw",
                    Street = "Main",
                    StreetNumber = "1"
                }
            };

            await Assert.ThrowsAsync<Exception>(() => _sut.AddCustomerAsync(customer));

            _transaction.Received(1).Rollback();
        }
    }
}
