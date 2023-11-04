using Data.Entities;
using Data.Models.PostModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.UserModel
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Image { get; set; }
        public string Phone { get; set; } = null!;
        public RoleModel Role { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreateAt { get; set; }
    }

    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UserLoginResModel
    {
        public UserModel UserModel { get; set; }
        public string? token { get; set; }
    }

    public class UserPageModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; }
        public string Username { get; set; } = null!;
        public bool IsVerify { get; set; }
        public string? Image { get; set; }
        public RoleModel Role { get; set; }
        public int Following { get; set; }
        public int Follower { get; set; }
        public List<PostResModel>? posts { get; set; }
    }

    public class OtherUserPageModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; }
        public string Username { get; set; } = null!;
        public string? Image { get; set; }
        public bool IsFollowed { get; set; }
        public RoleModel Role { get; set; }
        public int Following { get; set; }
        public int Follower { get; set; }
        public List<PostResModel>? posts { get; set; }
    }

    public class UserResgisterModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string Phone { get; set; } = null!;
    }

    public class UserLoginReqModel
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class UserChangePasswordModel
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }

    public class UserUpdateReqModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Image { get; set; }
    }

    public class UserFollowingModel
    {
        public string token { get; set; }
        public Guid userId { get; set; }
    }

    public class UserResetPasswordModel
    {
        public Guid UserId { get; set;} 
        public string NewPassword { get; set; } = null!;
    }

    public class UserFollowResModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
    }
        
}
