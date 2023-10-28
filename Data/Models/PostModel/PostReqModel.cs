using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.PostModel
{
    public class PostReqModel
    {
        public string token { get; set; }
        public Guid postId { get; set; }
    }

    public class PostCreateReqModel
    {
        public string token { get; set; } = null!;
        public string content { get; set; }
        public string[]? attachment { get; set; }
        public string[]? hashtag { get; set; }
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
        public string? PetName { get; set; }
        public string Type { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Age { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public decimal Weight { get; set; }
        public string Color { get; set; } = null!;
        public decimal? Amount { get; set; }
    }
    public class PostTradeUpdateReqModel
    {
        public string token { get; set; } = null!;
        public string Title { get; set; }
        public Guid postId { get; set; }
        public string? content { get; set; }
        public string[]? attachment { get; set; }
        public string? PetName { get; set; }
        public string Type { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Age { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public decimal Weight { get; set; }
        public string Color { get; set; } = null!;
        public decimal? Amount { get; set; }
    }

    public class PostReportModel
    {
        public string token { get; set; }
        public Guid postId { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; } = null!;
    }
}
