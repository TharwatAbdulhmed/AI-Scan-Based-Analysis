using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.models.AuthModles;
using Microsoft.AspNetCore.Identity;

namespace DomainLayer.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterModel model);
        Task<AuthResponse> LoginAsync(LoginModel model);
    }
}
