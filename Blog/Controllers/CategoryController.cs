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
    public class CategoryController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();

        // GET api/Category
        public IEnumerable<Category> GetCategories()
        {
            return db.Category.AsEnumerable();
        }

        // GET api/Category/5
        public Category GetCategory(int id)
        {
            Category category = db.Category.Find(id);
            if (category == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return category;
        }

        // PUT api/Category/5
        public HttpResponseMessage PutCategory(string token, int id, Category category)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            Token oldToken = db.Token.Where(t => t.token1 == token && t.expires > DateTime.Now).FirstOrDefault();
            if (oldToken != null)
            {
                if (id != category.id)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest);
                }

                oldToken.expires = DateTime.Now.AddMinutes(15);
                Util.RemoveTokens(db);
                db.Entry(oldToken).State = EntityState.Modified;
                db.Entry(category).State = EntityState.Modified;

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

        // POST api/Category
        public HttpResponseMessage PostCategory(string id, Category category)
        {
            if (ModelState.IsValid)
            {
                Token token = db.Token.Where(t => t.token1 == id && t.expires > DateTime.Now).FirstOrDefault();
                if (token != null)
                {
                    token.expires = DateTime.Now.AddMinutes(15);
                    Util.RemoveTokens(db);
                    db.Entry(token).State = EntityState.Modified;
                    db.Category.Add(category);
                    db.SaveChanges();

                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created, category);
                    response.Headers.Location = new Uri(Url.Link("DefaultApi", new { id = category.id }));
                    return response;
                }
                else
                {
                    ModelState.AddModelError("token", "token expired");
                }  
            }
            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
        }

        // DELETE api/Category/5
        public HttpResponseMessage DeleteCategory(string token, int id)
        {
            Token oldToken = db.Token.Where(t => t.token1 == token && t.expires > DateTime.Now).FirstOrDefault();
            if (oldToken != null)
            {
                Category category = db.Category.Find(id);
                if (category == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                oldToken.expires = DateTime.Now.AddMinutes(15);
                Util.RemoveTokens(db);
                db.Entry(oldToken).State = EntityState.Modified;
                db.Category.Remove(category);

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, ex);
                }

                return Request.CreateResponse(HttpStatusCode.OK, category);
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