using Microsoft.AspNetCore.Mvc;
using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Models;
using System.Collections.Concurrent;

namespace OpenTriviaDbWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpenTriviaDbWebServiceController(ILogger<OpenTriviaDbWebServiceController> logger, OpenTriviaDbConnector openTriviaDbConnector) : ControllerBase
    {
        // Mapping of client tokens to opentdb.com quiz
        private static readonly ConcurrentDictionary<string, QuizScore> _tokens = [];

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
                var sessionToken = Guid.NewGuid().ToString();
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

        [HttpPost("check_answers")]
        public async Task<IActionResult> CheckAnswers([FromBody] AnswerRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid answer request.");
            }

            // Check if the token is valid
            if (!_tokens.TryGetValue(request.Token, out var quizScore))
            {
                return BadRequest("Invalid session token.");
            }

            // Get the answer response and update the score
            var answerResponse = quizScore.GetAnswerResponseAndUpdateScore(request);

            return Ok(answerResponse);
        }
    }
}
