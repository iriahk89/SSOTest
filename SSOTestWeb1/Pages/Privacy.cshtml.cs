using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SSOTestWeb1.Pages
{
    public class PrivacyModel : PageModel
    {
        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public bool IsFromCache { get; set; }

        public void OnGet()
        {
            //read cookie from Request object  
            Token = Request.Cookies["JWT"];
            if (!string.IsNullOrEmpty(Token))
            {
                IsFromCache = bool.Parse(Request.Cookies["IsFromCache"]);
            }
        }
    }
}