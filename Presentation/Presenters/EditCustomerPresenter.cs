using Domain.Entities;
using Domain.Repositories;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Views;
using System.Threading.Tasks;

namespace Softforyou.CustomerManager.Presentation.Presenters
{
    public class EditCustomerPresenter
    {
        private readonly IEditCustomerView view;
        private readonly ICustomerRepository customerRepository;

        public EditCustomerPresenter(IEditCustomerView view)
        {
            this.view = view;
            this.customerRepository = AppServices.Get<ICustomerRepository>();
            SubsribeToViewEvents();
        }

        private void SubsribeToViewEvents()
        {
            view.EditCustomer += View_EditCustomer;
        }

        private void View_EditCustomer(object sender, Customer customer)
        {
            Task.Run(async () =>
            {
                await customerRepository.UpdateCustomerAsync(customer);
                view.EditCustomerEvent.Set();
            });
        }
    }
}
