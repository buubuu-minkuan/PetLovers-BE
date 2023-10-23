using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.CommentModel;
using Data.Models.PostAttachmentModel;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostReactRepo;
using Data.Repositories.PostRepo;
using Data.Repositories.PostStoredRepo;
using Data.Repositories.UserRepo;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.PostServices
{
    public class PostServices : IPostServices
    {
        private readonly IPostRepo _postRepo;
        private readonly IPostAttachmentRepo _postAttachmentRepo;
        private readonly IPostReactionRepo _postReactionRepo;
        private readonly IUserRepo _userRepo;
        private readonly IPostStoredRepo _postStoredRepo;
        private readonly UserAuthentication _userAuthentication;

        public PostServices(IPostRepo postRepo, IPostAttachmentRepo postAttachmentRepo, IPostReactionRepo postReactionRepo, IUserRepo userRepo, IPostStoredRepo postStoredRepo)
        {
            _postReactionRepo = postReactionRepo;
            _postStoredRepo = postStoredRepo;
            _postAttachmentRepo = postAttachmentRepo;
            _userRepo = userRepo;
            _userAuthentication = new UserAuthentication();
            _postRepo = postRepo;
        }
        public async Task<ResultModel> GetPostById(Guid id)
        {
            ResultModel result = new();
            try
            {
                var post = await _postRepo.GetPostById(id);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Not found";
                    result.Code = 200;
                    return result;
                }
                result.IsSuccess = true;
                result.Data = post;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetNewsFeed(string token)
        {
            ResultModel result = new();
            try
            {
                Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
                List<PostResModel> postsFollow = await _postRepo.GetPostsFromFollow(userId);
                postsFollow.Sort((x, y) => x.createdAt.CompareTo(y.createdAt));
                List<PostResModel> allPosts = await _postRepo.GetAllPosts();
                foreach(var post in postsFollow)
                {
                    if (allPosts.Contains(post))
                    {
                        allPosts.Remove(post);
                        allPosts.Insert(0, post);
                    }
                }
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = allPosts;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> CreatePost(PostCreateReqModel newPost)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(newPost.token, "userid"));
            Guid postId = Guid.NewGuid();
            var user = await _userRepo.GetUserById(userId);
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
            };
            TblPost postReq = new()
            {
                Id = postId,
                Type = PostingType.POSTING,
                UserId = userId,
                Status = PostingStatus.PENDING,
                IsProcessed = false,
                Content = newPost.content,
                CreateAt = now
            };
            try
            {
                _ = await _postRepo.Insert(postReq);
                foreach (var attachement in newPost.attachment)
                {
                    TblPostAttachment newAttachment = new()
                    {
                        PostId = postId,
                        Attachment = attachement,
                        Status = Status.ACTIVE
                    };
                    _ = await _postAttachmentRepo.Insert(newAttachment);
                }
                List<PostAttachmentResModel> listAttachment = await _postAttachmentRepo.GetListAttachmentByPostId(postId);
                PostResModel postResModel = new()
                {
                    Id = postId,
                    author = author,
                    content = newPost.content,
                    attachment = listAttachment,
                    createdAt = now,
                    updatedAt = null
                };
                result.IsSuccess = true;
                result.Code = 200;
                result.Data = postResModel;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> UpdatePost(PostUpdateReqModel postReq)
        {
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            ResultModel result = new();
            try
            {
                PostResModel post = await _postRepo.GetPostById(postReq.postId);
                TblPost tblPost = await _postRepo.GetTblPostById(postReq.postId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Post not found";
                    return result;
                } 
                else if (!userId.Equals(post.author.Id))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You do not have permission to update this post";
                    return result;
                }
                else
                {
                    if (!string.IsNullOrEmpty(postReq.content) && !post.content.Equals(postReq.content))
                    {
                        post.content = postReq.content;
                        tblPost.Content = postReq.content;
                    }
                    var currentAttachments = await _postAttachmentRepo.GetListAttachmentByPostId(postReq.postId);
                    var newAttachments = postReq.attachment;
                    var attachmentsToAdd = new List<TblPostAttachment>();
                    foreach (var newAttachment in newAttachments)
                    {
                        var isAttachmentExist = currentAttachments.Any(x => x.Attachment.Equals(newAttachment));
                        if (!isAttachmentExist)
                        {
                            attachmentsToAdd.Add(new TblPostAttachment()
                            {
                                PostId = postReq.postId,
                                Attachment = newAttachment,
                                Status = Status.ACTIVE
                            });
                        }
                    }
                    foreach (var attachment in attachmentsToAdd)
                    {
                        _ = await _postAttachmentRepo.Insert(attachment);
                    }

                    foreach (var currentAttachment in currentAttachments)
                    {
                        if (currentAttachment.Status != Status.DEACTIVE)
                        {
                            var isAttachmentExist = newAttachments.Any(x => x.Equals(currentAttachment.Attachment));
                            if (!isAttachmentExist)
                            {
                                var getAttachment = await _postAttachmentRepo.GetAttachmentById(currentAttachment.Id);
                                getAttachment.Status = Status.DEACTIVE;
                                _ = await _postAttachmentRepo.Update(getAttachment);
                            }
                        }
                    }

                    tblPost.UpdateAt = now;
                    _ = await _postRepo.Update(tblPost);
                    post.attachment = await _postAttachmentRepo.GetListAttachmentByPostId(postReq.postId);
                    result.IsSuccess = true;
                    result.Data = post;
                    result.Code = 200;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> DeletePost(PostDeleteReqModel postReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            try
            {
                PostResModel post = await _postRepo.GetPostById(postReq.postId);
                TblPost tblPost = await _postRepo.GetTblPostById(postReq.postId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Post not found";
                    return result;
                }
                else if (!userId.Equals(post.author.Id))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You do not have permission to delete this post";
                    return result;
                }
                else
                {
                    tblPost.UpdateAt = now;
                    tblPost.Status = Status.DEACTIVE;
                    _ = await _postRepo.Update(tblPost);
                    List<TblPostAttachment> Attachments = await _postAttachmentRepo.GetListTblPostAttachmentById(postReq.postId);
                    List<TblPostReaction> Reactions = await _postReactionRepo.GetListReactionById(postReq.postId);
                    foreach(var attachment in Attachments)
                    {
                        attachment.Status = Status.DEACTIVE;
                        _ = await _postAttachmentRepo.Update(attachment);
                    }

                    foreach(var reaction in Reactions)
                    {
                        reaction.Status = Status.DEACTIVE;
                        _ = await _postReactionRepo.Update(reaction);
                    }
                    result.IsSuccess = true;
                    result.Code = 200;
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> StorePost(PostStoreReqModel postReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            try
            {
                var checkExist = await _postStoredRepo.GetStoredPost(userId, postReq.postId);
                if(checkExist != null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "This post is already stored!";
                    return result;
                }
                TblPostStored newPost = new()
                {
                    UserId = userId,
                    PostId = postReq.postId,
                    Status = Status.ACTIVE,
                    CreateAt = now
                };
                _ = await _postStoredRepo.Insert(newPost);
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> RemoveStorePost(PostStoreReqModel postReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            try
            {
                var checkExist = await _postStoredRepo.GetStoredPost(userId, postReq.postId);
                if (checkExist == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "This post haven't been stored yet!";
                    return result;
                }
                checkExist.Status = Status.DEACTIVE;
                _ = await _postStoredRepo.Insert(checkExist);
                result.IsSuccess = true;
                result.Code = 200;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
    }
}
