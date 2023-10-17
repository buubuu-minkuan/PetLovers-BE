using Data.Entities;
using Data.Models.PostModel;
using Data.Repositories.UserRepo;
using Data.Repositories.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Models.UserModel;
using Data.Enums;
using System.Net.Mail;

namespace Data.Repositories.PostRepo
{
    public class PostRepo : Repository<TblPost>, IPostRepo
    {
        private readonly PetLoversDbContext _context;

        public PostRepo(/*IMapper mapper,*/ PetLoversDbContext context) : base(context)
        {
            //_mapper = mapper;
            _context = context;
        }
        public async Task<PostResModel> GetPostById(Guid id)
        {
            TblPost post = await _context.TblPosts.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            string[] arrAttachment = await GetPostAttachment(post.Id);
            return new PostResModel
            {
                Id = post.Id,
                content = post.Content,
                attachment = arrAttachment,
                createdAt = post.CreateAt,
                updatedAt = post.UpdateAt,
            };
        }

        public async Task<List<PostResModel>> GetNewFeed(Guid userId)
        {
            var following = new List<UserModel>();
            var listFollowers = await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId)).ToListAsync();

            foreach (var folower in listFollowers)
            {
                var folowerInfor = await _context.TblUsers.Where(x => x.Id.Equals(folower.FollowerId)).FirstOrDefaultAsync();
                var UserModelPaste = new UserModel();
                UserModelPaste.Id = folower.Id;
                UserModelPaste.Username = folowerInfor.Username;
                UserModelPaste.RoleId = folowerInfor.RoleId;
                UserModelPaste.Status = folowerInfor.Status;
                UserModelPaste.Email = folowerInfor.Email;
                UserModelPaste.CreateAt = folowerInfor.CreateAt;
                following.Add(UserModelPaste);
            }
            List<PostResModel> posts = new();
            DateTime now = DateTime.Now;
            if (following != null)
            {
                foreach (var postAuthor in following)
                {
                    var postFollowing = await _context.TblPosts.Where(x => x.UserId.Equals(postAuthor.Id) && (now - x.CreateAt).TotalMilliseconds <= 900000 && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed).FirstOrDefaultAsync();
                    var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(postFollowing.Id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
                    var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(postFollowing.Id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
                    string[] arrAttachment = await GetPostAttachment(postFollowing.Id);
                    if (postFollowing != null)
                    {
                        posts.Add(new PostResModel()
                        {
                            Id = postFollowing.Id,
                            userId = postFollowing.UserId,
                            content = postFollowing.Content,
                            amountFeeling = amountFeeling.Count,
                            amountComment = amountComment.Count,
                            attachment = arrAttachment,
                            createdAt = postFollowing.CreateAt,
                            updatedAt = postFollowing.UpdateAt
                        });
                    }
                }

                foreach (var postAuthor in following)
                {
                    List<TblPost> newPost = await _context.TblPosts.Where(x => !x.UserId.Equals(postAuthor.Id) && (now - x.CreateAt).TotalMilliseconds <= 900000 && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed).ToListAsync();
                    foreach(var post in newPost)
                    {
                        var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
                        var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
                        string[] arrAttachment = await GetPostAttachment(post.Id);
                        posts.AddRange(newPost.Select(x => new PostResModel()
                        {
                            Id = x.Id,
                            userId = x.UserId,
                            content = x.Content,
                            attachment = arrAttachment,
                            createdAt = x.CreateAt,
                            updatedAt = x.UpdateAt,
                            amountFeeling = amountFeeling.Count,
                            amountComment = amountComment.Count
                        }));
                    }
                }
            }
            else
            {
                List<TblPost> newPost = await _context.TblPosts.Where(x => (now - x.CreateAt).TotalMilliseconds <= 900000 && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed).ToListAsync();
                foreach (var post in newPost)
                {
                    var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
                    var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
                    string[] arrAttachment = await GetPostAttachment(post.Id);
                    posts.AddRange(newPost.Select(x => new PostResModel()
                    {
                        Id = x.Id,
                        userId = x.UserId,
                        content = x.Content,
                        attachment = arrAttachment,
                        createdAt = x.CreateAt,
                        updatedAt = x.UpdateAt,
                        amountFeeling = amountFeeling.Count,
                        amountComment = amountComment.Count
                    }));
                }
            }
            if (posts.Count <= 0)
            {
                var newPost = await _context.TblPosts.Where(x => x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed).OrderBy(x => x.CreateAt).ToListAsync();
                foreach (var post in newPost)
                {
                    var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
                    var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
                    string[] arrAttachment = await GetPostAttachment(post.Id);
                    posts.Add(new PostResModel()
                    {
                        Id = post.Id,
                        userId = post.UserId,
                        content = post.Content,
                        attachment = arrAttachment,
                        createdAt = post.CreateAt,
                        updatedAt = post.UpdateAt,
                        amountFeeling = amountFeeling.Count,
                        amountComment = amountComment.Count
                    });
                }
            }
            return posts;
        }

        private async Task<string[]> GetPostAttachment(Guid postId)
        {
            TblPostAttachment[] postAttachment = await _context.TblPostAttachments.Where(x => x.PostId.Equals(postId)).ToArrayAsync();
            string[] arrAttachment = new string[postAttachment.Length];
            foreach (var attachment in postAttachment)
            {
                _ = arrAttachment.Append(attachment.Attachment);
            }
            return arrAttachment;
        }
    }
}
