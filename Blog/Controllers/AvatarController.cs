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
using System.Threading.Tasks;
using System.IO;
using Blog.Utils;

namespace Blog.Controllers
{
    public class AvatarController : ApiController
    {
        private BlogAngularEntities db = new BlogAngularEntities();

        public async Task<HttpResponseMessage> PostUpload(string id)
        {
            Token token = db.Token.Where(t => t.token1 == id && t.expires > DateTime.Now).FirstOrDefault();
            if (token != null)
            {
                // Check if the request contains multipart/form-data.
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                token.expires = DateTime.Now.AddMinutes(15);
                Util.RemoveTokens(db);
                db.Entry(token).State = EntityState.Modified;

                User user = db.User.Find(token.userId);

                string root = HttpContext.Current.Server.MapPath("~/cliente/app/images/avatars");
                var provider = new MultipartFormDataStreamProvider(root);

                try
                {
                    // Read the form data.
                    await Request.Content.ReadAsMultipartAsync(provider);


                    // This illustrates how to get the file names.
                    foreach (MultipartFileData fileData in provider.FileData)
                    {
                        if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                        {
                            return Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                        }
                        string fileName = fileData.Headers.ContentDisposition.FileName;
                        if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                        {
                            fileName = fileName.Trim('"');
                        }
                        if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                        {
                            fileName = Path.GetFileName(fileName);
                        }
                        string path = Path.Combine(root, user.username + fileName.Substring(fileName.LastIndexOf('.')));
                        File.Delete(path);
                        File.Move(fileData.LocalFileName, path);

                        user.avatar = user.username + fileName.Substring(fileName.LastIndexOf('.'));
                        db.SaveChanges();
                    }

                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                catch (System.Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }
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