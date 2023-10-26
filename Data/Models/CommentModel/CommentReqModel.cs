using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.CommentModel
{
    public class CommentReqModel
    {
        public string token { get; set; } = null!;
        public Guid Id { get; set; }
        public string? content { get; set; }
        public string? attachment { get; set; }
    }

    public class CommentCreateReqModel
    {
        public string token { get; set; } = null!;
        public Guid postId { get; set; }
        public string? content { get; set; }
        public string? attachment { get; set; }
    }

    public class CommentDeleteReqModel
    {
        public string token { get; set; }
        public Guid Id { get; set; }
    }
}
