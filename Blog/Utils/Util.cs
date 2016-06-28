using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Blog.Utils
{
    public class Util
    {
        public static string saltPassword(string pass, string salt)
        {
            var plainText = Encoding.UTF8.GetBytes(pass);
            var saltBytes = Encoding.UTF8.GetBytes(salt);
            HashAlgorithm algorithm = new SHA256Managed();
            byte[] plainTextWithSaltBytes = new byte[plainText.Length + saltBytes.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for (int i = 0; i < saltBytes.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = saltBytes[i];
            }

            return BitConverter.ToString(algorithm.ComputeHash(plainTextWithSaltBytes)).Replace("-", "");
        }

        public static void RemoveTokens(BlogAngularEntities db)
        {
            db.Token.Where(t => t.expires < DateTime.Now).ToList().ForEach(t => db.Token.Remove(t));
        }
    }
}