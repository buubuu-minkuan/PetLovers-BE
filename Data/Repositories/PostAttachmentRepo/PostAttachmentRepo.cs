using Data.Entities;
using Data.Enums;
using Data.Models.PostAttachmentModel;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.PostAttachmentRepo
{
    public class PostAttachmentRepo : Repository<TblPostAttachment>, IPostAttachmentRepo
    {
        private readonly PetLoversDbContext _context;
        public PostAttachmentRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }

        public async Task<List<PostAttachmentResModel>> GetListAttachmentByPostId(Guid postId)
        {
            var listAttachment = await _context.TblPostAttachments.Where(x => x.PostId.Equals(postId) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
            List<PostAttachmentResModel> listResAttachement = new List<PostAttachmentResModel>();
            foreach (var postAttachment in listAttachment)
            {
                PostAttachmentResModel attachment = new()
                {
                    Id = postAttachment.Id,
                    PostId = postAttachment.PostId,
                    Attachment = postAttachment.Attachment,
                    Status = postAttachment.Status,
                };
                listResAttachement.Add(attachment);
            }
            return listResAttachement;
        }

        public async Task<TblPostAttachment> GetAttachmentById(Guid id)
        {
            return await _context.TblPostAttachments.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task<List<TblPostAttachment>> GetListTblPostAttachmentById (Guid id)
        {
            return await _context.TblPostAttachments.Where(x => x.PostId.Equals(id)).ToListAsync();
        }

    }
}
