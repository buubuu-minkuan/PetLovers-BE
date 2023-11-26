using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Business.Services.PostServices;
using Microsoft.AspNetCore.Authorization;
using Data.Models.PostModel;

namespace API.Controllers
{
    [Route("post/")]
    [Authorize]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IPostServices _post;

        public PostController(IPostServices post)
        {
            _post = post;
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPost(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.GetPostById(id, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("pending-post")]
        public async Task<IActionResult> GetUserPendingPost()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.GetUserPendingPost(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("news-feed")]
        public async Task<IActionResult> GetNewsFeed()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.GetNewsFeed(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create-post")]
        public async Task<IActionResult> CreatePost([FromBody] PostCreateReqModel newPost)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            newPost.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.CreatePost(newPost);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("store-post")]
        public async Task<IActionResult> StorePost([FromBody] Guid postId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.StorePost(postId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-post")]
        public async Task<IActionResult> UpdatePost([FromBody] PostUpdateReqModel post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            post.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.UpdatePost(post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("remove-store-post")]
        public async Task<IActionResult> RemoveStorePost([FromBody] Guid postId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.RemoveStorePost(postId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-post")]
        public async Task<IActionResult> DeletePost([FromBody] PostReqModel post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            post.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.DeletePost(post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpPost("report-post")]
        public async Task<IActionResult> ReportPost([FromBody] PostReportModel Post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.ReportPost(Post, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("list-stored-post")]
        public async Task<IActionResult> GetListStoredPost()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.GetListStoredPost(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}