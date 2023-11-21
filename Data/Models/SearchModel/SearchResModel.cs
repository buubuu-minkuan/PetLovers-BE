using System;
using Data.Models.UserModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.HashtagModel;
using Data.Models.PostModel;

namespace Data.Models.SearchModel
{
    public class SearchResModel
    {
        public List<UserSearchModel>? Users { get; set; }
        public List<PostResModel>? Posts { get; set; }
        public List<HashtagTrendingModel>? Hashtag { get; set; }
    }

    public class UserSearchModel
    {
        public Guid Id { get; set; }
        public string? Username { get; set; }
        public string? Avatar { get; set; }
        public string? Fullname { get; set; }
        public bool? IsFollow { get; set; }
    }
}
