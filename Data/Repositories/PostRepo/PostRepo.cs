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
using Data.Models.PostAttachmentModel;
using System.Reflection;
using System.Reflection.Metadata;
using System.Xml.Linq;

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
        public async Task<PostResModel> GetPostById(Guid id, Guid userId)
        {
            TblPost post = await _context.TblPosts.Where(x => x.Id.Equals(id) && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed && x.Type.Equals(PostingType.POSTING)).FirstOrDefaultAsync();
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.Image
            };
            List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
            var Comment = await _context.TblPostReactions.Where(x => x.PostId.Equals(id) && x.Type.Equals(ReactionType.COMMENT) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
            var Feeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(id) && x.Type.Equals(ReactionType.FEELING) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
            bool isFeeling = false;
            bool isAuthor = false;
            if (post.UserId.Equals(userId))
            {
                isAuthor = true;
            }
            foreach(var feeling in Feeling)
            {
                if (!feeling.UserId.Equals(userId))
                {
                    isFeeling = true; break;
                }
            }
            return new PostResModel
            {
                Id = post.Id,
                author = author,
                amountComment = Comment.Count,
                amountFeeling = Feeling.Count,
                content = post.Content,
                isFeeling = isFeeling,
                attachment = arrAttachment,
                createdAt = post.CreateAt,
                updatedAt = post.UpdateAt,
            };
        }

        public async Task<List<PostResModel>> GetPostsFromFollow(Guid userId)
        {
            var following = new List<UserModel>();
            var listFollowers = await _context.TblUserFollowings.Where(x => x.UserId.Equals(userId)).ToListAsync();

            foreach (var folower in listFollowers)
            {
                var followerInfor = await _context.TblUsers.Where(x => x.Id.Equals(folower.FollowerId)).FirstOrDefaultAsync();
                var role = _context.TblRoles.Where(x => x.Id == followerInfor.RoleId).FirstOrDefault();
                RoleModel getRole = new();
                role.Id = role.Id;
                role.Name = role.Name;
                var UserModelPaste = new UserModel();
                UserModelPaste.Id = folower.Id;
                UserModelPaste.Name = followerInfor.Name;
                UserModelPaste.Username = followerInfor.Username;
                UserModelPaste.Role = getRole;
                UserModelPaste.Status = followerInfor.Status;
                UserModelPaste.Email = followerInfor.Email;
                UserModelPaste.CreateAt = followerInfor.CreateAt;
                following.Add(UserModelPaste);
            }
            List<PostResModel> posts = new();
            DateTime now = DateTime.Now;
            if (following != null)
            {
                foreach (var postAuthor in following)
                {
                    var postFollowing = await _context.TblPosts.Where(x => x.UserId.Equals(postAuthor.Id) && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed && x.Type.Equals(PostingType.POSTING)).FirstOrDefaultAsync();
                    PostAuthorModel author = new()
                    {
                        Id = postAuthor.Id,
                        Name = postAuthor.Name,
                        ImageUrl = postAuthor.Image
                    };
                    if (postFollowing == null) continue;
                    if ((now - postFollowing.CreateAt).TotalMilliseconds > 900000) continue;
                    var Comment = await _context.TblPostReactions.Where(x => x.PostId.Equals(postFollowing.Id) && x.Type.Equals(ReactionType.COMMENT) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                    var Feeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(postFollowing.Id) && x.Type.Equals(ReactionType.FEELING) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                    bool isFeeling = false;
                    foreach (var feeling in Feeling)
                    {
                        if (!feeling.UserId.Equals(userId))
                        {
                            isFeeling = true; break;
                        }
                    }
                    List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(postFollowing.Id);
                    if (postFollowing != null)
                    {
                        posts.Add(new PostResModel()
                        {
                            Id = postFollowing.Id,
                            author = author,
                            content = postFollowing.Content,
                            amountFeeling = Feeling.Count,
                            isFeeling = isFeeling,
                            amountComment = Comment.Count,
                            attachment = arrAttachment,
                            createdAt = postFollowing.CreateAt,
                            updatedAt = postFollowing.UpdateAt
                        });
                    }
                }
            }
            return posts;
        }

        public async Task<List<PostResModel>> GetPostsFromUser(Guid userId)
        {
            var post = await _context.TblPosts.Where(x => x.UserId.Equals(userId) && x.Type.Equals(PostingType.POSTING) && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed).ToListAsync();
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.Image
            };
            List<PostResModel> listPostRes = new();
            foreach (var p in post)
            {
                var Comment = await _context.TblPostReactions.Where(x => x.PostId.Equals(p.Id) && x.Type.Equals(ReactionType.COMMENT) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                var Feeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(p.Id) && x.Type.Equals(ReactionType.FEELING) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                bool isFeeling = false;
                bool isAuthor = false;
                if (p.UserId.Equals(userId))
                {
                    isAuthor = true;
                }
                foreach (var feeling in Feeling)
                {
                    if (feeling.UserId.Equals(userId))
                    {
                        isFeeling = true; break;
                    }
                }
                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(p.Id);
                listPostRes.Add(new PostResModel()
                {
                    Id = p.Id,
                    author = author,
                    content = p.Content,
                    amountFeeling = Feeling.Count,
                    isFeeling = isFeeling,
                    amountComment = Comment.Count,
                    attachment = arrAttachment,
                    createdAt = p.CreateAt,
                    updatedAt = p.UpdateAt
                });
            }
            return listPostRes;
        }

        public async Task<List<PostResModel>> GetAllPosts(Guid userId)
        {
            List<PostResModel> posts = new();
            var newPost = await _context.TblPosts.Where(x => x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed && x.Type.Equals(PostingType.POSTING)).OrderByDescending(x => x.CreateAt).ToListAsync();
            foreach (var post in newPost)
            {
                TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };
                var Comment = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.COMMENT) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                var Feeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.FEELING) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
                bool isFeeling = false;
                bool isAuthor = false;
                if (post.UserId.Equals(userId))
                {
                    isAuthor = true;
                }
                foreach (var feeling in Feeling)
                {
                    if (feeling.UserId.Equals(userId))
                    {
                        isFeeling = true; break;
                    }
                }
                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
                posts.Add(new PostResModel()
                {
                    Id = post.Id,
                    author = author,
                    content = post.Content,
                    attachment = arrAttachment,
                    isFeeling = isFeeling,
                    isAuthor = isAuthor,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt,
                    amountFeeling = Feeling.Count,
                    amountComment = Comment.Count
                }) ;
            }
            return posts;
        }

        public async Task<TblPost> GetTblPostById(Guid id)
        {
            return await _context.TblPosts.Where(x => x.Id.Equals(id) && x.Type.Equals(PostingType.POSTING)).FirstOrDefaultAsync();
        }

        private async Task<List<PostAttachmentResModel>> GetPostAttachment(Guid postId)
        {
            var listAttachment = await _context.TblPostAttachments.Where(x => x.PostId.Equals(postId) && x.Status.Equals(Status.ACTIVE)).ToListAsync();
            List<PostAttachmentResModel> listResAttachement = new List<PostAttachmentResModel>();
            foreach (var postAttachment in listAttachment)
            {
                PostAttachmentResModel attachment = new()
                {
                    Id = postAttachment.Id,
                    Attachment = postAttachment.Attachment,
                    Status = postAttachment.Status,
                };
                listResAttachement.Add(attachment);
            }
            return listResAttachement;
        }
        
        public async Task<PostTradeResModel> GetPostTradeById(Guid id)
        {
            TblPost post = await _context.TblPosts.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            TblPetTradingPost pet = await _context.TblPetTradingPosts.Where(x => x.PostId.Equals(id)).FirstOrDefaultAsync();
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.Image
            };
            PetPostTradeModel petpost = new()
            {
                Name = pet.Name,
                Type = pet.Type,
                Breed = pet.Breed,
                Age = pet.Age,
                Gender = pet.Gender,
                Weight = pet.Weight,
                Color = pet.Color
            };

            List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
            PostTradeResModel res = new()
            {
                Id = post.Id,
                Author = author,
                Title = post.Title,
                Content = post.Content,
                Type = post.Type,
                Amount = post.Amount,
                Pet = petpost,
                Attachment = arrAttachment,
                createdAt = post.CreateAt,
                updatedAt = post.UpdateAt,
            };
            if(post.Status.Equals(TradingStatus.INPROGRESS))
            {
                res.isTrading = true;
            } else
            {
                res.isTrading = false;
            }
            return res;
        }
        
        public async Task<List<PostTradeResModel>> GetAllTradePostsTitle()
        {
            List<PostTradeResModel> posts = new();
            var newPost = await _context.TblPosts.Where(x => x.Status.Equals(TradingStatus.INPROGRESS) && x.IsProcessed).OrderByDescending(x => x.CreateAt).ToListAsync();
            foreach (var post in newPost)
            {
                TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };

                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
                posts.Add(new PostTradeResModel()
                {
                    Id = post.Id,
                    Author = author,
                    Title = post.Title,
                    Type = post.Type,
                    Amount = post.Amount,
                    Attachment = arrAttachment,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt,
                });
            }
            return posts;
        }

        public async Task<List<PostResModel>> GetAllPendingPost()
        {
            List<TblPost> listTblPost = await _context.TblPosts.Where(x => x.Status.Equals(PostingStatus.PENDING) && x.Type.Equals(PostingType.POSTING) && !x.IsProcessed).OrderBy(x => x.CreateAt).ToListAsync();
            List<PostResModel> listResPost = new List<PostResModel>();
            foreach(var post in listTblPost)
            {
                var attachment = await GetPostAttachment(post.Id);
                TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };
                listResPost.Add(new PostResModel()
                {
                    Id = post.Id,
                    author = author,
                    content = post.Content,
                    attachment = attachment,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt
                });
            }
            return listResPost;
        }
                
        public async Task<TblPost> GetTblPostTradeById(Guid id)
        {
            return await _context.TblPosts.Where(x => x.Id.Equals(id) && x.Type.Equals(PostingType.TRADING)).FirstOrDefaultAsync();
        }
        
        public async Task<List<PostResModel>> GetUserPendingPost(Guid userId)
        {
            List<TblPost> listTblPost = await _context.TblPosts.Where(x => x.UserId.Equals(userId) && x.Status.Equals(PostingStatus.PENDING) && x.Type.Equals(PostingType.POSTING) && !x.IsProcessed).OrderByDescending(x => x.CreateAt).ToListAsync();
            List<PostResModel> listResPost = new List<PostResModel>();
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.Image
            };
            foreach (var post in listTblPost)
            {
                bool isAuthor = false;
                if (post.UserId.Equals(userId))
                {
                    isAuthor = true;
                }
                var attachment = await GetPostAttachment(post.Id);
                listResPost.Add(new PostResModel()
                {
                    Id = post.Id,
                    author = author,
                    content = post.Content,
                    attachment = attachment,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt
                });
            }
            return listResPost;
        }

        public async Task<List<PostTradeResModel>> GetPostTradingInProgressByUserId(Guid userId)
        {
            var post = await _context.TblPosts.Where(x => x.UserId.Equals(userId) && x.Type.Equals(PostingType.TRADING) && !x.Status.Equals(TradingStatus.DONE)).ToListAsync();
            List<PostTradeResModel> posts = new();
            foreach (var p in post)
            {
                TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(p.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
                    ImageUrl = user.Image
                };

                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(p.Id);
                posts.Add(new PostTradeResModel()
                {
                    Id = p.Id,
                    Author = author,
                    Title = p.Title,
                    Type = p.Type,
                    Amount = p.Amount,
                    Attachment = arrAttachment,
                    createdAt = p.CreateAt,
                    updatedAt = p.UpdateAt,
                });
            }
            return posts;
        }

        public async Task<List<TblPost>> GetListPostTradingByUserId(Guid userId)
        {
            return await _context.TblPosts.Where(x => x.UserId.Equals(userId) && x.Type.Equals(PostingType.TRADING)).ToListAsync();
        }

        public async Task<List<TblPost>> GetPostsApproveByUserId(Guid userId)
        {
            return await _context.TblPosts.Where(x => x.UserId.Equals(userId) && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed).ToListAsync();
        }
    }
}
