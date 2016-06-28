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
using System.Text;
using System.Security.Cryptography;
using Blog.Utils;

namespace Blog.Controllers
{
    public class UserController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();        

        //// GET api/User
        //public IEnumerable<User> GetUsers()
        //{
        //    return db.User.AsEnumerable();
        //}

        //// GET api/User/5
        //public User GetUser(int id)
        //{
        //    User user = db.User.Find(id);
        //    if (user == null)
        //    {
        //        throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
        //    }

        //    return user;
        //}

        // PUT api/User/5
        //public HttpResponseMessage PutUser(int id, User user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        //    }

        //    if (id != user.id)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest);
        //    }

        //    db.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        // POST api/User
        public HttpResponseMessage PostUser(User user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
                    byte[] random = new byte[10];
                    rngCsp.GetBytes(random);
                    var strSalt = BitConverter.ToString(random).Replace("-", "");

                    user.password = Util.saltPassword(user.password, strSalt);
                    user.salt = strSalt;
                    user.created = DateTime.Now;
                    user.avatar = "default.png";
                    user.active = 0;

                    db.User.Add(user);
                    db.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, user);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = user.id }));
                    return response;
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        //// DELETE api/User/5
        //public HttpResponseMessage DeleteUser(int id)
        //{
        //    User user = db.User.Find(id);
        //    if (user == null)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.NotFound);
        //    }

        //    db.User.Remove(user);

        //    try
        //    {
        //        db.SaveChanges();
        //    }
        //    catch (DbUpdateConcurrencyException ex)
        //    {
        //        return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
        //    }

        //    return Request.CreateResponse(HttpStatusCode.OK, user);
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}