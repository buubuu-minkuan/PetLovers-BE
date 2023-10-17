using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.CommentModel
{
    public class CommentReqModel
    {
        public String? content { get; set; }
        public String? attachment { get; set; }
    }

    public class CommentCreateReqModel
    {
        public string token { get; set; } = null!;
        public Guid postId { get; set; }
        public string content { get; set; } = null!;
        public string attachment { get; set; } = null!;
    }
}
