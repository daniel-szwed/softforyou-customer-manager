using Domain.Entities;
using System;
using System.Threading;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public interface IAddCustomerView
    {
        event EventHandler<Customer> AddNewCustomer;
        AutoResetEvent AddNewCustomerEvent { get; set; }
    }
}
