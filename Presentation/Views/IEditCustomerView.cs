using Domain.Entities;
using System;
using System.Threading;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public interface IEditCustomerView
    {
        event EventHandler<Customer> EditCustomer;
        AutoResetEvent EditCustomerEvent { get; set; }
    }
}
