using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;


namespace Infrastructure.Repositories
{
    public class ChatMessageRepository : IChatMessageRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger _logger;
        public ChatMessageRepository(ILogger<ChatMessage> logger, AppDbContext appDbContext)
        {
            _appDbContext= appDbContext;
            _logger = logger;
        }
        public async Task<ChatMessage> Create(ChatMessage  chatMessage)
        {
            try
            {
                if (chatMessage != null)
                {
                    var obj = _appDbContext.Add<ChatMessage>(chatMessage);
                    await _appDbContext.SaveChangesAsync();
                    return obj.Entity;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Delete(ChatMessage  chatMessage)
        {
            try
            {
                if (chatMessage != null)
                {
                    var obj = _appDbContext.Remove(chatMessage);
                    if (obj != null)
                    {
                        _appDbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public IEnumerable<ChatMessage> GetAll()
        {
            try
            {
                var obj = _appDbContext.ChatMessages.ToList();
                if (obj != null) return obj;
                else return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ChatMessage GetById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    var Obj = _appDbContext.ChatMessages.FirstOrDefault(x => x.Id == Id);
                    if (Obj != null) return Obj;
                    else return null;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Update(ChatMessage chatMessage)
        {
            try
            {
                if (chatMessage != null)
                {
                    var obj = _appDbContext.Update(chatMessage);
                    if (obj != null) _appDbContext.SaveChanges();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
