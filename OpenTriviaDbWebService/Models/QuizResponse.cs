using System.Net;
using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public record QuizResponse
    {
        public QuizResponse(string token, OpenTriviaDbApiResponse openTriviaDbApiResponse)
        {
            Token = token;
            QuizItems = [.. openTriviaDbApiResponse.Results.Select(x => { return new QuizItem(x); })];
        }

        [JsonPropertyName("token")]
        public string Token { get; init; } = default!;

        [JsonPropertyName("quiz_items")]
        public List<QuizItem> QuizItems { get; set; } = [];
    }

    public record QuizItem
    {
        public QuizItem(OpenTriviaDbApiResult openTriviaDbApiResult)
        {
            Type = openTriviaDbApiResult.Type;
            Difficulty = openTriviaDbApiResult.Difficulty;
            Category = openTriviaDbApiResult.Category;
            Question = openTriviaDbApiResult.Question;

            var allAnswers = openTriviaDbApiResult.IncorrectAnswers.Append(openTriviaDbApiResult.CorrectAnswer);

            // Randomize the order of the answers so that the correct answer isn't always in the same position
            if (Type == "multiple")
            {
                allAnswers = allAnswers.OrderBy(x => Guid.NewGuid());
            }
            else if (Type == "boolean")
            {
                // for boolean questions, we have true first
                allAnswers = allAnswers.OrderByDescending(x => x);
            }
            Answers.AddRange(allAnswers);
        }

        [JsonPropertyName("type")]
        public string Type { get; set; } = default!;

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = default!;

        [JsonPropertyName("category")]
        public string Category { get; set; } = default!;

        [JsonPropertyName("question")]
        public string Question { get; set; } = default!;

        [JsonPropertyName("answers")]
        public List<string> Answers { get; set; } = [];

        public override string ToString()
        {
            var s = $"Type: {Type}, Difficulty: {Difficulty}, Category: {Category}, Question: {Question}, Answers: [{string.Join(", ", Answers)}]";
            return WebUtility.UrlDecode(s); // replace URL-encoded characters like %20 (space), %21 (!), etc. with their decoded equivalents
        }
    }
}
