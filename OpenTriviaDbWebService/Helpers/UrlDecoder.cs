using System.Net;

namespace OpenTriviaDbWebService.Helpers
{
    public static class UrlDecoder
    {
        /// <summary>
        /// Decodes URL-encoded characters like %20 (space), %21 (!), etc.
        /// </summary>
        /// <param name="encodedString">The URL-encoded string</param>
        /// <returns>The decoded string</returns>
        public static string DecodeUrl(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString))
                return encodedString;

            return WebUtility.UrlDecode(encodedString);
        }

        /// <summary>
        /// Decodes both URL-encoded characters and HTML entities
        /// </summary>
        /// <param name="encodedString">The encoded string</param>
        /// <returns>The fully decoded string</returns>
        public static string DecodeUrlAndHtml(string encodedString)
        {
            if (string.IsNullOrEmpty(encodedString))
                return encodedString;

            // First decode URL encoding (%20, %21, etc.)
            string urlDecoded = WebUtility.UrlDecode(encodedString);

            // Then decode HTML entities (&quot;, &#039;, &amp;, etc.)
            return WebUtility.HtmlDecode(urlDecoded);
        }
    }
}