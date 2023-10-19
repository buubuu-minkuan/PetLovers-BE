using Data.Entities;
using Data.Models.UserModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.UserFollowingRepo
{
    public interface IUserFollowingRepo : IRepository<TblUserFollowing>
    {
        public Task<bool> IsFollowing(Guid userId, Guid followerId);

        public Task<TblUserFollowing> GetUserFollow(Guid userId, Guid followerId);

        public Task<List<TblUserFollowing>> GetFollowers(Guid userId);

        public Task<List<TblUserFollowing>> GetFollowing(Guid userId);
    }
}
