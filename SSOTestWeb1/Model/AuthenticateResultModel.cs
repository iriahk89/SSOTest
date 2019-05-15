using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SSOTestWeb1.Model
{
    public class AuthenticateResultModel
    {
        public string Token { get; set; }

        public bool IsFromCache { get; set; }
    }
}
