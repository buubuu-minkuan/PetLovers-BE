using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.CommentModel;
using Data.Models.PostAttachmentModel;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.PetPostTradeRepo;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostReactRepo;
using Data.Repositories.PostRepo;
using Data.Repositories.UserRepo;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        private readonly UserAuthentication _userAuthentication;
        private readonly IPetPostTradeRepo _petPostTradeRepo;

        public PostServices(IPostRepo postRepo, IPostAttachmentRepo postAttachmentRepo, IPostReactionRepo postReactionRepo, IUserRepo userRepo, IPetPostTradeRepo petPostTradeRepo)
        {
            _petPostTradeRepo = petPostTradeRepo;
            _postReactionRepo = postReactionRepo;
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
                TblPostStored newPost = new()
                {
                    UserId = userId,
                    PostId = postReq.postId,
                    Status = Status.ACTIVE,
                    //CreateAt = now,
                };
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> GetPostTradeById(Guid id)
        {
            ResultModel result = new();
            try
            {
                var post = await _postRepo.GetPostTradeById(id);
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
        public async Task<ResultModel> CreatePostTrade(PostTradeCreateReqModel newPost)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(newPost.token, "userid"));
            Guid postId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();
            var user = await _userRepo.GetUserById(userId);
            PostAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
            };
            TblPost postTradeReq = new()
            {
                Id = postId,
                Type = PostingType.TRADING,
                UserId = userId,
                Status = PostingStatus.APPROVED,
                IsProcessed = true,
                Content = newPost.content,
                CreateAt = now
            };
            try
            {
                _ = await _postRepo.Insert(postTradeReq);
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
                TblPetTradingPost tblPet = new()
                {
                    Id = petId,
                    PostId = postId,
                    Name = newPost.PetName,
                    Type = newPost.Type,
                    Breed = newPost.Breed,
                    Age = newPost.Age,
                    Gender = newPost.Gender,
                    Weight = newPost.Weight,
                    Status = Status.ACTIVE
                };
                _ = await _petPostTradeRepo.Insert(tblPet);
                PetPostTradeModel newPet = new()
                {
                    Name = newPost.PetName,
                    Type = newPost.Type,
                    Breed = newPost.Breed,
                    Age = newPost.Age,
                    Gender = newPost.Gender,
                    Weight = newPost.Weight,
                };
                PostTradeResModel postResModel = new()
                {
                    Id = postId,
                    Author = author,
                    Content = newPost.content,
                    Attachment = listAttachment,
                    Title = newPost.title,
                    createdAt = now,
                    updatedAt = null,
                    Type = newPost.Type,
                    Amount = newPost.Amount,
                    Pet = newPet

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
        public async Task<ResultModel> UpdatePostTrade(PostTradeUpdateReqModel postReq)
        {
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            ResultModel result = new();
            try
            {
                PostTradeResModel post = await _postRepo.GetPostTradeById(postReq.postId);
                TblPost tblPost = await _postRepo.GetTblPostTradeById(postReq.postId);
                TblPetTradingPost tblPet = await _petPostTradeRepo.GetTblPetPostTradingByPostId(postReq.postId);
                PetPostTradeModel pet = await _petPostTradeRepo.GetPetByPostId(postReq.postId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Post not found";
                    return result;
                }
                else if (!userId.Equals(post.Author.Id))
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "You do not have permission to update this post";
                    return result;
                }
                else
                {
                    if (!string.IsNullOrEmpty(postReq.content) && !post.Content.Equals(postReq.content))
                    {
                        post.Content = postReq.content;
                        tblPost.Content = postReq.content;
                    }
                    if (!string.IsNullOrEmpty(postReq.PetName) && !pet.Name.Equals(postReq.PetName))
                    {
                        pet.Name = postReq.PetName;
                        tblPet.Name = postReq.PetName;
                    }
                    if (!string.IsNullOrEmpty(postReq.Type) && !pet.Type.Equals(postReq.Type))
                    {
                        pet.Type = postReq.Type;
                        tblPet.Type = postReq.Type;
                    }
                    if (!string.IsNullOrEmpty(postReq.Breed) && !pet.Type.Equals(postReq.Breed))
                    {
                        pet.Breed = postReq.Breed;
                        tblPet.Breed = postReq.Breed;
                    }
                    if (!string.IsNullOrEmpty(postReq.Age) && !pet.Type.Equals(postReq.Age))
                    {
                        pet.Age = postReq.Age;
                        tblPet.Age = postReq.Age;
                    }
                    if (!string.IsNullOrEmpty(postReq.Gender) && !pet.Type.Equals(postReq.Gender))
                    {
                        pet.Gender = postReq.Gender;
                        tblPet.Gender = postReq.Gender;
                    }
                    if (postReq.Weight != null && !pet.Type.Equals(postReq.Weight))
                    {
                        pet.Weight = postReq.Weight;
                        tblPet.Weight = postReq.Weight;
                    }
                    if(postReq.Amount != null && !post.Amount.Equals(postReq.Amount))
                    {
                        post.Amount = postReq.Amount;
                        tblPost.Amount = postReq.Amount;
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
                    post.Pet = pet;
                    post.Attachment = await _postAttachmentRepo.GetListAttachmentByPostId(postReq.postId);
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
        public async Task<ResultModel> DeletePostTrade(PostDeleteReqModel postReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            try
            {
                PostTradeResModel post = await _postRepo.GetPostTradeById(postReq.postId);
                TblPost tblPost = await _postRepo.Get(postReq.postId);
                TblPetTradingPost tblPet = await _petPostTradeRepo.GetTblPetPostTradingByPostId(postReq.postId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 200;
                    result.Message = "Post not found";
                    return result;
                }
                else if (!userId.Equals(post.Author.Id))
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
                    tblPet.Status = Status.DEACTIVE;
                    _ = await _petPostTradeRepo.Update(tblPet);
                    _ = await _postRepo.Update(tblPost);
                    List<TblPostAttachment> Attachments = await _postAttachmentRepo.GetListTblPostAttachmentById(postReq.postId);
                    foreach (var attachment in Attachments)
                    {
                        attachment.Status = Status.DEACTIVE;
                        _ = await _postAttachmentRepo.Update(attachment);
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
    }
}
