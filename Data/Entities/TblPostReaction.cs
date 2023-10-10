using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblPostReaction
    {
        public TblPostReaction()
        {
            TblReports = new HashSet<TblReport>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Type { get; set; } = null!;
        public string? Content { get; set; }
        public string? Attachment { get; set; }
        public string? TypeReact { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual TblPost Post { get; set; } = null!;
        public virtual TblUser User { get; set; } = null!;
        public virtual ICollection<TblReport> TblReports { get; set; }
    }
}
