using System.Diagnostics;

namespace OpenTriviaDbWebService.Models
{
    public partial record QuizScore
    {
        public AnswerResponse GetAnswerResponseAndUpdateScore(AnswerRequest answerRequest)
        {
            // Valid Id?
            var validId = answerRequest.QuestionId >= 0 && QuestionResults.Count > answerRequest.QuestionId;

            if (!validId)
            {
                throw new QuizScoreException($"Invalid questionId: {answerRequest.QuestionId}");
            }

            // Check if we have a question with the given questionId in the quiz
            var questionResult = QuestionResults[answerRequest.QuestionId];

            if (questionResult != null)
            {
                // No fooling around, a question will be answerd only once.
                return new AnswerResponse(questionResult!, this);
            }

            // Let's check the answer
            var quizItem = _quiz.Results[answerRequest.QuestionId];

            // Just to be sure
            Debug.Assert(QuestionResults[answerRequest.QuestionId]!.Question == quizItem.Question,
                $"Questions are not equal. [{QuestionResults[answerRequest.QuestionId]!.Question}] != [{quizItem.Question}]");

            questionResult = GetQuestionResultAndCalculateNewScore(answerRequest, quizItem);

            return new AnswerResponse(questionResult!, this);
        }

        private QuestionResult GetQuestionResultAndCalculateNewScore(AnswerRequest answerRequest, OpenTriviaDbApiResult quizItem)
        {
            bool isCorrect = string.Equals(answerRequest.Answer, quizItem.CorrectAnswer, StringComparison.OrdinalIgnoreCase);

            QuestionResult questionResult = new(
                questionId: answerRequest.QuestionId,
                question: quizItem.Question,
                givenAnswer: answerRequest.Answer,
                correctAnswer: quizItem.CorrectAnswer,
                isCorrect: isCorrect
            );

            QuestionResults[answerRequest.QuestionId] = questionResult;

            TotalCorrect = QuestionResults.Count(x => x != null && x.IsCorrect);
            TotalIncorrect = QuestionResults.Count(x => x != null && !x.IsCorrect);

            return questionResult;
        }
    }

    [Serializable]
    internal class QuizScoreException : Exception
    {
        public QuizScoreException()
        {
        }

        public QuizScoreException(string? message) : base(message)
        {
        }

        public QuizScoreException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
