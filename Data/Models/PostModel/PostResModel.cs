using Data.Entities;
using Data.Models.FeelingModel;
using Data.Models.PostAttachmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.PostModel
{
    public class PostResModel
    {
        public Guid Id { get; set; }
        public PostAuthorModel author { get; set; }
        public string content { get; set; }
        public List<PostAttachmentResModel> attachment { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public bool isFeeling { get; set; }
        public bool isAuthor { get; set; } = false;
        public int amountComment { get; set; }
        public int amountFeeling { get; set; }
    }

    public class PostFeelingResModel
    {
        public Guid Id { get; set; }
        public List<FeelingListResModel>? feeling { get; set; }
        public string Type { get; set; }
    }

    public class PostTradeResModel
    {
        public Guid Id { get; set; }
        public PostAuthorModel Author { get; set; }
        public bool? isFree { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<PostAttachmentResModel> Attachment { get; set; }
        public PetPostTradeModel Pet { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public bool isTrading { get; set; } = false;
        public bool isRequest { get; set; } = false;
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        public PostTradeUserRequestModel? UserRequest { get; set; }
        public string Address { get; set; }
    }

    public class PostTradeAuthorResModel
    {
        public Guid Id { get; set; }
        public PostAuthorModel Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<PostAttachmentResModel> Attachment { get; set; }
        public PetPostTradeModel Pet { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        public List<PostTradeUserRequestModel>? UserRequest { get; set; }
        public bool? isFree { get; set; }
        public bool isTrading { get; set; } = false;
        public string Address { get; set; }
    }

    public class PostTradeUserRequestModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string Status { get; set; }
        public int SocialCredit { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class PetPostTradeModel
    {
        public string? Name { get; set; }
        public string Type { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Age { get; set; } = null!;
        public string Gender { get; set; } = null!; 
        public string Color { get; set; } = null!;
        public decimal Weight { get; set; }
    }

    public class PostAuthorModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Phone { get; set; }
    }
    public class PostTradeTitleModel
    {
        public Guid Id { get; set; }
        public PostAuthorModel Author { get; set; }
        public List<PostAttachmentResModel> Attachment { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public bool? isFree { get; set; }
    }

    public class ReportResModel
    {
        public Guid Id { get; set;}
        public Guid userId { get; set; }
        public Guid? postId { get; set; }
        public Guid? commentId { get; set; }
        public string type { get; set; }
        public string reason { get; set; }
        public DateTime createdAt { get; set; }
    }
    public class GetAllPostTradeTitleResModel
    {
        public Guid Id { get; set; }
        public PostAuthorModel Author { get; set; }
        public string Title { get; set; }
        public List<PostAttachmentResModel> Attachment { get; set; }
        public string Type { get; set; }
        public decimal? Amount { get; set; }
    }
}
