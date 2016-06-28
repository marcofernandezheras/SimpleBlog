using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Blog.Models;
using Blog.Utils;

namespace Blog.Controllers
{
    public class ChatMessageController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();

        // GET api/ChatMessage
        public IEnumerable<ChatMessage> GetChatMessages(string id)
        {
            Token token = db.Token.Where(t => t.token1 == id && t.expires > DateTime.Now).FirstOrDefault();
            if (token != null)
            {
                ChatHub.storeOldChat(db, DateTime.Now);
                token.expires = DateTime.Now.AddMinutes(15);
                Util.RemoveTokens(db);
                db.Entry(token).State = EntityState.Modified;
                db.SaveChanges();
                var chatmessages = db.ChatMessage.Include(c => c.User).OrderBy(c => c.Created).ToList();
                return chatmessages.Select(c => new ChatMessage(){
                    Avatar = c.User.avatar,
                    Message = c.Message,
                    Username = c.User.username,
                    Hour = c.Created.ToString("h:mm:ss")
                });
            }
            return new List<ChatMessage>();
        }       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}