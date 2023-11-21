using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Models.SearchModel;
using Data.Models.UserModel;
using Data.Repositories.GenericRepository;

namespace Data.Repositories.UserRepo
{
    public interface IUserRepo : IRepository<TblUser>
    {
        public Task<TblUser> getUserByUsername(string username);
        public Task<Guid> GetRoleId(string RoleName);
        public Task<string> GetRoleName(Guid id);
        public Task<TblUser> GetUserByEmail(string Email);
        public Task<UserModel> GetUserById(Guid id);
        public Task<List<UserModel>> GetFollowingUser(Guid authorId);
        public Task<List<UserSearchModel>> SearchUser(string keyword, Guid userId);
        public Task<List<GetListUserModel>> GetListUserForAdmin();
    }
}
