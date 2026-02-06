using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public record AnswerResponse
    {
        public AnswerResponse(QuestionResult questionResult, QuizScore quizScore)
        {
            QuestionId = questionResult.QuestionId;
            Question = questionResult.Question;
            GivenAnswer = questionResult.GivenAnswer;
            CorrectAnswer = questionResult.CorrectAnswer;
            IsCorrect = questionResult.IsCorrect;
            QuizScore = quizScore;
        }

        [JsonPropertyName("question_id")]
        public int QuestionId { get; init; }

        [JsonPropertyName("question")]
        public string Question { get; init; }

        [JsonPropertyName("given_answer")]
        public string GivenAnswer { get; init; }

        [JsonPropertyName("correct_answer")]
        public string CorrectAnswer { get; init; }

        [JsonPropertyName("is_correct")]
        public bool IsCorrect { get; init; }

        [JsonPropertyName("quiz_score")]
        public QuizScore QuizScore { get; init; }
    }
}
