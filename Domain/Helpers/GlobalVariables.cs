using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using RiotSharp;
using RiotSharp.Caching;
using RiotSharp.Endpoints.StaticDataEndpoint;

namespace Domain.Helpers
{
    public static class GlobalVariables
    {
        public static ICache ChampionCache { get; set; }
        public static readonly TimeSpan ClearCacheTime = TimeSpan.FromDays(30);
        public static readonly string Apikey = "RGAPI-fc7bbbec-da3c-4513-9f79-68ea94de2990";
        public static readonly RiotApi Api = RiotApi.GetDevelopmentInstance(Apikey);
        public static readonly StaticDataEndpoints ChampsApi = StaticDataEndpoints.GetInstance();

        public static async Task SetupChampionCache(ILookupRepository lookupRepo)
        {
            var champions = (await lookupRepo.GetLookupEntitiesByCategory("Champion")).ToList();
            foreach (var championLookup in champions)
            {
                var enumValue = championLookup.Enum;
                var trueValue = championLookup.Value;
                GlobalVariables.ChampionCache.Add(enumValue, trueValue, GlobalVariables.ClearCacheTime);
            }

            if (ChampionCache.IsEmpty())
            {
                throw new NotImplementedException(nameof(GlobalVariables.ChampionCache));
            }
        }
    }
}
