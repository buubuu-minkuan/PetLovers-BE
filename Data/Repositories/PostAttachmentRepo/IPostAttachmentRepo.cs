using Data.Entities;
using Data.Models.PostAttachmentModel;
using Data.Repositories.GenericRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostAttachmentRepo
{
    public interface IPostAttachmentRepo : IRepository<TblPostAttachment>
    {
        public Task<List<PostAttachmentResModel>> GetListAttachmentByPostId(Guid postId);

        public Task<TblPostAttachment> GetAttachmentById (Guid id);

        public Task<List<TblPostAttachment>> GetListTblPostAttachmentById (Guid id);
    }
}
