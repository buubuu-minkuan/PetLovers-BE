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
    [Route("manage/post/")]
    [Authorize]
    [ApiController]
    public class PostManageController : Controller
    {
        private readonly IPostServices _post;

        public PostManageController(IPostServices post)
        {
            _post = post;
        }

        [HttpGet("pending-post")]
        public async Task<IActionResult> GetAllPendingPost()
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _post.GetAllPendingPost(token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("approve-post")]
        public async Task<IActionResult> ApprovePost([FromBody] PostReqModel Post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Post.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.ApprovePosting(Post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("refuse-post")]
        public async Task<IActionResult> RefusePost([FromBody] PostReqModel Post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Post.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.RefusePosting(Post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}