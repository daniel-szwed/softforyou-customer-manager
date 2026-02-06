using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure;
using Softforyou.CustomerManager.Presentation.Presenters;
using System;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public partial class CustomersForm : Form, ICustomersView
    {
        private readonly IMessageService messageService;
        private readonly CustomersPresenter presenter;

        public GridControl GridControlCustomers => this.gridControlCustomers;

        public GridView GridViewCustomers => this.gridViewCustomers;

        public event EventHandler RefreshDataSource;
        public event EventHandler<Customer> DeleteCustomer;

        public CustomersForm()
        {
            InitializeComponent();
            this.messageService = AppServices.Instance.Get<IMessageService>();
            presenter = new CustomersPresenter(
                this,
                AppServices.Instance.Get<Domain.Interfaces.ILogger>(),
                AppServices.Instance.Get<ICustomerRepository>(),
                AppServices.Instance.Get<IMessageService>());

            btnAddCustomer.Click += ButtonAddCustomer_Click;
            gridViewCustomers.DoubleClick += GridViewCustomers_EditCustomer_DoubleClick;
            gridViewCustomers.PopupMenuShowing += GridViewCustomers_PopupMenuShowing;

            SetupGridColumns();
            SetupUnboundColumns();

            gridViewCustomers.OptionsBehavior.Editable = false;
            gridViewCustomers.OptionsView.ShowAutoFilterRow = true;
            gridViewCustomers.OptionsView.ShowGroupPanel = true;

            this.Load += RefreshDataSource;
        }

        private void GridViewCustomers_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
        {
            if (e.HitInfo.InRow || e.HitInfo.InRowCell)
            {
                gridViewCustomers.FocusedRowHandle = e.HitInfo.RowHandle;

                var customer = gridViewCustomers.GetRow(e.HitInfo.RowHandle) as Customer;
                if (customer == null)
                    return;

                var menu = new ContextMenuStrip();

                menu.Items.Add("Edit", null, (s, ev) => GridViewCustomers_EditCustomer_DoubleClick(s, ev));
                menu.Items.Add("Delete", null, (s, ev) => AskUserAndDeleteCustomer(s, ev));

                menu.Show(Control.MousePosition);
            }
        }

        private void AskUserAndDeleteCustomer(object s, EventArgs ev)
        {
            var confirm = messageService.ShowConfirmation(
                "Are you sure you want to delete this customer?",
                "Confirm Delete");

            if (confirm != DialogResult.Yes)
                return;

            var rowHandle = GridViewCustomers.FocusedRowHandle;
            if (rowHandle < 0)
                return;

            var customer = GridViewCustomers.GetRow(rowHandle) as Customer;

            DeleteCustomer(s, customer);
        }

        private void SetupUnboundColumns()
        {
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

            gridViewCustomers.CustomUnboundColumnData += GridViewCustomers_CustomUnboundColumnData;
        }

        private void SetupGridColumns()
        {
            gridViewCustomers.Columns.Clear();
            gridViewCustomers.Columns.AddVisible("Name", "Name");
            gridViewCustomers.Columns.AddVisible("TaxId", "Tax ID");
            gridViewCustomers.Columns.AddVisible("PhoneNumber", "Phone");
            gridViewCustomers.Columns.AddVisible("EmailAddress", "Email");
        }

        private void GridViewCustomers_EditCustomer_DoubleClick(object sender, EventArgs eventArgs)
        {
            var rowHandle = gridViewCustomers.FocusedRowHandle;
            if (rowHandle < 0) return;

            var customer = gridViewCustomers.GetRow(rowHandle) as Customer;
            if (customer == null) return;

            using (var editForm = new EditCustomerForm(customer))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    RefreshDataSource?.Invoke(editForm, eventArgs);
                }
            }
        }

        private void ButtonAddCustomer_Click(object sender, EventArgs eventArgs)
        {
            using (var addForm = new AddCustomerForm())
            {
                if (addForm.ShowDialog() == DialogResult.OK)
                {
                    RefreshDataSource?.Invoke(addForm, eventArgs);
                }
            }
        }

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
