using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblNotifications = new HashSet<TblNotification>();
            TblPostReactions = new HashSet<TblPostReaction>();
            TblPostStoreds = new HashSet<TblPostStored>();
            TblPosts = new HashSet<TblPost>();
            TblReports = new HashSet<TblReport>();
            TblTradeRequests = new HashSet<TblTradeRequest>();
            TblUserFollowingFollowers = new HashSet<TblUserFollowing>();
            TblUserFollowingUsers = new HashSet<TblUserFollowing>();
            TblUserRewards = new HashSet<TblUserReward>();
            TblUserTimeoutModerators = new HashSet<TblUserTimeout>();
            TblUserTimeoutUsers = new HashSet<TblUserTimeout>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public byte[] Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public Guid RoleId { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual TblRole Role { get; set; } = null!;
        public virtual ICollection<TblNotification> TblNotifications { get; set; }
        public virtual ICollection<TblPostReaction> TblPostReactions { get; set; }
        public virtual ICollection<TblPostStored> TblPostStoreds { get; set; }
        public virtual ICollection<TblPost> TblPosts { get; set; }
        public virtual ICollection<TblReport> TblReports { get; set; }
        public virtual ICollection<TblTradeRequest> TblTradeRequests { get; set; }
        public virtual ICollection<TblUserFollowing> TblUserFollowingFollowers { get; set; }
        public virtual ICollection<TblUserFollowing> TblUserFollowingUsers { get; set; }
        public virtual ICollection<TblUserReward> TblUserRewards { get; set; }
        public virtual ICollection<TblUserTimeout> TblUserTimeoutModerators { get; set; }
        public virtual ICollection<TblUserTimeout> TblUserTimeoutUsers { get; set; }
    }
}
