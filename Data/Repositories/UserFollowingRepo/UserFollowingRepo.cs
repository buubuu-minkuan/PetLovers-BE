using Data.Entities;
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
        public async Task FollowUser(Guid userId, Guid followerId)
        {
            var check = await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId)).FirstOrDefaultAsync();
            if (check == null)
            {
                var userFollowing = new TblUserFollowing()
                {
                    UserId = userId,
                    FollowerId = followerId
                };
                await _context.TblUserFollowings.AddAsync(userFollowing);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> IsFollowing(Guid userId, Guid followerId)
        {
            var check = await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId)).FirstOrDefaultAsync();
            if (check != null)
            {
                return true;
            }
            return false;
        }
        public async Task UnfollowUser(Guid userId, Guid followerId)
        {
            var check = await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.FollowerId.Equals(followerId)).FirstOrDefaultAsync();
            if (check != null)
            {
                _context.TblUserFollowings.Remove(check);
                await _context.SaveChangesAsync();
            }
        }

    }
}
       
