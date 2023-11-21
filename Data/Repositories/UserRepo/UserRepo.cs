using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Enums;
using Data.Models.SearchModel;
using Data.Models.UserModel;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.UserRepo
{
    public class UserRepo : Repository<TblUser>, IUserRepo
    {
        private readonly PetLoversDbContext _context;
        public UserRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<TblUser> getUserByUsername(string username)
        {
            return await _context.TblUsers.Where(x => x.Username.Equals(username)).FirstOrDefaultAsync();
        }

        public async Task<Guid> GetRoleId(string RoleName)
        {
            var role = await _context.TblRoles.Where(x => x.Name.ToLower().Equals(RoleName.ToLower())).FirstOrDefaultAsync();
            return role.Id;
        }

        public async Task<TblUser> GetUserByEmail(string Email)
        {
            return await _context.TblUsers.Where(x => x.Email.Equals(Email)).FirstOrDefaultAsync();
        }

        public async Task<UserModel> GetUserById(Guid id)
        {
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            var role = await _context.TblRoles.Where(x => x.Id.Equals(user.RoleId)).FirstOrDefaultAsync();
            RoleModel roleInfor = new();
            roleInfor.Id = role.Id;
            roleInfor.Name = role.Name;
            UserModel reponseUser = new UserModel()
            {
                Id = id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                Image = user.Image,
                Phone = user.Phone,
                Role = roleInfor,
                Status = user.Status,
                CreateAt = user.CreateAt,
            };
            return reponseUser;
        }

        public async Task<List<UserModel>> GetFollowingUser(Guid authorId)
        {
            var result = new List<UserModel>();
            var listFollowers = await _context.TblUserFollowings.Where(x => x.UserId.Equals(authorId)).ToListAsync();

            foreach (var folower in listFollowers)
            {
                var folowerInfor = await _context.TblUsers.Where(x => x.Id.Equals(folower.Id)).FirstOrDefaultAsync();
                var role = await _context.TblRoles.Where(x => x.Id.Equals(folowerInfor.RoleId)).FirstOrDefaultAsync();
                RoleModel roleInfor = new();
                roleInfor.Id = role.Id;
                roleInfor.Name = role.Name;
                var UserModelPaste = new UserModel();
                UserModelPaste.Id = folower.Id;
                UserModelPaste.Username = folowerInfor.Username;
                UserModelPaste.Role = roleInfor;
                UserModelPaste.Status = folowerInfor.Status;
                UserModelPaste.Email = folowerInfor.Email;
                UserModelPaste.CreateAt = folowerInfor.CreateAt;
                result.Add(UserModelPaste);
            }
            return result;
        }

        public async Task<string> GetRoleName(Guid id)
        {
            var role = await _context.TblRoles.Where(x => x.Id == id).FirstOrDefaultAsync();
            return role.Name;
        }

        public async Task<List<UserSearchModel>> SearchUser(string keyword, Guid userId)
        {
            var users = await context.TblUsers.Where(x => x.Username.Contains(keyword) || x.Name.Contains(keyword)).ToListAsync();
            List<UserSearchModel> listUser = new();
            foreach (var user in users)
            {
                var follow = await context.TblUserFollowings.Where(x => x.UserId.Equals(userId) && x.FollowerId.Equals(user.Id) && x.Status.Equals(Status.ACTIVE)).FirstOrDefaultAsync();
                bool isFollow = false;
                if (follow != null)
                {
                    isFollow = true;
                }
                listUser.Add(new UserSearchModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Avatar = user.Image,
                    Fullname = user.Name,
                    IsFollow = isFollow
                });
            }
            return listUser;
        }
        public async Task<List<GetListUserModel>> GetListUserForAdmin()
        {
            var users = await context.TblUsers.Where(x => !x.Status.Equals(UserStatus.DEACTIVE)).ToListAsync();
            List<GetListUserModel> listUser = new();
            foreach (var user in users)
            {
                listUser.Add(new GetListUserModel()
                {
                    Id = user.Id,
                    Name = user.Name,
                    Image = user.Image,
                    Email = user.Email,
                    RoleName = await GetRoleName(user.RoleId),
                    CreateAt = user.CreateAt,
                });
            }
            return listUser;
        }
    }
}
