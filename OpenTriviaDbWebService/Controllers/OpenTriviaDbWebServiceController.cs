using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OpenTriviaDbWebServiceController(ILogger<OpenTriviaDbWebServiceController> logger, OpenTriviaDbConnector openTriviaDbConnector) : ControllerBase
    {
        // Mapping of client tokens to opentdb.com quiz
        private static readonly ConcurrentDictionary<string, QuizScore> _tokens = [];

        private const string SigningCredentials = "7E0B0424-4398-47A5-A6A2-9944556F4896";

        [AllowAnonymous]
        [HttpPost("get_quiz")]
        public async Task<IActionResult> GetQuiz([FromBody] QuizRequest model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest("Invalid quiz model.");
                }

                await model.ValidateAsync();

                var result = await openTriviaDbConnector.GetQuizAsync(model);

                if (result == null || result.ResponseCode != 0 || result.Results == null || result.Results.Count == 0)
                {
                    return BadRequest("Failed to retrieve quiz.");
                }

                // Generate a session token for the client and store results
                var sessionToken = GenerateJwtToken(Guid.NewGuid().ToString());
                Response.Headers["X-Session-Token"] = sessionToken;
                _tokens[sessionToken] = new(result);

                QuizResponse quizResponse = new(sessionToken, result);

                return Ok(quizResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting quiz.");
                return StatusCode(500, $"{ex.Message}");
            }
        }

        /*
         *     GET /questions
                POST /checkanswers
        */

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
