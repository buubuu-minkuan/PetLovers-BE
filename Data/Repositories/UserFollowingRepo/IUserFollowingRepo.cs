using Data.Models.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.UserFollowingRepo
{
    public interface IUserFollowingRepo
    {
        Task FollowUser(Guid userId, Guid followerId);

        Task UnfollowUser(Guid userId, Guid followerId);

        Task<bool> IsFollowing(Guid userId, Guid followerId);

    }
}
