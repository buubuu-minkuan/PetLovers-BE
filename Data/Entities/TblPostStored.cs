using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblPostStored
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
        public string Status { get; set; } = null!;
        public long CreateAt { get; set; }

        public virtual TblPost Post { get; set; } = null!;
        public virtual TblUser User { get; set; } = null!;
    }
}
