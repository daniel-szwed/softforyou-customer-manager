using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Presenters;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public partial class EditCustomerForm : Form, IEditCustomerView
    {
        private readonly EditCustomerPresenter presenter;
        public event EventHandler<Customer> EditCustomer;

        public Customer Customer { get; set; }
        public AutoResetEvent EditCustomerEvent { get; set; }
        public bool EditedSuccessfully { get; set; }

        public EditCustomerForm(Customer customer)
        {
            InitializeComponent();
            presenter = new EditCustomerPresenter(
                this,
                AppServices.Instance.Get<ILogger>(),
                AppServices.Instance.Get<ICustomerRepository>(),
                AppServices.Instance.Get<IMessageService>());
            EditCustomerEvent = new AutoResetEvent(false);
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            FillFormWithCustomerData();
        }

        private void FillFormWithCustomerData()
        {
            txtName.Text = Customer.Name;
            txtTaxId.Text = Customer.TaxId;
            txtPhone.Text = Customer.PhoneNumber;
            txtEmail.Text = Customer.EmailAddress;

            if (Customer.Address != null)
            {
                txtPostCode.Text = Customer.Address.PostCode;
                txtCity.Text = Customer.Address.City;
                txtStreet.Text = Customer.Address.Street;
                txtStreetNumber.Text = Customer.Address.StreetNumber;
                txtApartmentNumber.Text = Customer.Address.ApartmentNumber;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FillCustomerWithFormData();

            EditCustomer?.Invoke(this, Customer);
            EditCustomerEvent.WaitOne();

            if (EditedSuccessfully)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void FillCustomerWithFormData()
        {
            Customer.Name = txtName.Text;
            Customer.TaxId = txtTaxId.Text;
            Customer.PhoneNumber = txtPhone.Text;
            Customer.EmailAddress = txtEmail.Text;

            if (Customer.Address == null)
                Customer.Address = new Address();

            Customer.Address.PostCode = txtPostCode.Text;
            Customer.Address.City = txtCity.Text;
            Customer.Address.Street = txtStreet.Text;
            Customer.Address.StreetNumber = txtStreetNumber.Text;
            Customer.Address.ApartmentNumber = txtApartmentNumber.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
