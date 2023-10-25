﻿using Data.Entities;
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
            TblPost post = await _context.TblPosts.Where(x => x.Id.Equals(id) && x.Status.Equals(PostingStatus.APPROVED) && x.IsProcessed && x.Type.Equals(PostingType.POSTING)).FirstOrDefaultAsync();
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
            };
            List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
            var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
            var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
            return new PostResModel
            {
                Id = post.Id,
                author = author,
                amountComment = amountComment.Count,
                amountFeeling = amountFeeling.Count,
                content = post.Content,
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
                var UserModelPaste = new UserModel();
                UserModelPaste.Id = folower.Id;
                UserModelPaste.Name = followerInfor.Name;
                UserModelPaste.Username = followerInfor.Username;
                UserModelPaste.RoleId = followerInfor.RoleId;
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
                    };
                    if (postFollowing == null) continue;
                    if ((now - postFollowing.CreateAt).TotalMilliseconds > 900000) continue;
                    var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(postFollowing.Id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
                    var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(postFollowing.Id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
                    List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(postFollowing.Id);
                    if (postFollowing != null)
                    {
                        posts.Add(new PostResModel()
                        {
                            Id = postFollowing.Id,
                            author = author,
                            content = postFollowing.Content,
                            amountFeeling = amountFeeling.Count,
                            amountComment = amountComment.Count,
                            attachment = arrAttachment,
                            createdAt = postFollowing.CreateAt,
                            updatedAt = postFollowing.UpdateAt
                        });
                    }
                }
            }
            return posts;
        }

        public async Task<List<PostResModel>> GetAllPosts()
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
                };
                var amountComment = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.COMMENT)).ToListAsync();
                var amountFeeling = await _context.TblPostReactions.Where(x => x.PostId.Equals(post.Id) && x.Type.Equals(ReactionType.FEELING)).ToListAsync();
                List<PostAttachmentResModel> arrAttachment = await GetPostAttachment(post.Id);
                posts.Add(new PostResModel()
                {
                    Id = post.Id,
                    author = author,
                    content = post.Content,
                    attachment = arrAttachment,
                    createdAt = post.CreateAt,
                    updatedAt = post.UpdateAt,
                    amountFeeling = amountFeeling.Count,
                    amountComment = amountComment.Count
                });
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

        public async Task<List<PostResModel>> GetAllPendingPost()
        {
            List<TblPost> listTblPost = await _context.TblPosts.Where(x => x.Status.Equals(PostingStatus.PENDING) && x.Type.Equals(PostingType.POSTING) && !x.IsProcessed).ToListAsync();
            List<PostResModel> listResPost = new List<PostResModel>();
            foreach(var post in listTblPost)
            {
                var attachment = await GetPostAttachment(post.Id);
                TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(post.UserId)).FirstOrDefaultAsync();
                PostAuthorModel author = new()
                {
                    Id = user.Id,
                    Name = user.Name,
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
        public async Task<List<PostResModel>> GetUserPendingPost(Guid userId)
        {
            List<TblPost> listTblPost = await _context.TblPosts.Where(x => x.UserId.Equals(userId) && x.Status.Equals(PostingStatus.PENDING) && x.Type.Equals(PostingType.POSTING) && !x.IsProcessed).ToListAsync();
            List<PostResModel> listResPost = new List<PostResModel>();
            TblUser user = await _context.TblUsers.Where(x => x.Id.Equals(userId)).FirstOrDefaultAsync();
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
            };
            foreach (var post in listTblPost)
            {
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
    }
}
