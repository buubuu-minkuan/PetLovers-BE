using Azure.Core;
using Business.Ultilities.UserAuthentication;
using Data.Entities;
using Data.Enums;
using Data.Models.CommentModel;
using Data.Models.PostAttachmentModel;
using Data.Models.PostModel;
using Data.Models.ResultModel;
using Data.Models.UserModel;
using Data.Repositories.HashtagRepo;
using Data.Repositories.PetPostTradeRepo;
using Data.Repositories.PostAttachmentRepo;
using Data.Repositories.PostReactRepo;
using Data.Repositories.PostRepo;
using Data.Repositories.PostStoredRepo;
using Data.Repositories.PostTradeRequestRepo;
using Data.Repositories.ReportRepo;
using Data.Repositories.UserRewardRepo;
using Data.Repositories.UserRepo;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math.Field;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
        private readonly IPetPostTradeRepo _petPostTradeRepo;
        private readonly IPostTradeRequestRepo _postTradeRequestRepo;
        private readonly IReportRepo _reportRepo;
        private readonly IHashtagRepo _hashtagRepo;
        private readonly IUserRewardRepo _rewardRepo;

        public PostServices(IPostRepo postRepo, IPostAttachmentRepo postAttachmentRepo, IPostReactionRepo postReactionRepo, IUserRepo userRepo, IPetPostTradeRepo petPostTradeRepo, IPostStoredRepo postStoredRepo, IReportRepo reportRepo, IPostTradeRequestRepo postTradeRequestRepo, IUserRewardRepo rewardRepo, IHashtagRepo hashtagRepo)
        {
            _hashtagRepo = hashtagRepo;
            _rewardRepo = rewardRepo;
            _postTradeRequestRepo = postTradeRequestRepo;
            _reportRepo = reportRepo;
            _petPostTradeRepo = petPostTradeRepo;
            _postReactionRepo = postReactionRepo;
            _postStoredRepo = postStoredRepo;
            _postAttachmentRepo = postAttachmentRepo;
            _userRepo = userRepo;
            _userAuthentication = new UserAuthentication();
            _postRepo = postRepo;
        }
        
        public async Task<ResultModel> GetPostById(Guid id, string token)
        {
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var post = await _postRepo.GetPostById(id, userId);
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
                List<PostResModel> allPosts = await _postRepo.GetAllPosts(userId);
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
                ImageUrl = user.Image
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
                foreach (var hashtag in newPost.hashtag)
                {
                    TblPostHashtag newHashtag = new()
                    {
                        PostId = postId,
                        Hashtag = hashtag,
                        Status = Status.ACTIVE,
                        CreateAt = now
                    };
                    _ = await _hashtagRepo.Insert(newHashtag);
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
                PostResModel post = await _postRepo.GetPostById(postReq.postId, userId);
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
                    var currentHashtags = await _hashtagRepo.GetListHashTagByPostId(postReq.postId);
                    var newHashtags = postReq.hashtag;
                    var hashtagstoAdd = new List<TblPostHashtag>();
                    foreach (var newHashtag in newHashtags)
                    {
                        var isHashtagExist = currentHashtags.Any(x => x.Hashtag.Equals(newHashtag));
                        if (!isHashtagExist)
                        {
                            hashtagstoAdd.Add(new TblPostHashtag()
                            {
                                PostId = postReq.postId,
                                Hashtag = newHashtag,
                                Status = Status.ACTIVE,
                                CreateAt = now
                            });
                        }
                    }
                    foreach (var hashtag in hashtagstoAdd)
                    {
                        _ = await _hashtagRepo.Insert(hashtag);
                    }
                    foreach (var currentHashtag in currentHashtags)
                    {
                        if(currentHashtag.Status != Status.DEACTIVE)
                        {
                            var isHashtagExist = newHashtags.Any(x => x.Equals(currentHashtag.Hashtag));
                            if (!isHashtagExist)
                            {
                                var getHashtag = await _hashtagRepo.Get(currentHashtag.Id);
                                getHashtag.Status = Status.DEACTIVE;
                                _ = await _hashtagRepo.Update(getHashtag);
                            }
                        }
                        
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

        public async Task<ResultModel> DeletePost(PostReqModel postReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            try
            {
                foreach (var pReq in postReq.postId)
                {
                    PostResModel post = await _postRepo.GetPostById(pReq, userId);
                    TblPost tblPost = await _postRepo.GetTblPostById(pReq);
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
                        List<TblPostHashtag> Hashtags = await _hashtagRepo.GetListHashTagByPostId(pReq);
                        List<TblPostAttachment> Attachments = await _postAttachmentRepo.GetListTblPostAttachmentById(pReq);
                        List<TblPostReaction> Reactions = await _postReactionRepo.GetListReactionById(pReq);
                        foreach (var attachment in Attachments)
                        {
                            attachment.Status = Status.DEACTIVE;
                            _ = await _postAttachmentRepo.Update(attachment);
                        }

                        foreach (var reaction in Reactions)
                        {
                            reaction.Status = Status.DEACTIVE;
                            _ = await _postReactionRepo.Update(reaction);
                        }

                        foreach(var hashtag in Hashtags)
                        {
                            hashtag.Status = Status.DEACTIVE;
                            _ = await _hashtagRepo.Update(hashtag);
                        }
                        result.IsSuccess = true;
                        result.Code = 200;
                    }
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

        public async Task<ResultModel> StorePost(Guid postId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var checkExist = await _postStoredRepo.GetStoredPost(userId, postId);
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
                    PostId = postId,
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

        public async Task<ResultModel> RemoveStorePost(Guid postId, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var checkExist = await _postStoredRepo.GetStoredPost(userId, postId);
                if (checkExist == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "This post haven't been stored yet!";
                    return result;
                }
                checkExist.Status = Status.DEACTIVE;
                _ = await _postStoredRepo.Update(checkExist);
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

        public async Task<ResultModel> GetUserPendingPost(string token)
        {
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                List<PostResModel> listPost = await _postRepo.GetUserPendingPost(userId);
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = listPost;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }

        public async Task<ResultModel> GetPostTradeById(Guid id, string token)
        {
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var post = await _postRepo.GetPostTradeById(id);
                var user = await _userRepo.Get(userId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Not found";
                    result.Code = 200;
                    return result;
                }
                if (post.Author.Id.Equals(userId))
                {
                    var req = await _postTradeRequestRepo.GetListRequestPostTradeByPostId(id);
                    foreach (var r in req)
                    {
                        var u = await _userRepo.Get(r.UserId);
                        r.Name = u.Name;
                        r.SocialCredit = u.SocialCredit;
                    }
                    PostTradeAuthorResModel postRes = new()
                    {
                        Id = post.Id,
                        Author = post.Author,
                        Title = post.Title,
                        Content = post.Content,
                        Attachment = post.Attachment,
                        Amount = post.Amount,
                        Pet = post.Pet,
                        Type = post.Type,
                        createdAt = post.createdAt,
                        UserRequest = req,
                        isFree = post.isFree,
                        isTrading = post.isTrading,
                        Address = post.Address,
                        Status = post.Status
                    };
                    result.IsSuccess = true;
                    result.Data = postRes;
                    result.Code = 200;
                } else if (!post.Author.Id.Equals(userId))
                {
                    var req = await _postTradeRequestRepo.GetRequestPostTrade(id, userId);
                    PostTradeUserRequestModel userReq = new();
                    /*if (req != null && post.Status.Equals(TradingStatus.ACTIVE))
                    {
                        
                        if (userReq.Status.Equals(TradeRequestStatus.ACCEPT))
                        {
                            result.IsSuccess = true;
                            result.Data = post;
                            result.Code = 200;
                        } else
                        {
                            result.IsSuccess = false;
                            result.Code = 403;
                            result.Message = "You don't have permission to view this.";
                        }
                    } else
                    {
                        result.IsSuccess = true;
                        result.Data = post;
                        result.Code = 200;
                    }*/
                    if (req != null)
                    {
                        if (req.Status.Equals(TradeRequestStatus.PENDING) || req.Status.Equals(TradeRequestStatus.ACCEPT))
                        {
                            userReq.Id = req.Id;
                            userReq.UserId = req.UserId;
                            userReq.Status = req.Status;
                            userReq.createdAt = req.CreateAt;
                            userReq.Name = user.Name;
                            userReq.SocialCredit = user.SocialCredit;
                            post.UserRequest = userReq;
                            post.isRequest = true;
                            post.CanRequest = false;
                        }
                        else if (req.Status.Equals(TradeRequestStatus.DENY) || req.Status.Equals(TradeRequestStatus.CANCELBYAUTHOR))
                        {

                            userReq.Id = req.Id;
                            userReq.UserId = req.UserId;
                            userReq.Status = req.Status;
                            userReq.createdAt = req.CreateAt;
                            userReq.Name = user.Name;
                            userReq.SocialCredit = user.SocialCredit;
                            post.UserRequest = userReq;
                            post.isRequest = false;
                            post.CanRequest = true;
                        } else if (req.Status.Equals(TradeRequestStatus.CANCELBYUSER))
                        {
                            post.isRequest = false;
                            post.CanRequest = false;
                        }
                    } else
                    {
                        post.isRequest = false;
                        post.CanRequest = true;
                    }
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
        public async Task<ResultModel> CreatePostTrade(PostTradeCreateReqModel newPost)
        {
            DateTime now = DateTime.Now;
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(newPost.Token, "userid"));
            Guid postId = Guid.NewGuid();
            Guid petId = Guid.NewGuid();
            var user = await _userRepo.Get(userId);
            if (user.Status.Equals(UserStatus.VERIFYING))
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.Message = "You need verify your email before do this!";
                return result;
            }
            PostTradeAuthorModel author = new()
            {
                Id = user.Id,
                Name = user.Name,
                ImageUrl = user.Image,
                Phone = user.Phone
            };
            TblPost postTradeReq = new()
            {
                Id = postId,
                Type = PostingType.TRADING,
                Title = newPost.Title,
                UserId = userId,
                Status = TradingStatus.ACTIVE,
                IsProcessed = true,
                Content = newPost.Content,
                CreateAt = now,
                Amount = newPost.Amount,
                IsFree = newPost.isFree,
                Address = newPost.Address
            };
            try
            {
                var check = await _postRepo.GetPostTradingInProgressByUserId(userId);
                if(check.Count >= 3)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You can only create 3 trade post per time. Please check the previous post to continue";
                    return result;
                }
                _ = await _postRepo.Insert(postTradeReq);
                foreach (var attachement in newPost.Attachment)
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
                    Color = newPost.Color,
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
                    Color = newPost.Color
                };
                PostTradeResModel postResModel = new()
                {
                    Id = postId,
                    Author = author,
                    Content = newPost.Content,
                    Attachment = listAttachment,
                    Title = newPost.Title,
                    createdAt = now,
                    updatedAt = null,
                    Type = newPost.Type,
                    Amount = newPost.Amount,
                    Pet = newPet,
                    isFree = newPost.isFree,
                    Address = newPost.Address,
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
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
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
                    if(!string.IsNullOrEmpty(postReq.Title) && !post.Title.Equals(postReq.Title))
                    {
                        post.Title = postReq.Title;
                        tblPost.Title = postReq.Title;
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
                    if (!string.IsNullOrEmpty(postReq.Color) && !pet.Color.Equals(postReq.Color))
                    {
                        pet.Color = postReq.Color;
                        tblPet.Color = postReq.Color; 
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
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
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

        public async Task<ResultModel> ReportPost(PostReportModel postReq)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(postReq.token, "userid"));
            try
            {
                var post = await _postRepo.Get(postReq.postId);
                if(post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found";
                    return result;
                }
                TblReport newReport = new()
                {
                    UserId = userId,
                    PostId = postReq.postId,
                    Type = postReq.Type,
                    Reason = postReq.Reason,
                    Status = ReportingStatus.INPROGRESS,
                    IsProcessed = false,
                    CreateAt = now,
                };
                _ = await _reportRepo.Insert(newReport);
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

        public async Task<ResultModel> RequestTrading(PostTradeRequestReqModel reqRequest, string token)
        {
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            ResultModel result = new();
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
                var post = await _postRepo.Get(reqRequest.PostId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found!";
                    return result;
                }
                else if (!post.Type.Equals(PostingType.TRADING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "This is not Trading Post!";
                    return result;
                }
                else if (post.UserId.Equals(userId))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Author not available to request trading!";
                    return result;
                }
                var req = await _postTradeRequestRepo.GetRequestPostTrade(reqRequest.PostId, userId);
                if (req != null)
                {
                    if (req.Status.Equals(TradeRequestStatus.CANCELBYUSER))
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "You already cancelled this request!";
                        return result;
                    }
                    if (req.Status.Equals(TradeRequestStatus.ACCEPT) || req.Status.Equals(TradeRequestStatus.PENDING))
                    {
                        result.IsSuccess = false;
                        result.Code = 400;
                        result.Message = "You have already request!";
                        return result;
                    }
                }
                TblTradeRequest tradeRequest = new()
                {
                    PostId = reqRequest.PostId,
                    UserId = userId,
                    Status = TradeRequestStatus.PENDING,
                    CreateAt = now
                };
                if (post.Amount.Equals(-1))
                {
                    tradeRequest.Attachment = reqRequest.Attachments;
                }
                _ = await _postTradeRequestRepo.Insert(tradeRequest);
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

        public async Task<ResultModel> AcceptTrading(PostTradeProcessModel req, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
                var post = await _postRepo.Get(req.PostId);
                if(post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found";
                    return result;
                }
                if (!post.UserId.Equals(userId))
                {
                    result.IsSuccess = false;
                    result.Code = 403;
                    result.Message = "You do not have permission to do this!";
                    return result;
                }
                post.Status = TradingStatus.INPROGRESS;
                _ = await _postRepo.Update(post);
                var getReq = await _postTradeRequestRepo.Get(req.IdRequest);
                getReq.Status = TradeRequestStatus.ACCEPT;
                getReq.UpdateAt = now;
                _ = await _postTradeRequestRepo.Update(getReq);
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

        public async Task<ResultModel> DenyTrading(PostTradeProcessModel req, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
                var post = await _postRepo.Get(req.PostId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found";
                    return result;
                }
                if (!post.UserId.Equals(userId))
                {
                    result.IsSuccess = false;
                    result.Code = 403;
                    result.Message = "You do not have permission to do this!";
                    return result;
                }
                var getReq = await _postTradeRequestRepo.Get(req.IdRequest);
                getReq.Status = TradeRequestStatus.DENY;
                getReq.UpdateAt = now;
                _ = await _postTradeRequestRepo.Update(getReq);
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
        public async Task<ResultModel> DoneTradingForAuthor(PostTradeProcessModel req, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
                var post = await _postRepo.Get(req.PostId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found";
                    return result;
                }
                if (!post.UserId.Equals(userId))
                {
                    result.IsSuccess = false;
                    result.Code = 403;
                    result.Message = "You do not have permission to do this!";
                    return result;
                }
                if(post.Status.Equals(TradingStatus.INPROGRESS))
                {
                    post.Status = TradingStatus.WAITINGDONEBYAUTHOR;
                    _ = await _postRepo.Update(post);
                    result.IsSuccess = true;
                    result.Code = 200;
                }
                else if (post.Status.Equals(TradingStatus.WAITINGDONEBYUSER))
                {
                    post.Status = TradingStatus.DONE;
                    _ = await _postRepo.Update(post);
                    var getReq = await _postTradeRequestRepo.Get(req.IdRequest);
                    getReq.Status = TradeRequestStatus.SUCCESS;
                    getReq.UpdateAt = now;
                    _ = await _postTradeRequestRepo.Update(getReq);
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
        public async Task<ResultModel> DoneTradingForUser(PostTradeProcessModel req, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
                var post = await _postRepo.Get(req.PostId);
                var getReq = await _postTradeRequestRepo.Get(req.IdRequest);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found";
                    return result;
                }
                if (!getReq.UserId.Equals(userId))
                {
                    result.IsSuccess = false;
                    result.Code = 403;
                    result.Message = "You do not have permission to do this!";
                    return result;
                }
                if (post.Status.Equals(TradingStatus.INPROGRESS))
                {
                    post.Status = TradingStatus.WAITINGDONEBYUSER;
                    _ = await _postRepo.Update(post);
                    result.IsSuccess = true;
                    result.Code = 200;
                }
                else if (post.Status.Equals(TradingStatus.WAITINGDONEBYAUTHOR))
                {
                    post.Status = TradingStatus.DONE;
                    _ = await _postRepo.Update(post);
                    
                    getReq.Status = TradeRequestStatus.SUCCESS;
                    getReq.UpdateAt = now;
                    _ = await _postTradeRequestRepo.Update(getReq);
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
        public async Task<ResultModel> CancelTrading(PostTradeProcessModel req, string token)
        {
            ResultModel result = new();
            DateTime now = DateTime.Now;
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            var user = await _userRepo.Get(userId);
            try
            {
                if (user.Status.Equals(UserStatus.VERIFYING))
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "You need verify your email before do this!";
                    return result;
                }
                var post = await _postRepo.Get(req.PostId);
                if (post == null)
                {
                    result.IsSuccess = false;
                    result.Code = 400;
                    result.Message = "Post not found";
                    return result;
                }
                if (!post.UserId.Equals(userId))
                {
                    user.SocialCredit -= 10;
                    _ = await _userRepo.Update(user);
                    post.Status = TradingStatus.ACTIVE;
                    _ = await _postRepo.Update(post);
                    var getReq = await _postTradeRequestRepo.Get(req.IdRequest);
                    getReq.Status = TradeRequestStatus.CANCELBYUSER;
                    _ = await _postTradeRequestRepo.Update(getReq);
                    result.IsSuccess = true;
                    result.Code = 200;
                } else
                {
                    var postTrading = await _postRepo.GetListPostTradingByUserId(userId);
                    List<TblTradeRequest> checkReq = new();
                    foreach (var p in postTrading)
                    {
                        var reqCancel = await _postTradeRequestRepo.GetListRequestCancelByAuthor(p.Id);
                        foreach (var r in reqCancel)
                        {
                            checkReq.Add(r);
                        }
                    }
                    if((checkReq.Count + 1) % 5 == 0)
                    {
                        user.SocialCredit -= 15;
                        _ = await _userRepo.Update(user);
                    }
                    post.Status = TradingStatus.ACTIVE;
                    _ = await _postRepo.Update(post);
                    var getReq = await _postTradeRequestRepo.Get(req.IdRequest);
                    getReq.Status = TradeRequestStatus.CANCELBYAUTHOR;
                    _ = await _postTradeRequestRepo.Update(getReq);
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
        public async Task<ResultModel> GetAllTradePostsTitle()
        {
            ResultModel result = new();
            try
            {
                List<PostTradeTitleModel> poststradetitle = await _postRepo.GetAllTradePostsTitle();
                result.Code = 200;
                result.IsSuccess = true;
                result.Data = poststradetitle;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Code = 400;
                result.ResponseFailed = e.InnerException != null ? e.InnerException.Message + "\n" + e.StackTrace : e.Message + "\n" + e.StackTrace;
            }
            return result;
        }
        public async Task<ResultModel> GetListPostTradeByUserId(Guid id, string token)
        {
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            Guid roleId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var post = await _postRepo.GetListPostTradeResModelByUserId(id);
                var user = await _userRepo.Get(userId);
                if (post == null || post.Count <= 0)
                {
                    result.IsSuccess = true;
                    result.Code = 200;
                    return result;
                }
                if (id.Equals(userId))
                {
                    List<PostTradeAuthorResModel> listPostRes = new();
                    foreach (var p in post)
                    {
                        var req = await _postTradeRequestRepo.GetListRequestPostTradeByPostId(p.Id);
                        foreach (var r in req)
                        {
                            var u = await _userRepo.Get(r.UserId);
                            r.Name = u.Name;
                            r.SocialCredit = u.SocialCredit;
                        }
                        PostTradeAuthorResModel postRes = new()
                        {
                            Id = p.Id,
                            Author = p.Author,
                            Title = p.Title,
                            Content = p.Content,
                            Attachment = p.Attachment,
                            Amount = p.Amount,
                            Pet = p.Pet,
                            Type = p.Type,
                            createdAt = p.createdAt,
                            UserRequest = req,
                            isFree = p.isFree,
                            isTrading = p.isTrading
                        };
                        listPostRes.Add(postRes);
                    }
                    result.IsSuccess = true;
                    result.Data = listPostRes;
                    result.Code = 200;
                }
                else if (!id.Equals(userId))
                {
                    foreach (var p in post)
                    {
                        var req = await _postTradeRequestRepo.GetRequestPostTrade(p.Id, userId);
                        PostTradeUserRequestModel userReq = new();
                        if (req != null)
                        {
                            if (req.Status.Equals(TradeRequestStatus.PENDING) || req.Status.Equals(TradeRequestStatus.ACCEPT))
                            {
                                userReq.Id = req.Id;
                                userReq.UserId = req.UserId;
                                userReq.Status = req.Status;
                                userReq.createdAt = req.CreateAt;
                                userReq.Name = user.Name;
                                userReq.SocialCredit = user.SocialCredit;
                                p.UserRequest = userReq;
                                p.isRequest = true;
                                p.CanRequest = false;
                            }
                            else if (req.Status.Equals(TradeRequestStatus.DENY) || req.Status.Equals(TradeRequestStatus.CANCELBYAUTHOR))
                            {

                                userReq.Id = req.Id;
                                userReq.UserId = req.UserId;
                                userReq.Status = req.Status;
                                userReq.createdAt = req.CreateAt;
                                userReq.Name = user.Name;
                                userReq.SocialCredit = user.SocialCredit;
                                p.UserRequest = userReq;
                                p.isRequest = false;
                                p.CanRequest = true;
                            }
                            else if (req.Status.Equals(TradeRequestStatus.CANCELBYUSER))
                            {
                                p.isRequest = false;
                                p.CanRequest = false;
                            }
                        }
                        else
                        {
                            p.isRequest = false;
                            p.CanRequest = true;
                        }
                    }
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

        public async Task<ResultModel> GetListPostTradeRequested(string token)
        {
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var post = await _postTradeRequestRepo.GetListPostTradeRequested(userId);
                result.IsSuccess = true;
                result.Data = post;
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
        public async Task<ResultModel> GetListPostTradeHistory(string token)
        {
            ResultModel result = new();
            Guid userId = new Guid(_userAuthentication.decodeToken(token, "userid"));
            try
            {
                var post = await _postTradeRequestRepo.GetListPostTradeHistory(userId);
                result.IsSuccess = true;
                result.Data = post;
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
