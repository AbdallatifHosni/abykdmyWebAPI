using Core.Common;

namespace Infrastructure.Interfaces
{
    public interface IWhatsAppService
    {
        Task<WhatsAppResponseBody> SendAsync(string message, string PhoneNumber);
    }
}
