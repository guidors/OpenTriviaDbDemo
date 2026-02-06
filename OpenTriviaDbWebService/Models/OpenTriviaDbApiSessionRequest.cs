using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    /// <summary>
    /// SessionRequest represents the response from the Open Trivia Database API when requesting or resetting a session token.
    /// </summary>
    public record OpenTriviaDbApiSessionRequest
    {
        [JsonPropertyName("response_code")]
        public required int ResponseCode { get; init; }

        [JsonPropertyName("response_message")]
        public string? ResponseMessage { get; init; }

        [JsonPropertyName("token")]
        public string? Token { get; init; }
    }
}
