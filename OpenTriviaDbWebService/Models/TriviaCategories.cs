using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public record TriviaCategory(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name);

    public record TriviaCategories(
        [property: JsonPropertyName("trivia_categories")] List<TriviaCategory> Categories);
}
