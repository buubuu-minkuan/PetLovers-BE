using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Business.Services.CommentServices;
using Data.Models.CommentModel;

namespace API.Controllers
{
    [Route("comment/")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentServices _comment;

        public CommentController(ICommentServices comment)
        {
            _comment = comment;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(Guid id)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _comment.GetCommentById(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("fetch-post")]
        public async Task<IActionResult> GetPostComments(Guid postId)
        {
            Data.Models.ResultModel.ResultModel result = await _comment.GetCommentsForPost(postId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("create-comment")]
        public async Task<IActionResult> CreateComment([FromBody] CommentCreateResModel newComment)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            newComment.token = token;
            Data.Models.ResultModel.ResultModel result = await _comment.CreateComment(newComment);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
    }