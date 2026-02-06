using Application.Validation;
using Domain.Entities;
using Domain.Interfaces;
using Softforyou.CustomerManager.Presentation.Views;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Softforyou.CustomerManager.Presentation.Presenters
{
    public interface IAddCustomerPresenter
    {
        Task AddNewCustomerAsync(object sender, Customer customer);
    }

    public class AddCustomerPresenter : IAddCustomerPresenter
    {
        private readonly IAddCustomerView view;
        private readonly ILogger logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IMessageService messageService;

        public AddCustomerPresenter(
            IAddCustomerView view,
            ILogger logger,
            ICustomerRepository customerRepository,
            IMessageService messageService)
        {
            this.view = view;
            this.logger = logger;
            this.customerRepository = customerRepository;
            this.messageService = messageService;
            SubsribeToViewEvents();
        }

        private void SubsribeToViewEvents()
        {
            view.AddNewCustomer += View_AddNewCustomerAsync;
        }

        private void View_AddNewCustomerAsync(object sender, Customer customer)
        {
            Task.Run(async () => AddNewCustomerAsync(sender, customer));
        }

        public async Task AddNewCustomerAsync(object sender, Customer customer)
        {
            try
            {
                view.AddedSuccessfully = false;

                if (await customerRepository.IsTaxIdAlreadyTakenAsync(customer.TaxId, Guid.Empty))
                {
                    messageService.ShowWarning(
                        $"Tax ID: {customer.TaxId} is already taken.",
                        "Validation error");

                    return;
                }

                var errors = ModelValidator.Validate(customer)
                    .Concat(ModelValidator.Validate(customer.Address));

                if (errors.Any())
                {
                    var message = string.Join(
                        Environment.NewLine,
                        errors.Select(e => $"{e.PropertyName}: {e.Message}")
                    );

                    messageService.ShowWarning(message, "Validation error");
                    return;
                }

                await customerRepository.AddCustomerAsync(customer);
                view.AddedSuccessfully = true;
            }
            catch(Exception ex)
            {
                logger.Error(ex, "An error occurs durinc customer creation.");
                messageService.ShowError(
                    "Something went wrong.\nPlease inspect log.txt file for more details.",
                    "Operation error");
            }
            finally
            {
                view.AddNewCustomerEvent.Set();
            }
        }
    }
}
