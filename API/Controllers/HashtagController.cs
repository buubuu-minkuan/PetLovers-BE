using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Business.Services.UserServices;
using Data.Entities;
using Data.Models.UserModel;
using Microsoft.AspNetCore.Authorization;
using Business.Services.HashtagServices;

namespace API.Controllers
{
    [ApiController]
    [Route("hashtag/")]
    public class HashtagController : Controller
    {
        private readonly IHashtagServices _hashtagService;

        public HashtagController(IHashtagServices hashtagService)
        {
            _hashtagService = hashtagService;
        }
    }
}