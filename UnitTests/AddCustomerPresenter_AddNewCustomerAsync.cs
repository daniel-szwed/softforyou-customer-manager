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
    public class AddCustomerPresenter_AddNewCustomerAsync
    {
        private readonly IAddCustomerView _view;
        private readonly ILogger _logger;
        private readonly ICustomerRepository _repository;
        private readonly IMessageService _messageService;
        private readonly AddCustomerPresenter _sut;

        public AddCustomerPresenter_AddNewCustomerAsync()
        {
            _view = Substitute.For<IAddCustomerView>();
            _logger = Substitute.For<ILogger>();
            _repository = Substitute.For<ICustomerRepository>();
            _messageService = Substitute.For<IMessageService>();

            _view.AddNewCustomerEvent = new AutoResetEvent(false);

            _sut = new AddCustomerPresenter(
                _view,
                _logger,
                _repository,
                _messageService);
        }

        private static Customer GetValidCustomer()
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
        public async Task AddNewCustomerAsync_ValidCustomer_AddsCustomer()
        {
            Customer customer = GetValidCustomer();

            _repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, Arg.Any<Guid>())
                .Returns(false);

            await _sut.AddNewCustomerAsync(this, customer);

            await _repository.Received(1).AddCustomerAsync(customer);
            Assert.True(_view.AddedSuccessfully);

            _messageService.DidNotReceiveWithAnyArgs()
                .ShowWarning(default, default);
        }



        [Fact]
        public async Task AddNewCustomerAsync_TaxIdTaken_ShowsWarning()
        {
            var customer = GetValidCustomer();

            _repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, Arg.Any<Guid>())
                .Returns(true);

            await _sut.AddNewCustomerAsync(this, customer);

            await _repository.DidNotReceive().AddCustomerAsync(Arg.Any<Customer>());

            _messageService.Received(1).ShowWarning(
                Arg.Is<string>(m => m.Contains("already taken")),
                "Validation error");

            Assert.False(_view.AddedSuccessfully);
        }

        [Fact]
        public async Task AddNewCustomerAsync_InvalidModel_ShowsValidationWarning()
        {
            var customer = new Customer
            {
                TaxId = "123",
                Address = new Address()
            };

            _repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, Arg.Any<Guid>())
                .Returns(false);

            await _sut.AddNewCustomerAsync(this, customer);

            await _repository.DidNotReceive().AddCustomerAsync(Arg.Any<Customer>());

            _messageService.Received(1).ShowWarning(
                Arg.Any<string>(),
                "Validation error");
        }

        [Fact]
        public async Task AddNewCustomerAsync_Exception_ShowsError()
        {
            var customer = GetValidCustomer();

            _repository
                .IsTaxIdAlreadyTakenAsync(customer.TaxId, Arg.Any<Guid>())
                .Returns(false);

            _repository
                .AddCustomerAsync(customer)
                .Throws(new Exception("DB error"));

            await _sut.AddNewCustomerAsync(this, customer);

            _messageService.Received(1).ShowError(
                Arg.Any<string>(),
                "Operation error");

            Assert.False(_view.AddedSuccessfully);
        }
    }
}
