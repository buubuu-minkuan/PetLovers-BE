using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;

namespace API.Controllers
{
    [ApiController]
    [Route("user/")]
    public class UsersController : Controller
    {
        private readonly IUserServices _user;

        public UsersController(IUserServices user)
        {
            _user = user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] string name, [FromBody] string email, [FromBody] string username, [FromBody] string password, [FromBody] string phone)
        {
            Data.Models.ResultModel.ResultModel result = await _user.Register(name, email, username, password, phone);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] string username, [FromBody] string password)
        {
            Data.Models.ResultModel.ResultModel result = await _user.Login(username, password);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("read-jwt")]
        public IActionResult ReadJwt([FromBody] string jwttoken)
        {
            Data.Models.ResultModel.ResultModel result = _user.ReadJWT(jwttoken);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:Guid}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            Data.Models.ResultModel.ResultModel result = await _user.GetUser(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}