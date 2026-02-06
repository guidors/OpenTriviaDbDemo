using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public record AnswerRequest
    {
        [JsonPropertyName("question_id")]
        public int QuestionId { get; set; } = default!;

        [JsonPropertyName("question")]
        public string Question { get; set; } = default!;

        [JsonPropertyName("answer")]
        public string Answer { get; set; } = default!;
    }
}
