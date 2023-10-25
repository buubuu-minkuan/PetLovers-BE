using Data.Entities;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PetPostTradeRepo
{
    public class PetPostTradeRepo : Repository<TblPetTradingPost>, IPetPostTradeRepo
    {
        private readonly PetLoversDbContext _context;
        public PetPostTradeRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public async Task<TblPetTradingPost> GetTblPetPostTradingByPostId(Guid postId)
        {
            return await _context.TblPetTradingPosts.Where(x => x.PostId.Equals(postId)).FirstOrDefaultAsync();
        }

        public async Task<PetPostTradeModel> GetPetByPostId(Guid postId)
        {
            var pet = await _context.TblPetTradingPosts.Where(x => x.PostId.Equals(postId)).FirstOrDefaultAsync();
            PetPostTradeModel newPet = new();
            newPet.Gender = pet.Gender
            return newPet
        }
    }
}
