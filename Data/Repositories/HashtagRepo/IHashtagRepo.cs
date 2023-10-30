using Data.Entities;
using Data.Models.HashtagModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.HashtagRepo
{
    public interface IHashtagRepo : IRepository<TblPostHashtag>
    {
        public Task<List<TblPostHashtag>> GetListHashTagByPostId(Guid postId);
        public Task<List<Guid>> GetListPostIdByHashtag(string hashtag);
        public Task<List<HashtagTrendingModel>> GetListHashtagTrending();
    }
}
