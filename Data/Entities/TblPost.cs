using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblPost
    {
        public TblPost()
        {
            TblNotifications = new HashSet<TblNotification>();
            TblPostReactions = new HashSet<TblPostReaction>();
            TblPostStoreds = new HashSet<TblPostStored>();
            TblReports = new HashSet<TblReport>();
            TblTradeRequests = new HashSet<TblTradeRequest>();
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string Attachment { get; set; } = null!;
        public string Hashtag { get; set; } = null!;
        public decimal? Amount { get; set; }
        public string Status { get; set; } = null!;
        public bool IsProcessed { get; set; }
        public bool? IsFree { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual TblUser User { get; set; } = null!;
        public virtual ICollection<TblNotification> TblNotifications { get; set; }
        public virtual ICollection<TblPostReaction> TblPostReactions { get; set; }
        public virtual ICollection<TblPostStored> TblPostStoreds { get; set; }
        public virtual ICollection<TblReport> TblReports { get; set; }
        public virtual ICollection<TblTradeRequest> TblTradeRequests { get; set; }
    }
}
