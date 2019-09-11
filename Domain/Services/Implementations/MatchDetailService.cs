using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Domain.Services.Implementations
{
    public class MatchDetailService : IMatchDetailsService
    {
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;
        private readonly Guid _apiKey = new Guid("RGAPI-0ef9c440-9c6e-492e-bf95-9e3284fabeeb");
        public MatchDetailService(IEmailService emailService, ILogger logger)
        {
            _emailService = emailService ?? throw new NotImplementedException(nameof(emailService));
            _logger = logger ?? throw new NotImplementedException(nameof(logger));
        }

        public async Task<bool> SendMatchDetailAsync(string matchDetailId)
        {

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://na1.api.riotgames.com/lol/match/v4/");
                    var response = await client.GetAsync($"matches/{matchDetailId}?api_key={_apiKey}");
                    if (response.IsSuccessStatusCode)
                    {

                    }
                }
            }
            catch (Exception e)
            {

            }

            return true;
        }
    }
}
