using Data.Entities;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.UserRewardRepo
{
    public interface IUserRewardRepo : IRepository<TblUserReward>
    {
        public Task<List<TblReward>> GetListPostReward();
    }
}
