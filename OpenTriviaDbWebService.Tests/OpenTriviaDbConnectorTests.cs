using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq; // Niet meer open source :-( https://dariusz-wozniak.github.io/fossed/
using Moq.Protected;
using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Models;
using OpenTriviaDbWebService.Options;
using Shouldly;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;

namespace OpenTriviaDbWebService.Tests
{
    public class OpenTriviaDbConnectorTests
    {
        private readonly ITestOutputHelper _output;
        private readonly Mock<ILogger<OpenTriviaDbConnector>> _mockLogger;
        private readonly Mock<IOptions<OpenTriviaDbOptions>> _mockOptions;
        private readonly OpenTriviaDbOptions _options;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;

        public OpenTriviaDbConnectorTests(ITestOutputHelper output)
        {
            _output = output;
            _mockLogger = new Mock<ILogger<OpenTriviaDbConnector>>();
            _mockOptions = new Mock<IOptions<OpenTriviaDbOptions>>();
            _options = new OpenTriviaDbOptions
            {
                ApiUrl = "https://opentdb.com/api.php",
                SessionUrl = "https://opentdb.com/api_token.php"
            };
            _mockOptions.Setup(x => x.Value).Returns(_options);
            _mockHttpHandler = new Mock<HttpMessageHandler>();
        }

        private OpenTriviaDbConnector CreateConnector(bool withHttpHandler = true)
        {
            if (withHttpHandler)
            {
                OpenTriviaDbConnector.SetHttpMessageHandler(_mockHttpHandler.Object);
            }
            return new(_mockLogger.Object, _mockOptions.Object);
        }

        [Fact]
        public async Task GetQuizAsync_GetQuizFromOpenTDbWithRetry()
        {
            var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);
            var connector = CreateConnector(false);
            var response = await connector.GetQuizAsync(quizModel);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe(0);
            response.Results.Count.ShouldBe(5);

            // Schrijf naar output
            _output.WriteLine($"First serie");
            foreach (var item in response.Results)
            {
                _output.WriteLine($"Item: {item}");
            }

            response = await connector.GetQuizAsync(quizModel);

            response.ShouldNotBeNull();
            response.ResponseCode.ShouldBe(0);
            response.Results.Count.ShouldBe(5);

            _output.WriteLine($"\nSecond serie");

            foreach (var item in response.Results)
            {
                _output.WriteLine($"Item: {item}");
            }
        }

        [Fact]
        public async Task RequestTokenAsync_RequestTokenFromOpenTDbWithRetry()
        {
            var connector = CreateConnector(false);
            await connector.RequestTokenAsync();
            connector._token.ShouldNotBeNullOrEmpty();

            var previousToken = connector._token;

            await connector.RequestTokenAsync(true);

            connector._token.ShouldNotBeNullOrEmpty();

            connector._token.ShouldBe(previousToken);
        }

        //[Fact]
        //public async Task GetQuizAsync_SuccessfulResponse_ReturnsQuizData()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);
        //    var tokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "test-token-123",
        //        ResponseMessage = "Token Generated Successfully!"
        //    };
        //    var quizResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 0,
        //        Results = new List<OpenTriviaDbApiResult>
        //        {
        //            new OpenTriviaDbApiResult
        //            {
        //                Type = "multiple",
        //                Difficulty = "easy",
        //                Category = "General Knowledge",
        //                Question = "What is the capital of France?",
        //                CorrectAnswer = "Paris",
        //                IncorrectAnswers = new List<string> { "London", "Berlin", "Madrid" }
        //            }
        //        }
        //    };

        //    _mockHttpHandler.Protected()
        //        .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(quizResponse))
        //        });

        //    var connector = CreateConnector();

        //    // Act
        //    var result = await connector.GetQuizAsync(quizModel);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(0, result.ResponseCode);
        //    Assert.Single(result.Results);
        //    Assert.Equal("What is the capital of France?", result.Results[0].Question);
        //    Assert.Equal("Paris", result.Results[0].CorrectAnswer);
        //}

        //[Fact]
        //public async Task GetQuizAsync_NoResults_ThrowsException()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(50, 9, QuizType.multiple, QuizDifficulty.easy);
        //    var tokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "test-token-123",
        //        ResponseMessage = "Token Generated Successfully!"
        //    };
        //    var quizResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 1, // No Results
        //        Results = new List<OpenTriviaDbApiResult>()
        //    };

        //    _mockHttpHandler.Protected()
        //        .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(quizResponse))
        //        });

        //    var connector = CreateConnector();

        //    // Act & Assert
        //    var exception = await Assert.ThrowsAsync<OpenTriviaDbConnectorException>(
        //        () => connector.GetQuizAsync(quizModel));
        //    Assert.Contains("No results could be returned", exception.Message);
        //}

        //[Fact]
        //public async Task GetQuizAsync_InvalidParameter_ThrowsException()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);
        //    var tokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "test-token-123",
        //        ResponseMessage = "Token Generated Successfully!"
        //    };
        //    var quizResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 2, // Invalid Parameter
        //        Results = []
        //    };

        //    _mockHttpHandler.Protected()
        //        .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(quizResponse))
        //        });

        //    var httpClient = new HttpClient(_mockHttpHandler.Object);
        //    var connector = CreateConnector();

        //    var httpClientField = typeof(OpenTriviaDbConnector)
        //        .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //    httpClientField?.SetValue(null, httpClient);

        //    // Act & Assert
        //    var exception = await Assert.ThrowsAsync<OpenTriviaDbConnectorException>(
        //        () => connector.GetQuizAsync(quizModel));
        //    Assert.Contains("Invalid parameter(s) provided", exception.Message);
        //}

