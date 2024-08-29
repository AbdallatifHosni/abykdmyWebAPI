using Core.Models;

namespace Infrastructure.Interfaces
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
    }
}
