using Domain.Interfaces;
using Infrastructure.Repositories;
using NSubstitute;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class CustomersRepository_DeleteCustomerAsync_Tests
    {
        private readonly CustomersRepository _sut;

        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ISqlExecutor _sqlExecutor;
        private readonly IDbConnection _connection;
        private readonly IDbTransaction _transaction;
        private readonly ILogger _logger;

        public CustomersRepository_DeleteCustomerAsync_Tests()
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
        public async Task DeleteCustomerAsync_ExecutesDeleteAndCommits()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            await _sut.DeleteCustomerAsync(customerId);

            // Assert
            await _sqlExecutor.Received(1).ExecuteAsync(
                "DELETE FROM Customers WHERE Id = @Id",
                Arg.Is<object>(o =>
                    (Guid)o.GetType().GetProperty("Id").GetValue(o) == customerId),
                _transaction);

            _transaction.Received(1).Commit();
            _transaction.DidNotReceive().Rollback();
        }

        [Fact]
        public async Task DeleteCustomerAsync_WhenSqlFails_RollsBack()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            _sqlExecutor.ExecuteAsync(
                Arg.Any<string>(),
                Arg.Any<object>(),
                Arg.Any<IDbTransaction>())
                .Returns<Task<int>>(x => throw new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(
                () => _sut.DeleteCustomerAsync(customerId));

            _transaction.Received(1).Rollback();
            _transaction.DidNotReceive().Commit();
        }

        [Fact]
        public async Task DeleteCustomerAsync_AlwaysOpensConnection()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            // Act
            await _sut.DeleteCustomerAsync(customerId);

            // Assert
            _connection.Received(1).Open();
        }
    }
}
