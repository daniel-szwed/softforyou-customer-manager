using Domain.Repositories;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Views;
using System;

namespace Softforyou.CustomerManager.Presentation.Presenters
{
    public class CustomersPresenter
    {
        private readonly ICustomersView view;
        private readonly ICustomerRepository customersRepository;

        public CustomersPresenter(ICustomersView view)
        {
            this.view = view;
            this.customersRepository = AppServices.Get<ICustomerRepository>();
            SubsribeToViewEvents();
        }

        private void SubsribeToViewEvents()
        {
            view.ViewLoad += View_ViewLoad;
        }

        private async void View_ViewLoad(object sender, EventArgs e)
        {
            var pagedResult = await customersRepository.GetCustomersAsync(
                view.CurrentPage,
                view.PageSize
            );

            view.GridControlCustomers.DataSource = pagedResult.Result;
            view.GridControlCustomers.RefreshDataSource();

            view.TotalPages = (int)Math.Ceiling(
                (double)pagedResult.TotalCount / view.PageSize
            );

            view.LabelPageInfo.Text = $"Page {view.CurrentPage} / {view.TotalPages}";
            view.PreviousPageButton.Enabled = view.CurrentPage > 1;
            view.NextPageButton.Enabled = view.CurrentPage < view.TotalPages;
        }
    }
}
