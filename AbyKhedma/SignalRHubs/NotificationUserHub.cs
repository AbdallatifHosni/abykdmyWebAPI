using AbyKhedma.Entities;
using AbyKhedma.Interfaces;
using AbyKhedma.Persistance;
using Core.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.AspNetCore.Mvc;

namespace AbyKhedma.SignalRHubs
{
    public class NotificationUserHub : Hub
    {
        private readonly AppDbContext _appDbContext;

        public NotificationUserHub(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        static IList<UserConnection> Users = new List<UserConnection>();
        public class UserConnection
        {
            public int UserId { get; set; }
            public string ConnectionId { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
        }
        public async Task PublishUserOnConnect(int id, string fullname, string username)
        {
            var existingUser = Users.FirstOrDefault(x => x.Username == username);
            var indexExistingUser = Users.IndexOf(existingUser);
            UserConnection user = new UserConnection
            {
                UserId = id,
                ConnectionId = Context.ConnectionId,
                FullName = fullname,
                Username = username
            };

            if (!Users.Contains(existingUser))
            {
                Users.Add(user);
            }
            else
            {
                Users[indexExistingUser] = user;
            }
            await Clients.All.SendAsync("BroadcastUserOnConnect", Users);
        }

        public void RemoveOnlineUser(int userID)
        {
            var user = Users.Where(x => x.UserId == userID).FirstOrDefault();
            if (user != null)
            {
                Users.Remove(user);
            }
            Clients.All.SendAsync("BroadcastUserOnDisconnect", Users);
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var user = Users.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (user != null)
            {
                Users.Remove(user);
            }
            Clients.All.SendAsync("BroadcastUserOnDisconnect", Users);
        }
        public async Task IsUserOnline(int userId)
        {
            var user = Users.Where(x => x.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                await Clients.All.SendAsync("ReceiveMessage", false);
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", true);
            }
        }

        // Send message to all online users
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
        public async Task ChatUpdate(List<ChatMessageModel> chatMessages)
        {
            foreach (var chatMessage in chatMessages)
            {
                if (chatMessage != null && chatMessage.ToUserId > 0)
                {
                    var existingUser = Users.FirstOrDefault(x => x.UserId == chatMessage.ToUserId);
                    if (existingUser != null)
                    {
                        await Clients.Client(existingUser.ConnectionId).SendAsync("ChatUpdate", chatMessage);
                    }
                    else/*offline and send notification*/
                    {
                        await _appDbContext.UserNotifications.AddAsync(new UserNotification() { UserId = chatMessage.ToUserId.Value, Message = "تحديث على الطلب الخاص بكم" });
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
        }
        //public async Task  UserNotifications()
        //{
        //    var identityId = Context.User.Identity.Name;
        //    if (identityId == null)
        //    {
        //        return;
        //    }
        //    var notifications = _appDbContext.UserNotifications.Where(x => x.UserId.ToString() == identityId && x.IsRead == true).ToList();
        //    notifications.ForEach(x =>
        //    {
        //        x.IsRead = true;
        //    });
        //    _appDbContext.UserNotifications.UpdateRange(notifications);
        //    await _appDbContext.SaveChangesAsync();
        //    await Clients.Client(identityId).SendAsync("UserNotifications", notifications);
        //}
    }
}
