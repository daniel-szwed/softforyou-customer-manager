using DevExpress.XtraGrid;
using Domain.Entities;
using Softforyou.CustomerManager.Presentation.Presenters;
using System;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public partial class CustomersForm : Form, ICustomersView
    {
        private CustomersPresenter presenter;

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public GridControl GridControlCustomers => this.gridControlCustomers; 
        public Label LabelPageInfo => this.lblPageInfo;
        public Button PreviousPageButton => this.btnPreviousPage;
        public Button NextPageButton => this.btnNextPage;

        public event EventHandler ViewLoad;

        public CustomersForm()
        {
            InitializeComponent();
            presenter = new CustomersPresenter(this);

            btnAddCustomer.Click += ButtonAddCustomer_ClickAsync;

            gridViewCustomers.DoubleClick += GridViewCustomers_DoubleClick;

            gridViewCustomers.Columns.Clear();
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
            btnNextPage.Click += (s, e) =>
            {
                CurrentPage++; ViewLoad?.Invoke(s, e);
            };
            btnPreviousPage.Click += (s, e) => { CurrentPage--; if (CurrentPage < 1) CurrentPage = 1; ViewLoad?.Invoke(s, e); };

            // Load first page
            this.Load += ViewLoad; 
        }

        private void GridViewCustomers_DoubleClick(object sender, EventArgs eventArgs)
        {
            var rowHandle = gridViewCustomers.FocusedRowHandle;
            if (rowHandle < 0) return;

            var customer = gridViewCustomers.GetRow(rowHandle) as Customer;
            if (customer == null) return;

            using (var editForm = new EditCustomerForm(customer))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    ViewLoad?.Invoke(sender, eventArgs);
                }
            }
        }

        private void ButtonAddCustomer_ClickAsync(object sender, EventArgs eventArgs)
        {
            using (var addForm = new AddCustomerForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    ViewLoad?.Invoke(sender, eventArgs);
                }
            }
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
    }
}
