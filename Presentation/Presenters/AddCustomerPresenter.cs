using Domain.Entities;
using Domain.Repositories;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Views;
using System;
using System.Threading.Tasks;

namespace Softforyou.CustomerManager.Presentation.Presenters
{
    public class AddCustomerPresenter
    {
        private readonly IAddCustomerView view;
        private readonly ICustomerRepository customerRepository;

        public AddCustomerPresenter(IAddCustomerView view)
        {
            this.view = view;
            this.customerRepository = AppServices.Get<ICustomerRepository>();
            SubsribeToViewEvents();
        }

        private void SubsribeToViewEvents()
        {
            view.AddNewCustomer += View_AddNewCustomerAsync;
        }

        private void View_AddNewCustomerAsync(object sender, Customer customer)
        {
            Task.Run(async () =>
            {
                await customerRepository.AddCustomerAsync(customer);
                view.AddNewCustomerEvent.Set();
            });
        }
    }
}
