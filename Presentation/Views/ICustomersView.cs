using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Domain.Entities;
using System;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public interface ICustomersView
    {
        event EventHandler RefreshDataSource;
        event EventHandler<Customer> DeleteCustomer;
        GridControl GridControlCustomers { get; }
        GridView GridViewCustomers { get; }
    }
}
