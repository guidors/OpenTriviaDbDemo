using System.Diagnostics.CodeAnalysis;

namespace OpenTriviaDbWebService.Extentions;

/// <summary>
/// Contains the logger extensions.
/// </summary>
[ExcludeFromCodeCoverage]
internal static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 100, Level = LogLevel.Error,
        Message = "Error getting quiz.")]
    public static partial void ErrorGettingQuiz(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 101, Level = LogLevel.Information,
        Message = "Categories retrieved from API.")]
    public static partial void InformationCategoriesRetrieved(this ILogger logger);

    [LoggerMessage(EventId = 102, Level = LogLevel.Error,
        Message = "Error getting categories.")]
    public static partial void ErrorGettingCategories(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 103, Level = LogLevel.Warning,
        Message = "Open Trivia Database API returned HTTP {StatusCode}: {ReasonPhrase}")]
    public static partial void WarningApiHttp(this ILogger logger, int statusCode, string? reasonPhrase);

    [LoggerMessage(EventId = 104, Level = LogLevel.Error,
        Message = "Unexpected error occurred: {Message}")]
    public static partial void ErrorUnexpected(this ILogger logger, Exception exception, string message);

    [LoggerMessage(EventId = 105, Level = LogLevel.Information,
        Message = "Open Trivia Database API: Successfully retrieved quiz.")]
    public static partial void InformationQuizRetrievedSuccessfully(this ILogger logger);

    [LoggerMessage(EventId = 106, Level = LogLevel.Error,
        Message = "An error occurred while getting a quiz from the Open Trivia Database API.")]
    public static partial void ErrorGettingQuizFromApi(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 107, Level = LogLevel.Information,
        Message = "Using existing token for Open Trivia Database API: {Token}")]
    public static partial void InformationUsingExistingToken(this ILogger logger, string token);

    [LoggerMessage(EventId = 108, Level = LogLevel.Warning,
        Message = "Open Trivia Database API Session Request returned HTTP {StatusCode}: {ReasonPhrase}")]
    public static partial void WarningSessionRequestHttp(this ILogger logger, int statusCode, string? reasonPhrase);

    [LoggerMessage(EventId = 109, Level = LogLevel.Information,
        Message = "Open Trivia Database API: {ResponseMessage}")]
    public static partial void InformationApiResponseMessage(this ILogger logger, string? responseMessage);

    [LoggerMessage(EventId = 110, Level = LogLevel.Error,
        Message = "An error occurred while requesting a token from the Open Trivia Database API.")]
    public static partial void ErrorRequestingToken(this ILogger logger, Exception exception);

    [LoggerMessage(EventId = 111, Level = LogLevel.Warning,
        Message = "Open Trivia Database Categories API returned HTTP {StatusCode}: {ReasonPhrase}")]
    public static partial void WarningCategoriesApiHttp(this ILogger logger, int statusCode, string? reasonPhrase);

    [LoggerMessage(EventId = 112, Level = LogLevel.Information,
        Message = "Open Trivia Database Categories API: Successfully retrieved {Count} categories.")]
    public static partial void InformationCategoriesRetrievedSuccessfully(this ILogger logger, int count);

    [LoggerMessage(EventId = 113, Level = LogLevel.Error,
        Message = "An error occurred while getting categories from the Open Trivia Database API.")]
    public static partial void ErrorGettingCategoriesFromApi(this ILogger logger, Exception exception);    
}