        //[Fact]
        //public async Task GetQuizAsync_TokenNotFound_RequestsNewTokenAndRetries()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);
        //    var tokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "test-token-123",
        //        ResponseMessage = "Token Generated Successfully!"
        //    };
        //    var newTokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "new-test-token-456",
        //        ResponseMessage = "Token Reset Successfully!"
        //    };
        //    var failedQuizResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 3, // Token Not Found
        //        Results = new List<OpenTriviaDbApiResult>()
        //    };
        //    var successQuizResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 0,
        //        Results = new List<OpenTriviaDbApiResult>
        //        {
        //            new OpenTriviaDbApiResult
        //            {
        //                Type = "multiple",
        //                Difficulty = "easy",
        //                Category = "General Knowledge",
        //                Question = "Test question?",
        //                CorrectAnswer = "Test answer",
        //                IncorrectAnswers = new List<string> { "Wrong 1", "Wrong 2", "Wrong 3" }
        //            }
        //        }
        //    };

        //    _mockHttpHandler.Protected()
        //        .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Initial token request
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Failed quiz request
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(failedQuizResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Token reset request
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(newTokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Successful quiz request
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(successQuizResponse))
        //        });

        //    var httpClient = new HttpClient(_mockHttpHandler.Object);
        //    var connector = CreateConnector();

        //    var httpClientField = typeof(OpenTriviaDbConnector)
        //        .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //    httpClientField?.SetValue(null, httpClient);

        //    // Act
        //    var result = await connector.GetQuizAsync(quizModel);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(0, result.ResponseCode);
        //    Assert.Single(result.Results);
        //}

        //[Fact]
        //public async Task GetQuizAsync_RateLimit_WaitsAndRetries()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);
        //    var tokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "test-token-123",
        //        ResponseMessage = "Token Generated Successfully!"
        //    };
        //    var rateLimitResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 5, // Rate Limit
        //        Results = new List<OpenTriviaDbApiResult>()
        //    };
        //    var successResponse = new OpenTriviaDbApiResponse()
        //    {
        //        ResponseCode = 0,
        //        Results = [
        //            new ()
        //            {
        //                Type = "multiple",
        //                Difficulty = "easy",
        //                Category = "General Knowledge",
        //                Question = "Test question after rate limit?",
        //                CorrectAnswer = "Test answer",
        //                IncorrectAnswers = [ "Wrong 1", "Wrong 2", "Wrong 3" ]
        //            }
        //        ]
        //    };

        //        _mockHttpHandler.Protected()
        //        .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Token request
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Rate limit response
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(rateLimitResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK) // Success after wait
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(successResponse))
        //        });

        //    var httpClient = new HttpClient(_mockHttpHandler.Object);
        //    var connector = CreateConnector();

        //    var httpClientField = typeof(OpenTriviaDbConnector)
        //        .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //    httpClientField?.SetValue(null, httpClient);

        //    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        //    // Act
        //    var result = await connector.GetQuizAsync(quizModel);

        //    // Assert
        //    stopwatch.Stop();
        //    Assert.NotNull(result);
        //    Assert.Equal(0, result.ResponseCode);
        //    Assert.Single(result.Results);
        //    // Verify that some delay occurred (should be at least 5 seconds for rate limit)
        //    Assert.True(stopwatch.ElapsedMilliseconds >= 5000);
        //}

        //[Fact]
        //public async Task GetQuizAsync_UnexpectedResponseCode_ThrowsException()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);
        //    var tokenResponse = new OpenTriviaDbApiSessionRequest
        //    {
        //        ResponseCode = 0,
        //        Token = "test-token-123",
        //        ResponseMessage = "Token Generated Successfully!"
        //    };
        //    var unexpectedResponse = new OpenTriviaDbApiResponse
        //    {
        //        ResponseCode = 999, // Unexpected code
        //        Results = new List<OpenTriviaDbApiResult>()
        //    };

        //    _mockHttpHandler.Protected()
        //        .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(tokenResponse))
        //        })
        //        .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
        //        {
        //            Content = new StringContent(JsonSerializer.Serialize(unexpectedResponse))
        //        });

        //    var httpClient = new HttpClient(_mockHttpHandler.Object);
        //    var connector = CreateConnector();

        //    var httpClientField = typeof(OpenTriviaDbConnector)
        //        .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //    httpClientField?.SetValue(null, httpClient);

        //    // Act & Assert
        //    var exception = await Assert.ThrowsAsync<OpenTriviaDbConnectorException>(
        //        () => connector.GetQuizAsync(quizModel));
        //    Assert.Contains("Unexpected response code 999", exception.Message);
        //}

        //[Fact]
        //public async Task GetQuizAsync_HttpException_ReturnsEmptyResponse()
        //{
        //    // Arrange
        //    var quizModel = new QuizRequest(5, 9, QuizType.multiple, QuizDifficulty.easy);

        //    _mockHttpHandler.Protected()
        //        .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
        //        .ThrowsAsync(new HttpRequestException("Network error"));

        //    var httpClient = new HttpClient(_mockHttpHandler.Object);
        //    var connector = CreateConnector();

        //    var httpClientField = typeof(OpenTriviaDbConnector)
        //        .GetField("_httpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        //    httpClientField?.SetValue(null, httpClient);

        //    // Act
        //    var result = await connector.GetQuizAsync(quizModel);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(0, result.ResponseCode); // Default response
        //    Assert.Empty(result.Results);
        //}
    }
}