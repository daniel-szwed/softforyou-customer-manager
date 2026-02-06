using Domain.Entities;
using Domain.Interfaces;
using Softforyou.CustomerManager.Presentation.Views;
using System;
using System.Collections.ObjectModel;

namespace Softforyou.CustomerManager.Presentation.Presenters
{
    public class CustomersPresenter
    {
        private readonly ICustomersView view;
        private readonly ILogger logger;
        private readonly ICustomerRepository customersRepository;
        private readonly IMessageService messageService;

        public CustomersPresenter(
            ICustomersView view,
            ILogger logger,
            ICustomerRepository customersRepository,
            IMessageService messageService)
        {
            this.view = view;
            this.logger = logger;
            this.customersRepository = customersRepository;
            this.messageService = messageService;

            SubsribeToViewEvents();
        }

        private void SubsribeToViewEvents()
        {
            view.RefreshDataSource += View_RefreshDataSource;
            view.DeleteCustomer += DeleteCustomerAsync;
        }

        public async void DeleteCustomerAsync(object sender, Customer customer)
        {
            try
            {
                await customersRepository.DeleteCustomerAsync(customer.Id);

                var dataSource = (ObservableCollection<Customer>)view.GridControlCustomers.DataSource;
                dataSource.Remove(customer);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "An error occurs during customer deletion.");
                messageService.ShowError(
                    "Something went wrong.\nPlease inspect log.txt file for more details.",
                    "Operation error");
            }
        }

        private async void View_RefreshDataSource(object sender, EventArgs e)
        {
            try
            {
                if (sender is AddCustomerForm addCustomerView)
                {
                    var newCustomer = addCustomerView.Customer;
                    var dataSource = (ObservableCollection<Customer>)view.GridControlCustomers.DataSource;
                    dataSource.Add(newCustomer);
                    view.GridControlCustomers.RefreshDataSource();

                    return;
                }

                if (sender is EditCustomerForm editCustomerView)
                {
                    return;
                }

                var customers = await customersRepository.GetAllCustomersAsync();

                view.GridControlCustomers.DataSource = new ObservableCollection<Customer>(customers); ;
                view.GridControlCustomers.RefreshDataSource();
            }
            catch(Exception ex) 
            {
                logger.Error(ex, "Something went wrong, inspect log.txt file for more details");
                messageService.ShowError(
                    "Something went wrong.\nPlease inspect log.txt file for more details.",
                    "Operation error");
            }
        }
    }
}
