using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SSOTestWeb1.Model;

namespace SSOTestWeb1.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty] // Bind on Post
        public LoginData loginData { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            AuthenticateResultModel result = new AuthenticateResultModel();
            try
            {
                string baseUrl = "http://localhost:5000";

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);

                string stringData = JsonConvert.SerializeObject(loginData);
                var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync("/api/user/authenticate", contentData).Result;
                string responseString = response.Content.ReadAsStringAsync().Result;

                result = JsonConvert.DeserializeObject<AuthenticateResultModel>(responseString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (string.IsNullOrEmpty(result.Token))
            {
                ModelState.AddModelError("", "username or password is invalid");
                return Page();
            }
            else
            {
                SetCookie("JWT", result.Token, 15);
                SetCookie("Username", loginData.Username, 15);
                SetCookie("IsFromCache", result.IsFromCache.ToString(), 15);
                return RedirectToPage("Index");
            }
        }

        /// <summary>  
        /// set the cookie  
        /// </summary>  
        /// <param name="key">key (unique indentifier)</param>  
        /// <param name="value">value to store in cookie object</param>  
        /// <param name="expireTime">expiration time</param>  
        public void SetCookie(string key, string value, int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            Response.Cookies.Append(key, value, option);
        }
    }

    public class LoginData
    {
        [Required]
        public string Username { get; set; }

        [Required, DataType(DataType.Password)]
        public string Password { get; set; }
    }
}