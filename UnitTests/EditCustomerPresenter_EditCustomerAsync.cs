using Domain.Entities;
using Domain.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Softforyou.CustomerManager.Presentation.Presenters;
using Softforyou.CustomerManager.Presentation.Views;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class EditCustomerPresenter_EditCustomerAsync
    {
        private readonly IEditCustomerView view;
        private readonly ILogger logger;
        private readonly ICustomerRepository repository;
        private readonly IMessageService messageService;

        private readonly EditCustomerPresenter presenter;

        public EditCustomerPresenter_EditCustomerAsync()
        {
            view = Substitute.For<IEditCustomerView>();
            logger = Substitute.For<ILogger>();
            repository = Substitute.For<ICustomerRepository>();
            messageService = Substitute.For<IMessageService>();

            view.EditCustomerEvent.Returns(new AutoResetEvent(false));

            presenter = new EditCustomerPresenter(
                view,
                logger,
                repository,
                messageService
            );
        }

        private static Customer CreateValidCustomer()
        {
            return new Customer
            {
                Name = "Test",
                TaxId = "123",
                PhoneNumber = "1234567890",
                EmailAddress = "test@test.test",
                Address = new Address()
                {
                    PostCode = "123",
                    City = "City",
                    Street = "Street",
                    StreetNumber = "123",
                    ApartmentNumber = null
                }
            };
        }

        [Fact]
        public async Task EditCustomerAsync_TaxIdAlreadyTaken_ShowsWarningAndReturnsFalse()
        {
            // Arrange
            var customer = CreateValidCustomer();

            repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, customer.Id)
                .Returns(true);

            // Act
            var result = await presenter.EditCutomerAsync(customer);

            // Assert
            Assert.False(result);

            await repository.DidNotReceive()
                .UpdateCustomerAsync(Arg.Any<Customer>());

            messageService.Received(1).ShowWarning(
                $"Tax ID: {customer.TaxId} is already taken.",
                "Validation error"
            );

            Assert.False(view.EditedSuccessfully);
        }

        [Fact]
        public async Task EditCustomerAsync_ModelValidationFails_ShowsWarningAndReturnsFalse()
        {
            // Arrange
            var customer = CreateValidCustomer();
            customer.TaxId = null; // force validation error

            repository
                .IsTaxIdAlreadyTakenAsync(Arg.Any<string>(), Arg.Any<Guid>())
                .Returns(false);

            // Act
            var result = await presenter.EditCutomerAsync(customer);

            // Assert
            Assert.False(result);

            await repository.DidNotReceive()
                .UpdateCustomerAsync(Arg.Any<Customer>());

            messageService.Received(1).ShowWarning(
                Arg.Is<string>(msg => msg.Contains("TaxId")),
                "Validation error"
            );
        }

        [Fact]
        public async Task EditCustomerAsync_ValidCustomer_UpdatesCustomerAndReturnsTrue()
        {
            // Arrange
            var customer = CreateValidCustomer();

            repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, customer.Id)
                .Returns(false);

            // Act
            var result = await presenter.EditCutomerAsync(customer);

            // Assert
            Assert.True(result);

            await repository.Received(1)
                .UpdateCustomerAsync(customer);

            Assert.True(view.EditedSuccessfully);

            messageService.DidNotReceiveWithAnyArgs()
                .ShowWarning(default, default);

            messageService.DidNotReceiveWithAnyArgs()
                .ShowError(default, default);
        }

        [Fact]
        public async Task EditCustomerAsync_ExceptionThrown_LogsErrorAndShowsErrorMessage()
        {
            // Arrange
            var customer = CreateValidCustomer();

            repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, customer.Id)
                .Returns(false);

            repository
                .UpdateCustomerAsync(customer)
                .Throws(new Exception("DB error"));

            // Act
            var result = await presenter.EditCutomerAsync(customer);

            // Assert
            Assert.True(result); // current implementation returns true even on exception

            logger.Received(1)
                .Error(Arg.Any<Exception>(), "An error occurs during customer edit.");

            messageService.Received(1).ShowError(
                "Something went wrong,\nplease inspect log.txt file for more details.",
                "Operation error"
            );

            Assert.False(view.EditedSuccessfully);
        }
    }
}
