﻿using System;
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
    [Route("postTrade/")]
    [Authorize]
    [ApiController]
    public class PostTradeController : Controller
    {
        private readonly IPostServices _post;

        public PostTradeController(IPostServices post)
        {
            _post = post;
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetPostTrade(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _post.GetPostTradeById(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }


        [HttpPost("create-post-trade")]
        public async Task<IActionResult> CreatePostTrade([FromBody] PostTradeCreateReqModel newPost)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            newPost.Token = token;
            Data.Models.ResultModel.ResultModel result = await _post.CreatePostTrade(newPost);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update-post-trade")]
        public async Task<IActionResult> UpdatePostTrade([FromBody] PostTradeUpdateReqModel post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            post.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.UpdatePostTrade(post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-post-trade")]
        public async Task<IActionResult> DeletePostTrade([FromBody] PostDeleteReqModel post)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            post.token = token;
            Data.Models.ResultModel.ResultModel result = await _post.DeletePostTrade(post);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}