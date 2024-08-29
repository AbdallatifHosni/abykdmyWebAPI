using AbyKhedma.Entities;

namespace AbyKhedma.Interfaces
{
    public interface IAccountService
    {
        public Task<bool> UserExist(string phoneNumber);
        public Task Register(User user, string password);
    }
}
