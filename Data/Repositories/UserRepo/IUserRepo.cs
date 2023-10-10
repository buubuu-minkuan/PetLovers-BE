using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Models.UserModel;
using Data.Repositories.GenericRepository;

namespace Data.Repositories.UserRepo
{
    public interface IUserRepo : IRepository<TblUser>
    {
        public Task<TblUser> getUserByUsername(string username);
        public Task<Guid> GetRoleId(string RoleName);
        public Task<TblUser> GetUserByEmail(string Email);
        public Task<UserModel> GetUserById(Guid id);
        public Task<List<UserModel>> GetFollowingUser(Guid authorId);
    }
}
