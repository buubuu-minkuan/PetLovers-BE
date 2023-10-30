using Data.Entities;
using Data.Repositories.GenericRepository;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.ReportRepo
{
    public class ReportRepo : Repository<TblReport>, IReportRepo
    {
        private readonly PetLoversDbContext _context;

        public ReportRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
    }
}
