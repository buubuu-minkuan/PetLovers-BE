using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Business.Services.ReactionServices;
using Data.Models.CommentModel;
using Microsoft.AspNetCore.Authorization;
using Data.Models.FeelingModel;

namespace API.Controllers
{
    [Route("feeling/")]
    [Authorize]
    [ApiController]
    public class FeelingController : Controller
    {
        private readonly IReactionServices _feeling;

        public FeelingController(IReactionServices feeling)
        {
            _feeling = feeling;
        }

        [HttpPost("create-feeling")]
        public async Task<IActionResult> CreateFeeling([FromBody] Guid postId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _feeling.CreateFeeling(postId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete-feeling")]
        public async Task<IActionResult> DeleteFeeling([FromBody] Guid postId)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _feeling.RemoveFeeling(postId, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}