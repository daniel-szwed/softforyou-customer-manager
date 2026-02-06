using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Presenters;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public partial class AddCustomerForm : Form, IAddCustomerView
    {
        private readonly IAddCustomerPresenter presenter;
        public AutoResetEvent AddNewCustomerEvent { get; set; }
        public Customer Customer { get; set; }
        public bool AddedSuccessfully { get; set; }

        public AddCustomerForm()
        {
            InitializeComponent();
            AddNewCustomerEvent = new AutoResetEvent(false);
            presenter = new AddCustomerPresenter(
                this,
                AppServices.Instance.Get<ILogger>(),
                AppServices.Instance.Get<ICustomerRepository>(),
                AppServices.Instance.Get<IMessageService>());
        }

        public event EventHandler<Customer> AddNewCustomer;

        private void btnSave_Click(object sender, EventArgs e)
        {
            Customer = new Customer
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

            AddNewCustomer?.Invoke(this, Customer);
            AddNewCustomerEvent.WaitOne();

            if (AddedSuccessfully)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

}
