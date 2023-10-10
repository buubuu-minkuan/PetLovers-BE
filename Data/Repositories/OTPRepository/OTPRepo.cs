using Data.Entities;
using Data.Repositories.GenericRepository;
using Data.Repositories.PostRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Data.Repositories.OTPRepository
{
    public class OTPRepo : Repository<TblOtpverify>, IOTPRepo
    {
        private readonly PetLoversDbContext _context;
        public OTPRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
    }
}
