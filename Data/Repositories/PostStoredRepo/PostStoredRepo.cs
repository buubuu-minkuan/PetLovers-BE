using Data.Entities;
using Data.Enums;
using Data.Repositories.GenericRepository;
using Data.Repositories.OTPRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostStoredRepo
{
    public class PostStoredRepo : Repository<TblPostStored>, IPostStoredRepo
    {
        private readonly PetLoversDbContext _context;
        public PostStoredRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public async Task<TblPostStored> GetStoredPost(Guid userId, Guid postId)
        {
            return await _context.TblPostStoreds.Where(x => x.UserId.Equals(userId) && x.PostId.Equals(postId) && x.Status.Equals(Status.ACTIVE)).FirstOrDefaultAsync();
        }
    }
}
