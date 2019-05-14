using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOTestAPI.Models
{
    public class AuthenticateResultModel
    {
        public string Token { get; set; }

        public bool IsFromCache { get; set; }
    }
}
