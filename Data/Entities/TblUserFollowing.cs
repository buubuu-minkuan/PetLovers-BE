using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblUserFollowing
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid FollowerId { get; set; }
        public string Status { get; set; } = null!;

        public virtual TblUser Follower { get; set; } = null!;
        public virtual TblUser User { get; set; } = null!;
    }
}
