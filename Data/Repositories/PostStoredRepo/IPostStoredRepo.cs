using Data.Entities;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostStoredRepo
{
    public interface IPostStoredRepo : IRepository<TblPostStored>
    {
        public Task<TblPostStored> GetStoredPost(Guid userId, Guid postId);
    }
}
