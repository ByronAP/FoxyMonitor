using FoxyMonitor.Contracts.Services;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace FoxyMonitor.Services
{
    public partial class ToastNotificationsService : IToastNotificationsService
    {
        public ToastNotificationsService()
        {
        }

        public void ShowToastNotification(ToastNotification toastNotification)
        {
            ToastNotificationManagerCompat.CreateToastNotifier().Show(toastNotification);
        }
    }
}
