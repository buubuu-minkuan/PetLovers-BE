using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.PostModel
{
    public class PostReqModel
    {
        public String? content { get; set; }
        public String? attachment { get; set; }
    }

    public class PostCreateReqModel
    {
        public string token { get; set; } = null!;
        public string content { get; set; }
        public string[]? attachment { get; set; }
    }

    public class PostUpdateReqModel
    {
        public string token { get; set; } = null!;
        public Guid postId { get; set; }
        public string? content { get; set; }
        public string[]? attachment { get; set; }
    }
}
