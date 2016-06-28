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
using System.Security.Cryptography;

namespace Blog.Controllers
{
    public class NewPassword
    {
        public string oldPassword { get; set; }
        public string password { get; set; }
    }

    public class PasswordController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();


        // PUT api/Password/5
        public HttpResponseMessage PutPassword(string id, NewPassword pass)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }           

             Token token = db.Token.Where(t => t.token1 == id && t.expires > DateTime.Now).FirstOrDefault();
             if (token != null)
             {          

                 var user = db.User.Find(token.userId);
                 var salted = Util.saltPassword(pass.oldPassword, user.salt);

                 if (salted != user.password)
                 {
                     db.Token.Remove(token);
                     db.SaveChanges();
                     ModelState.AddModelError("token", "token expired");
                     return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                 }

                 RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                 byte[] random = new byte[10];
                 rngCsp.GetBytes(random);
                 var strSalt = BitConverter.ToString(random).Replace("-", "");

                 user.password = Util.saltPassword(pass.password, strSalt);
                 user.salt = strSalt;
                 
                 Util.RemoveTokens(db);
                 db.Token.Remove(token);
                 db.Entry(user).State = EntityState.Modified;

                 try
                 {
                     db.SaveChanges();
                 }
                 catch (DbUpdateConcurrencyException ex)
                 {
                     return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                 }
                 return Request.CreateResponse(HttpStatusCode.OK);
             }
             else
             {
                 ModelState.AddModelError("token", "token expired");
                 return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
             }            
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}