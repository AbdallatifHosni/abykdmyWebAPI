using AbyKhedma.Core.Common;
using AbyKhedma.Entities;
using AbyKhedma.Persistance;
using AutoMapper;
using Core.Dtos;
using Core.Models;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AbyKhedma.Services
{
    public class ChatMessageService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public ChatMessageService(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public List<ChatMessageModel> GetChatMessageList(List<int> ChatMessageIdList)
        {
            var chatMessages = _dbContext.ChatMessages.Include(el=>el.Messages).Include(c => c.Answer).Where(c => ChatMessageIdList.Contains(c.RequestFlowId)).ToList();
            return _mapper.Map<List<ChatMessageModel>>(chatMessages);
        }
        public ChatMessageModel GetChatMessageById(int messageId)
        {
            var chatMessages = _dbContext.ChatMessages.Include(el => el.Messages).Include(c => c.Answer).FirstOrDefault(c => c.Id == messageId);
            return _mapper.Map<ChatMessageModel>(chatMessages);
        }
        public ChatMessage AddChatMessage(ChatMessageToCreateDto chatMessage)
        {
            if (RequestFlowExist(chatMessage.RequestFlowId))
            {
                return null;
            }
            var chatMessageToCreate = _mapper.Map<ChatMessage>(chatMessage);
            _dbContext.ChatMessages.Add(chatMessageToCreate);
            _dbContext.SaveChanges();
            return chatMessageToCreate;
        }

        private bool RequestFlowExist(int requestFlowId)
        {
            return _dbContext.ChatMessages.FirstOrDefault(c=>c.RequestFlowId==requestFlowId)!=null;
        }

        public ChatMessage UpdateChatMessage(ChatMessageToCreateDto chatMessage)
        {
            
            var dbMessage = _dbContext.ChatMessages.FirstOrDefault(c => c.RequestFlowId == chatMessage.RequestFlowId);
            if (dbMessage != null)
            {
                dbMessage.AnswerId = chatMessage.AnswerId;
                if(chatMessage!=null && chatMessage.Messages != null)
                {
                    dbMessage.Messages = _mapper.Map<List<Message>>(chatMessage.Messages);
                }
            }
            _dbContext.ChatMessages.Update(dbMessage);
            _dbContext.SaveChanges();
            return dbMessage;
        }
    }
}
