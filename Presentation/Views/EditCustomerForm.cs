using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories;
using Softforyou.CustomerManager.Presentation.Presenters;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public partial class EditCustomerForm : Form, IEditCustomerView
    {
        private EditCustomerPresenter presenter;
        public event EventHandler<Customer> EditCustomer;

        public Customer Customer { get; private set; }
        public AutoResetEvent EditCustomerEvent { get; set; }

        public EditCustomerForm(Customer customer)
        {
            InitializeComponent();
            EditCustomerEvent = new AutoResetEvent(false);
            presenter = new EditCustomerPresenter(this);
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));

            // Fill form fields with existing data
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

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "Are you sure you want to delete this customer?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                EditCustomer?.Invoke(this, Customer);
                EditCustomerEvent.WaitOne();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete customer: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
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

            EditCustomer?.Invoke(this, Customer);
            EditCustomerEvent.WaitOne();

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
