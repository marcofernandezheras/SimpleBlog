using Blog.Models;
using Blog.Utils;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Blog.Controllers
{

    public class ChatMessage
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string Hour { get; set; }
        public string Avatar { get; set; }
    }

    public class EnteredMessage
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
    }

    public class ChatUser
    {
        public string Id { get; set; }
        public User user { get; set; }
        public Token token { get; set; }
    }

    public class ChatHub : Hub
    {
        private static readonly List<ChatUser> users = new List<ChatUser>();

        public void enterChat(string token)
        {
            try
            {
                using (BlogAngularEntities db = new BlogAngularEntities())
                {
                    var oldToken = db.Token.Find(token);
                    if (oldToken != null)
                    {
                        var user = db.User.Find(oldToken.userId);
                        oldToken.expires = DateTime.Now.AddMinutes(35);
                        Util.RemoveTokens(db);

                        Clients.Others.userEnter(new EnteredMessage()
                        {
                            Username = user.username,
                            Avatar = user.avatar
                        });

                        users.ForEach(u => Clients.Caller.userEnter(new EnteredMessage()
                        {
                            Username = u.user.username,
                            Avatar = u.user.avatar
                        }));

                        users.Add(new ChatUser() { 
                            user = user,
                            token = oldToken,
                            Id = Context.ConnectionId
                        });
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public void Send(string token, string message)
        {
            try
            {
                using (BlogAngularEntities db = new BlogAngularEntities())
                {
                    var oldToken = db.Token.Find(token);
                    if (oldToken != null)
                    {
                        var user = db.User.Find(oldToken.userId);
                        oldToken.expires = DateTime.Now.AddMinutes(35);
                        Util.RemoveTokens(db);

                        storeMessage(db, user, message);

                        // Call the broadcastMessage method to update clients.
                        Clients.All.broadcastMessage(new ChatMessage()
                        {
                            Username = user.username,
                            Message = message,
                            Hour = DateTime.Now.ToString("h:mm:ss"),
                            Avatar = user.avatar,
                        });

                        db.SaveChanges();
                    }
                    else
                    {

                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private void storeMessage(BlogAngularEntities db, User user, string message)
        {
            var now = DateTime.Now;
            var chat = new Models.ChatMessage()
            {
                uid = Guid.NewGuid(),
                Created = now,
                User = user,
                UserId = user.id,
                Message = message.Length <= 150 ? message : message.Substring(0, 150)
            };
            db.ChatMessage.Add(chat);

            db.Entry(user).State = EntityState.Unchanged;
            db.Entry(chat).State = EntityState.Added;

            storeOldChat(db, now);
        }

        public static void storeOldChat(BlogAngularEntities db, DateTime now)
        {
            var yesterdayChats = db.ChatMessage.ToList().Where(c => c.Created.Year < now.Year
                                                        || c.Created.DayOfYear < now.DayOfYear)
                                                        .OrderBy(c => c.Created).ToList();

            if (yesterdayChats.Count > 0)
            {
                yesterdayChats.ForEach(c => db.ChatMessage.Remove(c));
                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(root + "/" + now.AddDays(-1).ToString("yyyy_MM_dd") + ".txt"))
                {
                    foreach (var item in yesterdayChats)
                    {
                        var itemUser = db.User.Find(item.UserId);
                        file.WriteLine(JsonConvert.SerializeObject(new ChatMessage
                        {
                            Message = item.Message,
                            Username = itemUser.username,
                            Avatar = itemUser.avatar,
                            Hour = item.Created.ToString("h:mm:ss")
                        }));
                    }
                }
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            ChatUser user = users.Find(u => u.Id == Context.ConnectionId);
            users.Remove(user);
            Clients.Others.userLeft(new EnteredMessage() { 
                Username = user.user.username
            });
            return base.OnDisconnected(stopCalled);
        }
    }
}