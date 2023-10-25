using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.PostAttachmentModel
{
    public class PostAttachmentResModel
    {
        public Guid Id { get; set; }
        public string? Attachment { get; set; }
        public string? Status { get; set; }
    }
}
