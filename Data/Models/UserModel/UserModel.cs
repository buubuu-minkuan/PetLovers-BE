using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.UserModel
{
    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Guid RoleId { get; set; }
        public string Status { get; set; } 
        public DateTime CreateAt { get; set; }
    }
}
