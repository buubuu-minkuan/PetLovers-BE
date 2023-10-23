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

    public class PostDeleteReqModel
    {
        public string token { get; set; }
        public Guid postId { get; set; }
    }

    public class PostStoreReqModel
    {
        public string token { get; set; }
        public Guid postId { get; set; }
    }

    public class PostTradeCreateReqModel
    {
        public string Token { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string[]? Attachment { get; set; }
        public string? Name { get; set; }
        public string Type { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Age { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public decimal Weight { get; set; }
    }
}
