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
        public async Task<TblPost> GetPostById(Guid id)
        {
            return await _context.TblPosts.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
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
                    var postFollowing = await _context.TblPosts.Where(x => x.UserId.Equals(postAuthor.Id) && (now - x.CreateAt).TotalMilliseconds <= 900000).FirstOrDefaultAsync();
                    if (postFollowing != null)
                    {
                        posts.Add(new PostResModel()
                        {
                            Id = postFollowing.Id,
                            userId = postFollowing.UserId,
                            content = postFollowing.Content,
                            attachment = postFollowing.Attachment,
                            createdAt = postFollowing.CreateAt,
                            updatedAt = postFollowing.UpdateAt
                        });
                    }
                }

                foreach (var postAuthor in following)
                {
                    var newPost = await _context.TblPosts.Where(x => !x.UserId.Equals(postAuthor.Id) && (now - x.CreateAt).TotalMilliseconds <= 900000).ToListAsync();
                    posts.AddRange(newPost.Select(x => new PostResModel()
                    {
                        Id = x.Id,
                        userId = x.UserId,
                        content = x.Content,
                        attachment = x.Attachment,
                        createdAt = x.CreateAt,
                        updatedAt = x.UpdateAt
                    }));
                }
            }
            else
            {
                var newPost = await _context.TblPosts.Where(x => (now - x.CreateAt).TotalMilliseconds <= 900000).ToListAsync();
                posts.AddRange(newPost.Select(x => new PostResModel()
                {
                    Id = x.Id,
                    userId = x.UserId,
                    content = x.Content,
                    attachment = x.Attachment,
                    createdAt = x.CreateAt,
                    updatedAt = x.UpdateAt
                }));
            }
            if (posts.Count <= 0)
            {
                var newPost = await _context.TblPosts.OrderBy(x => x.CreateAt).ToListAsync();
                foreach (var post in newPost)
                {
                    posts.Add(new PostResModel()
                    {
                        Id = post.Id,
                        userId = post.UserId,
                        content = post.Content,
                        attachment = post.Attachment,
                        createdAt = post.CreateAt,
                        updatedAt = post.UpdateAt
                    });
                }
            }
            return posts;
        }
    }
}
