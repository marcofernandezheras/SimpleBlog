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
    public class PostRange
    {
        public IEnumerable<Post> posts { get; set; }
        public int total { get; set; }
    }

    public class PostController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();

        // GET api/Post
        public PostRange GetPosts(int token, int id)
        {
            var start = token;
            var amount = id;

            var list = db.Post.ToList();
            list.Reverse();
            var result = list.Skip(start * amount).Take(amount).ToList();
            result.ForEach(p => p.UserName = db.User.Find(p.idUser).username);

            return new PostRange()
            {
                posts = result,
                total = list.Count
            };
        }

        // GET api/Post/5
        public Post GetPost(int id)
        {
            Post post = db.Post.Find(id);
            if (post == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            post.UserName = db.User.Where(u => u.id == post.idUser).First().username;
            return post;
        }

        // PUT api/Post/5
        public HttpResponseMessage PutPost(string token, int id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            Token oldToken = db.Token.Where(t => t.token1 == token && t.expires > DateTime.Now).FirstOrDefault();
            if (oldToken != null)
            {
                if (id != post.id)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                Post oldPost = db.Post.Find(id);
                if (oldPost.idUser != oldToken.userId)
                {
                    ModelState.AddModelError("Forbiden", "operation not allowed");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                oldPost.idCategory = post.Category.id;
                oldPost.postContent = post.postContent;
                oldPost.synopsis = post.synopsis;
                oldPost.title = post.title;

                oldToken.expires = DateTime.Now.AddMinutes(15);
                Util.RemoveTokens(db);

                db.Entry(oldToken).State = EntityState.Modified;
                db.Entry(post.Category).State = EntityState.Unchanged;
                db.Entry(oldPost).State = EntityState.Modified;

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

        // POST api/Post
        public HttpResponseMessage PostPost(string id, Post post)
        {
            if (ModelState.IsValid)
            {
                Token token = db.Token.Where(t => t.token1 == id && t.expires > DateTime.Now).FirstOrDefault();
                if (token != null)
                {
                    post.created = DateTime.Now;
                    post.idCategory = post.Category.id;
                    post.idUser = token.userId;
                    token.expires = DateTime.Now.AddMinutes(15);
                    Util.RemoveTokens(db);

                    db.Post.Add(post);
                    db.Entry(token).State = EntityState.Modified;
                    db.Entry(post.Category).State = EntityState.Unchanged;
                    db.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, post);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = post.id }));
                    return response;
                }
                else
                {
                    ModelState.AddModelError("token", "token expired");
                }
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/Post/5
        public HttpResponseMessage DeletePost(string token, int id)
        {
            Token oldToken = db.Token.Where(t => t.token1 == token && t.expires > DateTime.Now).FirstOrDefault();
            if (oldToken != null)
            {
                Post oldPost = db.Post.Find(id);
                if (oldPost == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                if (oldPost.idUser != oldToken.userId)
                {
                    ModelState.AddModelError("Forbiden", "operation not allowed");
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
                }

                db.Post.Remove(oldPost);
                oldToken.expires = DateTime.Now.AddMinutes(15);
                Util.RemoveTokens(db);
                db.Entry(oldToken).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, oldPost);
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