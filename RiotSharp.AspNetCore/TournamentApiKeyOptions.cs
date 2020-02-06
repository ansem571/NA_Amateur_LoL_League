namespace RiotSharp.AspNetCore
{
    public class TournamentApiKeyOptions : ApiKeyOptions
    {
        internal TournamentApiKeyOptions() { }

        public bool UseStub { get; set; }
    }
}
