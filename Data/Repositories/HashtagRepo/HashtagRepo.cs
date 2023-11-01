using Data.Entities;
using Data.Models.HashtagModel;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.HashtagRepo
{
    public class HashtagRepo : Repository<TblPostHashtag>, IHashtagRepo
    {
        private readonly PetLoversDbContext _context;

        public HashtagRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<TblPostHashtag>> GetListHashTagByPostId(Guid postId)
        {
            return await _context.TblPostHashtags.Where(x => x.PostId.Equals(postId)).ToListAsync();
        }

        public async Task<List<Guid>> GetListPostIdByHashtag(string hashtag)
        {
            var postsId = await _context.TblPostHashtags.Where(x => x.Hashtag.Equals(hashtag)).ToListAsync();
            var resListPostsId = new List<Guid>();
            foreach (var post in postsId)
            {
                if (!resListPostsId.Contains(post.Id))
                {
                    resListPostsId.Add(post.Id);
                }
            }
            return resListPostsId;
        }

        public async Task<List<HashtagTrendingModel>> GetListHashtagTrending()
        {
            var postHashtag = await _context.TblPostHashtags.ToListAsync();
            var groups = postHashtag.GroupBy(p => p.Hashtag).Select(g => new { Hashtag = g.Key, Count = g.Count() }).OrderByDescending(g => g.Count).Take(3);
            List<HashtagTrendingModel> listRes = new();
            foreach (var group in groups)
            {
                listRes.Add(new HashtagTrendingModel()
                {
                    Hashtag = group.Hashtag,
                    Count = group.Count
                });
            }
            return listRes;
        }
    }
}
