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
        private readonly UserAuthentication _userAuthentication;

        public PostServices(IPostRepo postRepo, IPostAttachmentRepo postAttachmentRepo, IPostReactionRepo postReactionRepo)
        {
            _postReactionRepo = postReactionRepo;
            _postAttachmentRepo = postAttachmentRepo;
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
                var data = await _postRepo.GetNewFeed(userId);
                if (data == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Not found";
                    result.Code = 200;
                    return result;
                }
                result.IsSuccess = true;
                result.Data = data;
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
                    userId = userId,
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
                else if (!userId.Equals(post.userId))
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
                        await _postAttachmentRepo.Insert(attachment);
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
                                await _postAttachmentRepo.Update(getAttachment);
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
                else if (!userId.Equals(post.userId))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You do not have permission to delete this post";
                    return result;
                }
                else
                {
                    tblPost.Status = Status.DEACTIVE;
                    _ = _postRepo.Update(tblPost);
                    List<TblPostAttachment> Attachments = await _postAttachmentRepo.GetListTblPostAttachmentById(postReq.postId);
                    List<TblPostReaction> Reactions = await _postReactionRepo.GetListReactionById(postReq.postId);
                    foreach(var attachment in Attachments)
                    {
                        attachment.Status = Status.DEACTIVE;
                        _ = _postAttachmentRepo.Update(attachment);
                    }

                    foreach(var reaction in Reactions)
                    {
                        reaction.Status = Status.DEACTIVE;
                        _ = _postReactionRepo.Update(reaction);
                    }
                    result.IsSuccess = false;
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
    }
}
