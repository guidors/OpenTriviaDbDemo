using Microsoft.Extensions.Options;
using OpenTriviaDbWebService.Models;
using OpenTriviaDbWebService.Options;

namespace OpenTriviaDbWebService.Infrastructure
{
    /// <summary>
    /// Connector class for interacting with the Open Trivia Database API. 
    /// This class is responsible for handling API requests, managing tokens, and processing responses from the Open Trivia Database.
    /// 
    /// Response Codes
    ///
    /// The API appends a "Response Code" to each API Call to help tell developers what the API is doing.
    /// 
    ///    Code 0: Success Returned results successfully.
    ///    Code 1: No Results Could not return results.The API doesn't have enough questions for your query. (Ex. Asking for 50 Questions in a Category that only has 20.)
    ///    Code 2: Invalid Parameter Contains an invalid parameter. Arguements passed in aren't valid. (Ex. Amount = Five)
    ///    Code 3: Token Not Found Session Token does not exist.
    ///    Code 4: Token Empty Session Token has returned all possible questions for the specified query.Resetting the Token is necessary.
    ///    Code 5: Rate Limit Too many requests have occurred.Each IP can only access the API once every 5 seconds.
    /// 
    ///    Code 6: Internal Server Error Something went wrong on our end. (This is never returned by the API, but is used when something goes wrong with the API itself.)
    /// </summary>
    public class OpenTriviaDbConnector(ILogger<OpenTriviaDbConnector> logger, IOptions<OpenTriviaDbOptions> options)
    {
        internal string? _token;
        private static HttpClient _httpClient = new();
        private readonly OpenTriviaDbApiResponse ErrorResult = new() { ResponseCode = 6, Results = [] };
        private readonly SemaphoreSlim _semaphore = new(1, 1);
        private TriviaCategories? _triviaCategories = null;

        /// <summary>
        /// For unit testing purposes
        /// </summary>
        /// <param name="httpMessageHandler"></param>
        internal static void SetHttpMessageHandler(HttpMessageHandler httpMessageHandler)
        {
            _httpClient = new(httpMessageHandler);
        }

