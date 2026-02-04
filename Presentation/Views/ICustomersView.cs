using DevExpress.XtraGrid;
using System;
using System.Windows.Forms;

namespace Softforyou.CustomerManager.Presentation.Views
{
    public interface ICustomersView
    {
        event EventHandler ViewLoad;
        int CurrentPage { get; set; }
        int PageSize { get; set; }
        int TotalPages { get; set; }
        GridControl GridControlCustomers { get; }
        Label LabelPageInfo { get; }
        Button PreviousPageButton { get; }
        Button NextPageButton { get; }
    }
}
