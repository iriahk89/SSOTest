using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SSOTestWeb1.Pages
{
    public class IndexModel : PageModel
    {

        [BindProperty]
        public string Token { get; set; }

        [BindProperty]
        public string Username { get; set; }

        public void OnGet()
        {
            //read cookie from Request object  
            Token = Request.Cookies["JWT"];

            if (string.IsNullOrEmpty(Token))
            {
                Username = "Guest";
            } 
            else
            {
                Username = Request.Cookies["Username"];
            }
        }

        public IActionResult OnPost()
        {
            Response.Cookies.Delete("JWT");
            Response.Cookies.Delete("Username");
            Response.Cookies.Delete("IsFromCache");
            return RedirectToPage("Index");
        }
    }
}
