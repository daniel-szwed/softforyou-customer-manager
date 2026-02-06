using System.Windows.Forms;

namespace Domain.Interfaces
{
    public interface IMessageService
    {
        void ShowInfo(string message, string title);
        void ShowWarning(string message, string title);
        void ShowError(string message, string title);
        DialogResult ShowConfirmation(string message, string title);
    }
}
