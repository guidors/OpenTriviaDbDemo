
namespace OpenTriviaDbWebService.Models
{
    public delegate Task<TriviaCategories?> GetCategoriesDelegate();
    public partial record QuizRequest
    {
        private static GetCategoriesDelegate? _getCategoriesDelegate;

        private static TriviaCategories? _categories;

        private const int MinQuestions = 1;
        private const int MaxQuestions = 20;

        public static void Init(GetCategoriesDelegate getCategoriesDelegate)
        {
            _getCategoriesDelegate = getCategoriesDelegate;
        }

        public async Task ValidateAsync()
        {
            _categories ??= await FillCategoriesAsync();

            if (NumberOfQuestions < MinQuestions || NumberOfQuestions > MaxQuestions)
                throw new QuizRequestException($"Number of questions must be between {MinQuestions} and {MaxQuestions}.");

            if (_categories == null)
                throw new QuizRequestException("Categories have not been loaded.");

            if (!_categories.Categories.Exists(c => c.Id == Category))
                throw new QuizRequestException($"Category {Category} does not exist.");
        }

        private static async Task<TriviaCategories?> FillCategoriesAsync()
        {
            if (_getCategoriesDelegate == null)
                return null;

            return await _getCategoriesDelegate();
        }
    }

    [Serializable]
    internal class QuizRequestException : Exception
    {
        public QuizRequestException()
        {
        }

        public QuizRequestException(string? message) : base(message)
        {
        }

        public QuizRequestException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
