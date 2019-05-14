using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SSOTestAPI.Models;
using SSOTestAPI.Repositories;

namespace SSOTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private UserRepository _repo;
        private IMemoryCache _cache;

        public UserController(IConfiguration config, IMemoryCache memoryCache)
        {
            _repo = new UserRepository(config);
            _cache = memoryCache;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]UserModel user)
        {
            try
            {
                _repo.Register(user);
                return Ok("User registered");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]UserModel userInput)
        {
            bool isCache = false, isValid = false;
            try
            {
                UserModel user = null;
                if (!_cache.TryGetValue(userInput.Username, out user))
                {
                    user = _repo.Authenticate(userInput);
                    if (user.Id > 0)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(5)); //5 Minutes Expiry
                        _cache.Set(userInput.Username, user, cacheEntryOptions);
                        isValid = true;
                    }
                }
                else
                {
                    if (userInput.Password == user.Password)
                    {
                        isCache = isValid= true;
                    }
                }
                if (!isValid) return Unauthorized();
                else
                {
                    return Ok(new AuthenticateResultModel
                    {
                        Token = CreateJWT(user),
                        IsFromCache = isCache
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private string CreateAccessToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        private string CreateJWT(UserModel user)
        {
            var claim = new[]
               {
                new Claim(ClaimTypes.Name, user.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SSOSiteCoreTest@14052019"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: "http://localhost:5000",
                audience: "http://localhost:5002",
                claim,
                expires: DateTime.Now.AddMinutes(5), //5 Minutes Expiration
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}