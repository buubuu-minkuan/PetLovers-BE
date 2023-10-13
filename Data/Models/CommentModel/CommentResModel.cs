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
        public string? content { get; set; }
        public string? attachment { get; set; }
        public DateTime? createdAt { get; set; }
    }
}
