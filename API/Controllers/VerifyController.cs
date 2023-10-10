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
using Business.Services.EmailServices;

namespace API.Controllers
{
    [Route("email/")]
    [ApiController]
    public class EmailController : Controller
    {
        private readonly IEmailServices _email;

        public EmailController(IEmailServices email)
        {
            _email = email;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail(string email)
        {
            Data.Models.ResultModel.ResultModel result = await _email.SendMail(email);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}