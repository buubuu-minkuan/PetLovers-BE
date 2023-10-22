using Data.Entities;
using Data.Models.PostAttachmentModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public int amountComment { get; set; }
        public int amountFeeling { get; set; }
    }

    public class PostAuthorModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
