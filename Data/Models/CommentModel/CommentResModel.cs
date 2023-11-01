using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.CommentModel
{
    public class CommentResModel
    { 
        public Guid Id { get; set; }
        public CommentAuthor Author { get; set; }
        public Guid PostId { get; set; }
        public string Type { get; set; }
        public string? content { get; set; }
        public string? attachment { get; set; }
        public DateTime? createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
    }
    public class CommentAuthor
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }
}
