//------------------------------------------------------------------------------
// <auto-generated>
//    Este código se generó a partir de una plantilla.
//
//    Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//    Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Blog.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChatMessage
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public System.DateTime Created { get; set; }
        public System.Guid uid { get; set; }
    
        public virtual User User { get; set; }
        public string Avatar { get; set; }
    }
}
