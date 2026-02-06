namespace OpenTriviaDbWebService.Models
{
    public enum QuizDifficulty
    {
        any,
        easy,
        medium,
        hard
    }

    public enum QuizType
    {
        any,
        multiple,
        boolean
    }

    public partial record QuizRequest(
        int NumberOfQuestions,
        int Category,
        QuizType QuizType,
        QuizDifficulty Difficulty)
    {
        public override string ToString()
        {
            return $"amount={NumberOfQuestions}&category={Category}&difficulty={Difficulty}&type={QuizType}";
        }
    }
}
