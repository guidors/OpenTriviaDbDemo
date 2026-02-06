using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Models;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OpenTriviaDbWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenTriviaDbWebServiceController(ILogger<OpenTriviaDbWebServiceController> logger, OpenTriviaDbConnector openTriviaDbConnector) : ControllerBase
    {
        // Mapping of client tokens to opentdb.com tokens
        private static readonly ConcurrentDictionary<string, string> _tokens = [];

        private const string SigningCredentials = "7E0B0424-4398-47A5-A6A2-9944556F4896";

        [HttpPost("quiz")]
        public async Task<IActionResult> GetQuiz([FromBody] QuizModel model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid quiz model.");
                }

                var result = await openTriviaDbConnector.GetQuizAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting quiz.");
                return StatusCode(500, "An error occurred while getting the quiz.");
            }
        }

        private static string GenerateJwtToken(string sessionId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SigningCredentials);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                    [new Claim("sessionId", sessionId)]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenId = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(tokenId);
        }
    }
}
