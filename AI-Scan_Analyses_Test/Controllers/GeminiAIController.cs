using AI_Scan_Analyses_Test.ChatBot.Client;
using AI_Scan_Analyses_Test.ChatBot.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AI_Scan_Analyses_Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeminiAIController : ControllerBase
    {
        private readonly GeminiApiClient _geminiApiClient;

        public GeminiAIController(GeminiApiClient geminiApiClient)
        {
            _geminiApiClient = geminiApiClient;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContent([FromBody] PromptRequest request)
         {
            try
            {
                string response = await _geminiApiClient.GenerateContentAsync(request.Prompt);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
