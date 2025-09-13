using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Interfaces;
using DomainLayer.models.AuthModles;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace RepositoryLayer
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterModel model)
        {
            // Check if the email already exists
            var emailExists = await _userManager.FindByEmailAsync(model.Email);
            if (emailExists != null)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Registration failed.",
                    Errors = new List<string> { "Email already exists." }
                };
            }

            // Create the new user
            var user = new AppUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber // Add this line
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "User registration failed.",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            // Generate a JWT token for the newly registered user
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddDays(double.Parse(jwtSettings["DurationInDays"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponse
            {
                Success = true,
                Message = "User registered successfully.",
                Data = new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                }
            };
        }
        public async Task<AuthResponse> LoginAsync(LoginModel model)
        {
            // Find the user by email
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Check if the user exists and the password is correct
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return new AuthResponse
                {
                    Success = false,
                    Message = "Login failed.",
                    Errors = new List<string> { "Invalid email or password." }
                };
            }

            // Generate claims for the JWT token
            var authClaims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            // Generate the JWT token
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecurityKey"]);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: authClaims,
                expires: DateTime.UtcNow.AddDays(double.Parse(jwtSettings["DurationInDays"])),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256)
            );

            return new AuthResponse
            {
                Success = true,
                Message = "Login successful.",
                Data = new
                {
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = token.ValidTo
                },
                Errors = new List<string>() // No errors for successful login
            };
        }

    }
}
