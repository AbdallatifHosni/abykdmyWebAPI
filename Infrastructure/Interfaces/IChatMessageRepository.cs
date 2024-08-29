using AbyKhedma.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface IChatMessageRepository
    {
        public Task<ChatMessage> Create(ChatMessage message);
        public void Delete(ChatMessage message);
        public void Update(ChatMessage message);
        public IEnumerable<ChatMessage> GetAll();
        public ChatMessage GetById(int Id);
    }
}
