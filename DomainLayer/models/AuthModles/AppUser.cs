using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DomainLayer.models.AuthModles
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; } // Example: Add a full name property
    }
}
