using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Identity.Dto
{
    public class UserLoginDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
