using FoxyMonitor.Models;
using Windows.UI.Notifications;

namespace FoxyMonitor.Contracts.Services
{
    public interface IToastNotificationsService
    {
        public abstract void ShowToastNotification(ToastNotification toastNotification);

        public abstract void ShowToastNotification(string tag, string title, string body, ToastAction action, params string[] args);
    }
}
