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
using Business.Services.VerifyServices;
using Microsoft.AspNetCore.Authorization;
using Data.Models.OTPVerifyModel;
using Business.Services.SearchServices;

namespace API.Controllers
{
    [Route("search/")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly ISearchServices _search;

        public SearchController(ISearchServices search)
        {
            _search = search;
        }

        [HttpGet("search-main")]
        [Authorize]
        public async Task<IActionResult> Search(string keyword)
        {
            string token = Request.Headers["Authorization"].ToString().Split(" ")[1];
            Data.Models.ResultModel.ResultModel result = await _search.Search(keyword, token);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}