namespace OpenTriviaDbWebService.Models
{
    public partial record QuizModel
    {
        private static TriviaCategories? _categories;
        private static readonly HttpClient _httpClient = new();

        private const int MinQuestions = 1;
        private const int MaxQuestions = 20;

        public async Task ValidateAsync()
        {
            _categories ??= await FillCategoriesAsync();

            if (NumberOfQuestions < MinQuestions || NumberOfQuestions > MaxQuestions)
                throw new ArgumentException($"Number of questions must be between {MinQuestions} and {MaxQuestions}.");

            if (_categories == null)
                throw new InvalidOperationException("Categories have not been loaded.");

            if (!_categories.Categories.Exists(c => c.Id == Category))
                throw new ArgumentException($"Category {Category} does not exist.");
        }

        private static async Task<TriviaCategories?> FillCategoriesAsync()
        {
            try
            {
                // Improvement would be to 
                var categories = await _httpClient.GetFromJsonAsync<TriviaCategories>("https://opentdb.com/api_category.php");
                return categories;
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return null;
            }
        }
    }
}
