using Domain.Entities;
using Domain.Repositories;
using Infrastructure.Repositories;
using System;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation
{
    public partial class EditCustomerForm : Form
    {
        private readonly ICustomerRepository _repository;

        public Customer Customer { get; private set; }

        public EditCustomerForm(Customer customer, ICustomerRepository repository)
        {
            InitializeComponent();

            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

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

            //btnSave.Click += btnSave_Click;
            //btnCancel.Click += btnCancel_Click;
            //btnDelete.Click += btnDelete_Click;
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
                await _repository.DeleteCustomerAsync(Customer.Id);
                this.DialogResult = DialogResult.OK; // Signal caller to refresh
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
