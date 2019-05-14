using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOTestAPI.Models
{
    public class AuthenticateResultModel
    {
        public string AccessToken { get; set; }

        public int ExpireInSeconds { get; set; }

        public long UserId { get; set; }

        public bool IsValid { get; set; }

        public bool IsFromCache { get; set; }
    }
}
