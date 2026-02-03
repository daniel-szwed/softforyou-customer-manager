using Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation
{
    public partial class AddCustomerForm : Form
    {
        public Customer Customer { get; private set; }

        public AddCustomerForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Create customer and address objects
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
