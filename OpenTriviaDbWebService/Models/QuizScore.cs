using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public partial record QuizScore
    {
        private OpenTriviaDbApiResponse _quiz;

        public QuizScore(OpenTriviaDbApiResponse quiz)
        {
            _quiz = quiz;

            // Initialize the QuestionResults list with null values for each question in the quiz, to be filled in later when the user answers the questions.
            _quiz.Results.ForEach(x => QuestionResults.Add(null));

            Total = _quiz.Results.Count;
        }

        [JsonPropertyName("question_results")]
        public List<QuestionResult?> QuestionResults { get; } = [];

        [JsonPropertyName("total")]
        public int Total { get; private set; } = 0;

        [JsonPropertyName("total_correct")]
        public int TotalCorrect { get; private set; } = 0;

        [JsonPropertyName("total_incorrect")]
        public int TotalIncorrect { get; private set; } = 0;
    }

    public record QuestionResult
    {
        public QuestionResult(int questionId, string question, string givenAnswer, string correctAnswer, bool isCorrect)
        {
            QuestionId = questionId;
            Question = question;
            GivenAnswer = givenAnswer;
            CorrectAnswer = correctAnswer;
            IsCorrect = isCorrect;
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
    }
}
