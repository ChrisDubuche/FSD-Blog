using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FSD_Blog.Models
{
    public class Post
    { 
        public int Id { get; set; }
        public string Title { get; set; }

        //allowing html to display in the body
       [AllowHtml]
        public string Body { get; set; }
        public string Abstract { get; set; }

        public string Slug { get; set; }
        public string MediaURL { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }       
        public bool Published { get; set; }

        //Navigational props
        public virtual ICollection<Comment> Comments { get; set; }

        public Post()
        {
            this.Comments = new HashSet<Comment>();
        }
    }
}