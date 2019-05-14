using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
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
            try
            {
                UserModel user = null;
                AuthenticateResultModel model = new AuthenticateResultModel()
                {
                    ExpireInSeconds = 600,
                    IsValid = false,
                    IsFromCache = false
                };
                if (!_cache.TryGetValue(userInput.Username, out user))
                {
                    user = _repo.Authenticate(userInput);
                    if (user.Id > 0)
                    {
                        var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(5)); //5 Minutes Expiry
                        _cache.Set(userInput.Username, user, cacheEntryOptions);

                        model.AccessToken = CreateAccessToken();
                        model.UserId = user.Id;
                        model.IsValid = true;
                    }
                }
                else
                {
                    if (userInput.Password == user.Password)
                    {
                        model.AccessToken = CreateAccessToken();
                        model.UserId = user.Id;
                        model.IsValid = true;
                        model.IsFromCache = true;
                    }
                }
                return Ok(model);
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
    }
}