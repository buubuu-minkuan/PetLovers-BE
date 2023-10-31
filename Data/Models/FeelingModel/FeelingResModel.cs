using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.FeelingModel
{
    public class FeelingResModel
    {
        public Guid Id { get; set; }
        public FeelingAuthorModel Author { get; set; }
        public Guid postId { get; set; }
        public string Type { get; set; }
        public DateTime createdAt { get; set; }

    }

    public class FeelingListResModel
    {
        public Guid Id { get; set; }
        public FeelingAuthorModel Author { get; set; }
        public string Type { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class FeelingAuthorModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
