using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblPostAttachment
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string? Attachment { get; set; }

        public virtual TblPost Post { get; set; } = null!;
    }
}
