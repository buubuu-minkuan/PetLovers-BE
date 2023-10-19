using Data.Entities;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.UserFollowingServices
{
    public interface IUserFollowingServices
    {
        public Task<ResultModel> FollowUser(UserFollowingModel userFollowing);
        public Task<ResultModel> UnFollowUser(UserFollowingModel userFollowing);
        public Task<ResultModel> GetFollowers(Guid userId);
        public Task<ResultModel> GetFollowings(Guid userId);
    }
}
