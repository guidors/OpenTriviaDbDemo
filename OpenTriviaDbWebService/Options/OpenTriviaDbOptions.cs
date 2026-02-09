namespace OpenTriviaDbWebService.Options
{    public class OpenTriviaDbOptions
    {
        internal const string OptionKey = "OpenTriviaDb";

        public required string ApiUrl { get; set; }
        public required string SessionUrl { get; set; }
        public required string CategoryUrl { get; set; }
    }
}