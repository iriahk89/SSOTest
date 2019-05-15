using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using SSOTestWeb1.Model;

namespace SSOTestWeb1.Pages
{
    public class RegisterModel : PageModel
    {
        [BindProperty] // Bind on Post
        public UserModel user { get; set; }
        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string baseUrl = "http://localhost:5000";

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(baseUrl);
                    var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Add(contentType);

                    string stringData = JsonConvert.SerializeObject(user);
                    var contentData = new StringContent(stringData, System.Text.Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync("/api/user/register", contentData).Result;
                    string responseString = response.Content.ReadAsStringAsync().Result;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError("", "username or password is blank");
                return Page();
            }
        }
    }
}