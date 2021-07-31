using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DAL.Entities.UserData;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using RiotSharp;
using RiotSharp.Caching;
using RiotSharp.Endpoints.StaticDataEndpoint;

namespace Domain.Helpers
{
    public static class GlobalVariables
    {
        public static readonly TimeSpan ClearCacheTime = TimeSpan.FromDays(30);
        public static readonly TimeSpan CheckTime = TimeSpan.FromDays(1);
        public static readonly string Apikey = "RGAPI-fc7bbbec-da3c-4513-9f79-68ea94de2990";
        public static readonly RiotApi Api = RiotApi.GetDevelopmentInstance(Apikey);
        public static readonly StaticDataEndpoints ChampsApi = StaticDataEndpoints.GetInstance();
        public static readonly Dictionary<string, LookupEntity> ChampionDictionary = new Dictionary<string, LookupEntity>();

        public static async Task SetupChampionCache(ILookupRepository lookupRepo)
        {
            var retry = 0;
            var maxRetry = 4;
            while (retry < maxRetry)
            {
                try
                {
                    if (!ChampionDictionary.Any())
                    {
                        var champions = (await lookupRepo.GetLookupEntitiesByCategory("Champion")).ToList();
                        foreach (var championLookup in champions.OrderBy(x=>x.Enum))
                        {
                            var enumValue = championLookup.Enum;
                            var trueValue = championLookup;
                            ChampionDictionary.Add(enumValue.ToLowerInvariant(), trueValue);
                        }

                        if (!ChampionDictionary.Any())
                        {
                            throw new NotImplementedException(nameof(GlobalVariables.ChampionDictionary));
                        }
                    }

                    break;
                }
                catch (Exception)
                {
                    retry++;
                    if (retry >= maxRetry)
                    {
                        //somethings wrong I can feel it
                    }
                }
            }
        }
    }
}
