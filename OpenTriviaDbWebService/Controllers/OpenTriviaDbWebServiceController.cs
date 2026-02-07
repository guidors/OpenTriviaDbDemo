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
                    return BadRequest($"It was not possible to retrieve {model.NumberOfQuestions} for given options. Please retry with different options or reduce number of questions.");
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

        /// <summary>
        /// Get categories from OpenTriviaDb. The connector caches result. Also avoiding CORS issues
        /// </summary>
        /// <returns></returns>
        [HttpGet("get_categories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await openTriviaDbConnector.GetCategoriesAsync();

                if (categories == null)
                {
                    return StatusCode(500, "Failed to retrieve categories from Open Trivia Database API.");
                }

                logger.LogInformation("Categories retrieved from API.");
                return Ok(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting categories.");
                return StatusCode(500, $"{ex.Message}");
            }
        }
    }
}
