using System.Net;
using System.Text.Json.Serialization;

namespace OpenTriviaDbWebService.Models
{
    public partial record OpenTriviaDbApiResponse
    {
       public void RemoveWebCoding()
        {
            foreach (var result in Results)
            {
                result.Category = WebUtility.HtmlDecode(result.Category);
                result.Question = WebUtility.HtmlDecode(result.Question);
                result.CorrectAnswer = WebUtility.HtmlDecode(result.CorrectAnswer);
                for (int i = 0; i < result.IncorrectAnswers.Count; i++)
                {
                    result.IncorrectAnswers[i] = WebUtility.HtmlDecode(result.IncorrectAnswers[i]);
                }
            }
        }
    }
}