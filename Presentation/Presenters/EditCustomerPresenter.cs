using Application.Validation;
using Domain.Entities;
using Domain.Interfaces;
using Softforyou.CustomerManager.Presentation.Views;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Presenters
{
    public class EditCustomerPresenter
    {
        private readonly IEditCustomerView view;
        private readonly ILogger logger;
        private readonly ICustomerRepository customerRepository;
        private readonly IMessageService messageService;

        public EditCustomerPresenter(
            IEditCustomerView view,
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
            view.EditCustomer += View_EditCustomer;
        }

        private void View_EditCustomer(object sender, Customer customer)
        {
            Task.Run(async () => EditCutomerAsync(customer));
        }

        public async Task<bool> EditCutomerAsync(Customer customer)
        {
            try
            {
                view.EditedSuccessfully = false;
                if (await customerRepository.IsTaxIdAlreadyTakenAsync(customer.TaxId, customer.Id))
                {
                    messageService.ShowWarning(
                        $"Tax ID: {customer.TaxId} is already taken.",
                        "Validation error"
                    );

                    return false;
                }

                var errors = ModelValidator.Validate(customer)
                    .Concat(ModelValidator.Validate(customer.Address));

                if (errors.Any())
                {
                    var message = new StringBuilder();
                    foreach (var error in errors)
                    {
                        message.AppendLine($"{error.PropertyName}: {error.Message}");
                    }

                    messageService.ShowWarning(
                        message.ToString(),
                        "Validation error"
                    );

                    return false;
                }

                await customerRepository.UpdateCustomerAsync(customer);
                view.EditedSuccessfully = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurs during customer edit.");
                messageService.ShowError(
                                    "Something went wrong,\nplease inspect log.txt file for more details.",
                                    "Operation error"
                                );
            }
            finally
            {
                view.EditCustomerEvent.Set();
            }

            return true;
        }
    }
}
