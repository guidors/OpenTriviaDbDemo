using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq; // Niet meer open source :-( https://dariusz-wozniak.github.io/fossed/
using OpenTriviaDbWebService.Infrastructure;
using OpenTriviaDbWebService.Models;
using OpenTriviaDbWebService.Options;
using Shouldly;
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
                SessionUrl = "https://opentdb.com/api_token.php",
                CategoryUrl = "https://opentdb.com/api_category.php"
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
    }
}