using FoxyMonitor.Contracts.Services;
using FoxyMonitor.Models;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace FoxyMonitor.Services
{
    public class ToastNotificationsService : IToastNotificationsService
    {
        public ToastNotificationsService()
        {
        }

        public void ShowToastNotification(ToastNotification toastNotification)
        {
            ToastNotificationManagerCompat.CreateToastNotifier().Show(toastNotification);
        }

        public void ShowToastNotification(string tag, string title, string body, ToastAction action, params string[] args)
        {
            // Create the toast content
            var content = new ToastContent()
            {
                // More about the Launch property at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastcontent
                Launch = "ToastContentActivationParams",
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveText()
                            {
                                 Text = body
                            }
                        }
                    }
                },

                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        // More about Toast Buttons at https://docs.microsoft.com/dotnet/api/microsoft.toolkit.uwp.notifications.toastbutton
                        new ToastButton("OK", $"\"{action}\":\"{string.Join(',', args)}\"")
                        {
                            ActivationType = ToastActivationType.Foreground
                        },

                        new ToastButtonDismiss("Cancel")
                    }
                }
            };

            // Add the content to the toast
            var doc = new XmlDocument();
            doc.LoadXml(content.GetContent());
            var toast = new ToastNotification(doc)
            {
                // More details at https://docs.microsoft.com/uwp/api/windows.ui.notifications.toastnotification.tag
                Tag = tag
            };

            // And show the toast
            ShowToastNotification(toast);
        }
    }
}
