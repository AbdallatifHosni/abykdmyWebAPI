using Core.Common;

namespace Infrastructure.Interfaces
{
    public interface ISMSService
    {
        Task<SmsMsegatResponseBody> SendAsync(string message, string PhoneNumber);
    }
}
