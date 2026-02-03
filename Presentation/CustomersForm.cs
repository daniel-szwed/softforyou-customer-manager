using DevExpress.Data;
using Domain.Entities;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Repositories;
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
    public partial class CustomersForm : Form
    {
        private int currentPage = 1;
        private const int pageSize = 10; // change as needed
        private int totalPages = 1;

        private readonly ICustomerRepository _repository;

        public CustomersForm()
        {
            InitializeComponent();

            // Initialize repository
            _repository = AppServices.Get<ICustomerRepository>();

            // Add “Add Customer” button click
            btnAddCustomer.Click += async (s, e) =>
            {
                using (var addForm = new AddCustomerForm())
                {
                    if (addForm.ShowDialog() == DialogResult.OK)
                    {
                        var newCustomer = addForm.Customer;
                        await _repository.AddCustomerAsync(newCustomer);
                        await LoadCustomersAsync();
                    }
                }
            };

            // Setup GridView columns for Customer properties
            gridViewCustomers.Columns.Clear();

            gridViewCustomers.DoubleClick += async (s, e) =>
            {
                var rowHandle = gridViewCustomers.FocusedRowHandle;
                if (rowHandle < 0) return;

                var customer = gridViewCustomers.GetRow(rowHandle) as Customer;
                if (customer == null) return;

                using (var editForm = new EditCustomerForm(customer, _repository))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        // Save changes in database
                        await _repository.UpdateCustomerAsync(customer);

                        // Reload grid
                        await LoadCustomersAsync();
                    }
                }
            };

            gridViewCustomers.Columns.AddVisible("Name", "Name");
            gridViewCustomers.Columns.AddVisible("TaxId", "Tax ID");
            gridViewCustomers.Columns.AddVisible("PhoneNumber", "Phone");
            gridViewCustomers.Columns.AddVisible("EmailAddress", "Email");

            // Setup unbound columns for Address properties
            string[] addressFields = { "PostCode", "City", "Street", "StreetNumber", "ApartmentNumber" };
            foreach (var field in addressFields)
            {
                gridViewCustomers.Columns.Add(new DevExpress.XtraGrid.Columns.GridColumn()
                {
                    FieldName = field,
                    Caption = field,
                    Visible = true,
                    UnboundType = DevExpress.Data.UnboundColumnType.String
                });
            }

            // Assign unbound column handler
            gridViewCustomers.CustomUnboundColumnData += GridViewCustomers_CustomUnboundColumnData;

            // Make grid read-only and enable filtering/grouping
            gridViewCustomers.OptionsBehavior.Editable = false;
            gridViewCustomers.OptionsView.ShowAutoFilterRow = true;
            gridViewCustomers.OptionsView.ShowGroupPanel = true;

            // Wire paging buttons
            btnNextPage.Click += async (s, e) => { currentPage++; await LoadCustomersAsync(); };
            btnPreviousPage.Click += async (s, e) => { currentPage--; if (currentPage < 1) currentPage = 1; await LoadCustomersAsync(); };

            // Load first page
            this.Load += async (s, e) => { await LoadCustomersAsync(); };
        }

        // Unbound column handler to fill Address columns
        private void GridViewCustomers_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            if (!(e.Row is Customer customer) || customer.Address == null)
                return;

            switch (e.Column.FieldName)
            {
                case "PostCode":
                    e.Value = customer.Address.PostCode ?? string.Empty;
                    break;
                case "City":
                    e.Value = customer.Address.City ?? string.Empty;
                    break;
                case "Street":
                    e.Value = customer.Address.Street ?? string.Empty;
                    break;
                case "StreetNumber":
                    e.Value = customer.Address.StreetNumber ?? string.Empty;
                    break;
                case "ApartmentNumber":
                    e.Value = customer.Address.ApartmentNumber ?? string.Empty;
                    break;
            }
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var pagedResult = await _repository.GetCustomersAsync(currentPage, pageSize);
                gridControlCustomers.DataSource = pagedResult.Result;
                gridControlCustomers.RefreshDataSource();

                // Update paging info
                totalPages = (int)Math.Ceiling((double)pagedResult.TotalCount / pageSize);
                lblPageInfo.Text = $"Page {currentPage} / {totalPages}";

                // Disable previous/next if needed
                btnPreviousPage.Enabled = currentPage > 1;
                btnNextPage.Enabled = currentPage < totalPages;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}");
            }
        }
    }
}
