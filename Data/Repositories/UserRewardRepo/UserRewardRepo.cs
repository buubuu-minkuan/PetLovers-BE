using Data.Entities;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.UserRewardRepo
{
    public class UserRewardRepo : Repository<TblUserReward>, IUserRewardRepo
    {
        private readonly PetLoversDbContext _context;

        public UserRewardRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<TblReward>> GetListPostReward()
        {
            return await _context.TblRewards.Where(x => x.TotalPost != null).ToListAsync();
        }
    }
}
