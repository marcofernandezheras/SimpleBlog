using Blog.Models;
using Blog.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Blog.Controllers
{
    public class Response
    {
        public bool valid { get; set; }
        public string token { get; set; }
        public int idUser { get; set; }
        public string userName { get; set; }
        public string avatar { get; set; }
        public DateTime expires { get; set; }
    }

    public class LogController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();

        // POST api/log
        public Response Post(User user)
        {
            var response = new Response()
            {
                valid = false
            };

            var oldUser = db.User.Where(u => u.username == user.username).First();
            if (oldUser != null && oldUser.active != 0)
            {
                var triedPass = Util.saltPassword(user.password, oldUser.salt);
                if (triedPass == oldUser.password)
                {
                    db.Token.Where(t => t.userId == oldUser.id || t.expires < DateTime.Now).ToList().ForEach(t => db.Token.Remove(t));
                    Token token = new Token()
                    {
                        expires = DateTime.Now.AddMinutes(15),
                        token1 = Guid.NewGuid().ToString(),
                        userId = oldUser.id
                    };
                    db.Token.Add(token);
                    db.SaveChanges();

                    response.valid = true;
                    response.token = token.token1;
                    response.expires = token.expires;
                    response.idUser = token.userId;
                    response.userName = oldUser.username;
                    response.avatar = oldUser.avatar;
                    return response;
                }
            }
            return response;
        }


        // DELETE api/log/5
        public Response Delete(string id)
        {
            var response = new Response();
            db.Token.Where(t => t.token1 == id || t.expires < DateTime.Now).ToList().ForEach(t => db.Token.Remove(t));
            db.SaveChanges();
            return response;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
