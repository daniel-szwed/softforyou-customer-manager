using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Repositories;
using NSubstitute;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class CustomersRepository_UpdateCustomerAsync_Tests
    {
        private readonly CustomersRepository _sut;

        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly ILogger _logger;

        public CustomersRepository_UpdateCustomerAsync_Tests()
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
        public async Task UpdateCustomerAsync_WithValidCustomer_UpdatesCustomerAndAddress_AndCommits()
        {
            // Arrange
            _sqlExecutor
                .ExecuteAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<IDbTransaction>())
                .Returns(Task.FromResult(1));

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "John",
                TaxId = "123",
                PhoneNumber = "555-1234",
                EmailAddress = "john@example.com",
                Address = new Address
                {
                    Id = Guid.NewGuid(),
                    PostCode = "00-001",
                    City = "Warsaw",
                    Street = "Main",
                    StreetNumber = "1",
                    ApartmentNumber = "2A"
                }
            };

            // Act
            var result = await _sut.UpdateCustomerAsync(customer);

            await _sqlExecutor.Received(2).ExecuteAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>()
);
            // Assert transaction committed
            _transaction.Received(1).Commit();
            _transaction.DidNotReceive().Rollback();

            // Assert returned customer
            Assert.Equal(customer, result);
        }

        [Fact]
        public async Task UpdateCustomerAsync_WhenSqlFails_RollsBack()
        {
            // Arrange
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                Name = "John",
                Address = new Address { Id = Guid.NewGuid(), PostCode = "00-001", City = "Warsaw", Street = "Main", StreetNumber = "1" }
            };

            _sqlExecutor.ExecuteAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>())
                .Returns<Task<int>>(x => throw new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _sut.UpdateCustomerAsync(customer));

            _transaction.Received(1).Rollback();
            _transaction.DidNotReceive().Commit();
        }

        [Fact]
        public async Task UpdateCustomerAsync_NullCustomer_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.UpdateCustomerAsync(null));
        }

        [Fact]
        public async Task UpdateCustomerAsync_EmptyCustomerId_ThrowsArgumentException()
        {
            var customer = new Customer
            {
                Id = Guid.Empty,
                Name = "John"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.UpdateCustomerAsync(customer));
        }
    }
}
