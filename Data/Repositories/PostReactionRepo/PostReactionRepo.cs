using Data.Entities;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostReactRepo
{
    public class PostReactionRepo : Repository<TblPostReaction>, IPostReactionRepo
    {
        private readonly PetLoversDbContext _context;
        public PostReactionRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<TblPostReaction>> GetListReactionById(Guid Id)
        {
            return await _context.TblPostReactions.Where(x => x.PostId.Equals(Id)).ToListAsync();
        }
    }
}
