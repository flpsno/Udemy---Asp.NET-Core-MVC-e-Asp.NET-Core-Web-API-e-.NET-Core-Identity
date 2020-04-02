using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Domain
{
    public class User : IdentityUser<int>
    {
        public string NomeCompleto { get; set; }
        public string Member { get; set; } = "Member";
        public string OrgId { get; set; }

        public List<UserRole> UserRoles { get; set; }

    }
}