        public async Task<OpenTriviaDbApiResponse> GetQuizAsync(QuizRequest quizModel)
        {
            try
            {
                // let's ensure that only one request is made to the API at a time, to avoid hitting the rate limit.
                await _semaphore.WaitAsync();
                return await GetQuizAsync(quizModel, false, 0);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private async Task<OpenTriviaDbApiResponse> GetQuizAsync(QuizRequest quizModel, bool isRetry, int recursiveCounter)
        {
            OpenTriviaDbApiResponse? response = await GetResponseFromBackendAsync(quizModel);

            if (response == null)
            {
                return ErrorResult;
            }

            response = await ProcessResponseAsync(response!, quizModel, isRetry, recursiveCounter);

            return response;
        }

        private async Task<OpenTriviaDbApiResponse?> GetResponseFromBackendAsync(QuizRequest quizModel)
        {
            OpenTriviaDbApiResponse? response = null;

            try
            {
                await RequestTokenAsync();

                using var httpResponse = await _httpClient.GetAsync($"{options.Value.ApiUrl}?{quizModel}&token={_token}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    logger.LogWarning("Open Trivia Database API returned HTTP {StatusCode}: {ReasonPhrase}",
                        httpResponse.StatusCode, httpResponse.ReasonPhrase);
                }

                // Ook voor HttpStatusCode.TooManyRequests en HttpStatusCode.BadRequest wordt er een json terug gegeven.
                response = await httpResponse.Content.ReadFromJsonAsync<OpenTriviaDbApiResponse>()
                    ?? throw new OpenTriviaDbConnectorException("Failed to retrieve quiz from Open Trivia Database API: HTTP {httpResponse.StatusCode}: {httpResponse.ReasonPhrase}");
            }
            catch (Exception e)
            {
                logger.LogError(e, "Unexpected error occurred: {Message}", e.Message);
            }
            return response;
        }

        private async Task<OpenTriviaDbApiResponse> ProcessResponseAsync(OpenTriviaDbApiResponse response, QuizRequest quizModel, bool isRetry, int recursiveCounter)
        {
            try
            {
                if (recursiveCounter == 3 && // Allow a request for a new token and a timeout
                    !(response.ResponseCode == 0 && response.Results.Count > 0)) // Pass when we have a good result
                {
                    throw new OpenTriviaDbConnectorException("Too many recursive tries to get a result.");
                }

                switch (response.ResponseCode)
                {
                    case 0: // Success
                        if (response.Results.Count == 0)
                        {
                            if (isRetry)
                            {
                                throw new OpenTriviaDbConnectorException("Received empty results from Open Trivia Database API after retrying.");
                            }
                            await Task.Delay(5500); // Avoid the described rate limit
                            response = await GetQuizAsync(quizModel, true, ++recursiveCounter);
                        }
                        logger.LogInformation("Open Trivia Database API: Successfully retrieved quiz.");
                        break;
                    case 1: // No Results
                        throw new OpenTriviaDbConnectorException($"No results could be returned from the Open Trivia Database API for the given query.");
                    case 2: // Invalid Parameter
                        throw new OpenTriviaDbConnectorException("Invalid parameter(s) provided to the Open Trivia Database API.");
                    case 3: // Token Not Found 
                    case 4: // Token Empty
                        await RequestTokenAsync(true);
                        response = await GetQuizAsync(quizModel, false, ++recursiveCounter);
                        break;
                    case 5: // Rate Limit
                        await Task.Delay(5500); // Avoid the described rate limit
                        response = await GetQuizAsync(quizModel, true, ++recursiveCounter);
                        break;
                    default:
                        throw new OpenTriviaDbConnectorException($"Unexpected response code {response.ResponseCode} from the Open Trivia Database API.");
                }
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting a quiz from the Open Trivia Database API.");
            }
            return ErrorResult;
        }

        internal async Task RequestTokenAsync(bool resetToken = false)
        {
            OpenTriviaDbApiSessionRequest? response;

            try
            {
                if (_token != null && !resetToken)
                {
                    logger.LogInformation("Using existing token for Open Trivia Database API: {Token}", _token);
                    return;
                }

                string command = (resetToken && (_token != null)) ? $"reset&token={_token}" : "request";

                using var httpResponse = await _httpClient.GetAsync($"{options.Value.SessionUrl}?command={command}");

                if (!httpResponse.IsSuccessStatusCode)
                {
                    logger.LogWarning("Open Trivia Database API Session Request returned HTTP {StatusCode}: {ReasonPhrase}",
                        httpResponse.StatusCode, httpResponse.ReasonPhrase);
                }

                response = await httpResponse.Content.ReadFromJsonAsync<OpenTriviaDbApiSessionRequest>()
                    ?? throw new OpenTriviaDbConnectorException("Failed to retrieve token from Open Trivia Database API: HTTP {httpResponse.StatusCode}: {httpResponse.ReasonPhrase}");

                switch (response.ResponseCode)
                {
                    case 0:
                        _token = response.Token;
                        logger.LogInformation("Open Trivia Database API: {ResponseMessage}", response.ResponseMessage);
                        return;
                    case 4:
                        await RequestTokenAsync(true);
                        return;
                    case 5:
                        await Task.Delay(5500); // Avoid the described rate limit
                        await RequestTokenAsync(resetToken);
                        return;

                    default:
                        throw new OpenTriviaDbConnectorException($"Unexpected response code {response.ResponseCode} when requesting token from Open Trivia Database API.");
                }
            }
            catch (OpenTriviaDbConnectorException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while requesting a token from the Open Trivia Database API.");
                throw new OpenTriviaDbConnectorException("Failed to request token from Open Trivia Database API.", ex);
            }
        }

        public async Task<TriviaCategories?> GetCategoriesAsync()
        {
            try
            {
                if (_triviaCategories != null)
                    return _triviaCategories;

                using var httpResponse = await _httpClient.GetAsync(options.Value.CategoryUrl);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    logger.LogWarning("Open Trivia Database Categories API returned HTTP {StatusCode}: {ReasonPhrase}",
                        httpResponse.StatusCode, httpResponse.ReasonPhrase);
                    return null;
                }

                _triviaCategories = await httpResponse.Content.ReadFromJsonAsync<TriviaCategories>();
                logger.LogInformation("Open Trivia Database Categories API: Successfully retrieved {Count} categories.", _triviaCategories?.Categories?.Count ?? 0);
                return _triviaCategories;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while getting categories from the Open Trivia Database API.");
                return null;
            }
        }
    }

    [Serializable]
    public class OpenTriviaDbConnectorException : Exception
    {
        public OpenTriviaDbConnectorException()
        {
        }

        public OpenTriviaDbConnectorException(string? message) : base(message)
        {
        }

        public OpenTriviaDbConnectorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
