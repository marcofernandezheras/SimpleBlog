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

namespace Blog.Controllers
{
    public class CommentController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();



        // GET api/Comment/
        public IEnumerable<Comment> GetComment(int id)
        {
            var list = db.Comment.Where(c => c.idPost == id).ToList();
            list.ForEach(c => {
                var u = db.User.Find(c.idUser);
                c.UserName = u.username;
                c.UserAvatar = u.avatar;
            });
            return list;
        }

        // PUT api/Comment/5
        public HttpResponseMessage PutComment(string token, int id, Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != comment.id)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Token oldToken = db.Token.Where(t => t.token1 == token && t.expires > DateTime.Now).FirstOrDefault();
            if (oldToken != null)
            {
                Comment oldComment = db.Comment.Find(id);
                if (oldComment.idUser != oldToken.userId)
                {
                    ModelState.AddModelError("Forbiden", "operation not allowed");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                oldComment.comment1 = comment.comment1;

                oldToken.expires = DateTime.Now.AddMinutes(15);
                db.Entry(oldToken).State = EntityState.Modified;
                Utils.Util.RemoveTokens(db);
                

                db.Entry(oldComment).State = EntityState.Modified;
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

        // POST api/Comment
        public HttpResponseMessage PostComment(string id, Comment comment)
        {
            if (ModelState.IsValid)
            {
                Token token = db.Token.Where(t => t.token1 == id && t.expires > DateTime.Now).FirstOrDefault();
                if (token != null)
                {
                    comment.idUser = token.userId;
                    comment.UserName = db.User.Find(token.userId).username;

                    db.Comment.Add(comment);
                    Utils.Util.RemoveTokens(db);
                    token.expires = DateTime.Now.AddMinutes(15);
                    db.Entry(token).State = EntityState.Modified;
                    db.SaveChanges();

                    comment.UserAvatar = db.User.Find(token.userId).avatar;

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, comment);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = comment.id }));
                    return response;
                }
                else
                {
                    ModelState.AddModelError("token", "token expired");
                }               
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/Comment/5
        public HttpResponseMessage DeleteComment(string token, int id)
        {
            Comment comment = db.Comment.Find(id);
            if (comment == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            Token oldToken = db.Token.Where(t => t.token1 == token && t.expires > DateTime.Now).FirstOrDefault();
            if (oldToken != null)
            {
                if (comment.idUser != oldToken.userId)
                {
                    ModelState.AddModelError("Forbiden", "operation not allowed");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                Utils.Util.RemoveTokens(db);
                oldToken.expires = DateTime.Now.AddMinutes(15);
                db.Entry(oldToken).State = EntityState.Modified;
                db.Comment.Remove(comment);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, comment);
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