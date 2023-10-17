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
        public Guid userId { get; set; }
        public string content { get; set; }
        public string[] attachment { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime? updatedAt { get; set; }
        public int amountComment { get; set; }
        public int amountFeeling { get; set; }
    }
}
