using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using RiotSharp;
using RiotSharp.Caching;
using RiotSharp.Endpoints.StaticDataEndpoint;

namespace Domain.Helpers
{
    public static class GlobalVariables
    {
        public static ICache ChampionCache { get; set; }
        public static readonly TimeSpan ClearCacheTime = TimeSpan.FromDays(30);
        public static readonly TimeSpan CheckTime = TimeSpan.FromDays(1);
        public static readonly string Apikey = "RGAPI-fc7bbbec-da3c-4513-9f79-68ea94de2990";
        public static readonly RiotApi Api = RiotApi.GetDevelopmentInstance(Apikey);
        public static readonly StaticDataEndpoints ChampsApi = StaticDataEndpoints.GetInstance();
        private static DateTime _nextCheck;
        private static DateTime _nextUpdate;

        public static async Task SetupChampionCache(ILookupRepository lookupRepo)
        {
            if (ChampionCache.IsEmpty())
            {
                var champions = (await lookupRepo.GetLookupEntitiesByCategory("Champion")).ToList();
                foreach (var championLookup in champions)
                {
                    var enumValue = championLookup.Enum;
                    var trueValue = championLookup;
                    GlobalVariables.ChampionCache.Add(enumValue, trueValue, GlobalVariables.ClearCacheTime);
                }

                if (ChampionCache.IsEmpty())
                {
                    throw new NotImplementedException(nameof(GlobalVariables.ChampionCache));
                }

                _nextCheck = DateTime.Now.AddDays(1);
                _nextUpdate = DateTime.Now.Add(ClearCacheTime);
            }
            else
            {
                _nextCheck = DateTime.Now.Add(CheckTime);
            }
        }

        public static void UpdateCache(ILookupRepository lookupRepo, ILogger logger)
        {
            var thread = new Thread(() => Runner(async () => await SetupChampionCache(lookupRepo)));
            while (true)
            {
                try
                {
                    if (!thread.IsAlive)
                    {
                        thread.Start();
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error running Runner for Champion Cache");
                    thread.Join();
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
        }

        //Thread
        private static void Runner(Action action)
        {
            var now = DateTime.Now;
            while (now < _nextUpdate)
            {
                var nextcheck = _nextCheck - now;
                Thread.Sleep(nextcheck);
                now = DateTime.Now;
                _nextCheck = now.AddDays(1);
            }

            if (now > _nextUpdate)
            {
                action();
                var nextcheck = _nextCheck - now;
                Thread.Sleep(nextcheck);
                now = DateTime.Now;
                _nextCheck = now.AddDays(1);
            }
        }
    }
}
