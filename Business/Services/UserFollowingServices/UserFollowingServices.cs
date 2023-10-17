using Data.Models.UserModel;
using Data.Repositories.UserFollowingRepo;
using Data.Repositories.UserRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.UserFollowingServices
{
    public class UserFollowingService : IUserFollowingService
    {
        private readonly IUserFollowingRepo _repo;
        private readonly IUserRepo _userRepo;
        public UserFollowingService(IUserFollowingRepo repo)
        {
            _userRepo = new UserRepo();
            _repo = repo;
        }
        public async Task FollowUser(UserFollowingModel userFollowing)
        {
            
        }
        public async Task<bool> IsFollowing(Guid userId, Guid followerId)
        {
            return await _repo.IsFollowing(userId, followerId);
        }
        public async Task UnfollowUser(Guid userId, Guid followerId)
        {
            await _repo.UnfollowUser(userId, followerId);
        }
    }
}
