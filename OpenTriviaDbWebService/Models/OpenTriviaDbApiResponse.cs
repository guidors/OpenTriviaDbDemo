using System.Net;
using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public record OpenTriviaDbApiResponse
    {
        [JsonPropertyName("response_code")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("results")]
        public List<OpenTriviaDbApiResult> Results { get; set; } = [];
    }

    public record OpenTriviaDbApiResult
    {
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("difficulty")]
        public required string Difficulty { get; init; }

        [JsonPropertyName("category")]
        public required string Category { get; init; }

        [JsonPropertyName("question")]
        public required string Question { get; init; }

        [JsonPropertyName("correct_answer")]
        public required string CorrectAnswer { get; init; }

        [JsonPropertyName("incorrect_answers")]
        public required List<string> IncorrectAnswers { get; init; }

        public override string ToString()
        {
            var s = $"Type: {Type}, Difficulty: {Difficulty}, Category: {Category}, Question: {Question}, Correct Answer: {CorrectAnswer}, Incorrect Answers: [{string.Join(", ", IncorrectAnswers)}]";
            return WebUtility.UrlDecode(s); // replace URL-encoded characters like %20 (space), %21 (!), etc. with their decoded equivalents
        }
    }
}