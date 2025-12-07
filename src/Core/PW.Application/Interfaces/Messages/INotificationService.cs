using PW.Application.Common.Enums;

namespace PW.Application.Interfaces.Messages
{
    public interface INotificationService
    {
        Task NotificationAsync(NotificationType type, string message, string title = "");
        Task SuccessNotificationAsync(string message, string title = "");
        Task InfoNotificationAsync(string message, string title = "");
        Task WarningNotificationAsync(string message, string title = "");
        Task ErrorNotificationAsync(string message, string title = "");
    }

}
