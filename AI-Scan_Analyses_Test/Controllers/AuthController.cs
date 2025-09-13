using DomainLayer.models.AuthModles;
using Microsoft.AspNetCore.Mvc;
using DomainLayer.Interfaces;
using System.Threading.Tasks;

namespace AI_Scan_Analyses_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);

            // If there are errors, return them with a 400 Bad Request status
            if (result.Errors != null && result.Errors.Count > 0)
                return BadRequest(result);

            // Otherwise, return the successful response
            return Ok(result);
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(model);

            // If there are errors, return them with a 400 Bad Request status
            if (!result.Success)
                return BadRequest(result);

            // Otherwise, return the successful response
            return Ok(result);
        }
    }
}