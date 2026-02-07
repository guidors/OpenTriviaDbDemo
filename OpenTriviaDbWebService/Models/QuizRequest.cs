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
            string apiString = $"amount={NumberOfQuestions}";

            if (Category != 0)
                apiString += $"&category={Category}";
            if (Difficulty != QuizDifficulty.any)
                apiString += $"&difficulty={Difficulty}";
            if (QuizType != QuizType.any)
                apiString += $"&type={QuizType}";

            return apiString;
        }
    }
}
