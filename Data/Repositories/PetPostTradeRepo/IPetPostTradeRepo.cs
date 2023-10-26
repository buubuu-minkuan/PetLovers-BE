using Data.Entities;
using Data.Models.PostModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PetPostTradeRepo
{
    public interface IPetPostTradeRepo : IRepository<TblPetTradingPost>
    {
        public Task<TblPetTradingPost> GetTblPetPostTradingByPostId(Guid postId);
        public Task<PetPostTradeModel> GetPetByPostId(Guid id);
    }
}
