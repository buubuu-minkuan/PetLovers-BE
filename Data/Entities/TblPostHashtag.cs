using System;
using System.Collections.Generic;

namespace Data.Entities
{
    public partial class TblPostHashtag
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string? Hashtag { get; set; }
    }
}
