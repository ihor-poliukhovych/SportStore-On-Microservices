﻿using System.Threading.Tasks;
using Identity.Api.Models;
using Identity.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/identity")]
    public class IdentityController : Controller
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;

        public IdentityController(IIdentityService identityService, IJwtTokenService jwtTokenService)
        {
            _identityService = identityService;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token(LoginModel model)
        {
            var identity = await _identityService.GetIdentity(model);
            if (identity == null)
                return BadRequest("invalid username or password");

            return Ok(new
            {
                token = _jwtTokenService.GenerateUserToken(identity.Claims),
                username = identity.Name
            });
        }
    }
}