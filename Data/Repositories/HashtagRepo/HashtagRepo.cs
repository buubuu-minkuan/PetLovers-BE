using Data.Entities;
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
    }
}
