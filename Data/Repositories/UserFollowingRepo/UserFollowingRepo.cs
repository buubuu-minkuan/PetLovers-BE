using Data.Entities;
using Data.Enums;
using Data.Models.UserModel;
using Data.Repositories.GenericRepository;
using Data.Repositories.PostRepo;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.UserFollowingRepo
{
    public class UserFollowingRepo : Repository<TblUserFollowing>, IUserFollowingRepo
    {
        private readonly PetLoversDbContext _context;

        public UserFollowingRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<bool> IsFollowing(Guid userId, Guid followerId)
        {
            var check = await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId) && x.Status.Equals(Status.ACTIVE)).FirstOrDefaultAsync();
            if (check != null)
            {
                return true;
            }
            return false;
        }

        public async Task<TblUserFollowing> GetUserFollow(Guid userId, Guid followerId)
        {
            return await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId)).FirstOrDefaultAsync();
        }

        public async Task<List<TblUserFollowing>> GetFollowers(Guid userId)
        {
            return await _context.TblUserFollowings.Where(x => x.FollowerId.Equals(userId) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
        }

        public async Task<List<TblUserFollowing>> GetFollowing(Guid userId)
        {
            return await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
        }

    }
}
       
