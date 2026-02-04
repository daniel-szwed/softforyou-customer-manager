using Domain.Entities;
using Domain.Repositories;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Presenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public partial class AddCustomerForm : Form, IAddCustomerView
    {
        private AddCustomerPresenter presenter;

        public AutoResetEvent AddNewCustomerEvent { get; set; }

        public AddCustomerForm()
        {
            InitializeComponent();
            AddNewCustomerEvent = new AutoResetEvent(false);
            presenter = new AddCustomerPresenter(this);
        }

        public event EventHandler<Customer> AddNewCustomer;

        private void btnSave_Click(object sender, EventArgs e)
        {
            var customer = new Customer
            {
                Name = txtName.Text,
                TaxId = txtTaxId.Text,
                PhoneNumber = txtPhone.Text,
                EmailAddress = txtEmail.Text,
                Address = new Address
                {
                    PostCode = txtPostCode.Text,
                    City = txtCity.Text,
                    Street = txtStreet.Text,
                    StreetNumber = txtStreetNumber.Text,
                    ApartmentNumber = txtApartmentNumber.Text
                }
            };

            AddNewCustomer?.Invoke(this, customer);
            AddNewCustomerEvent.WaitOne();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

}
