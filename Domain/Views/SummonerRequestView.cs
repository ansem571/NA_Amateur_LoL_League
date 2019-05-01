using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.Views
{
    public class SummonerRequestView
    {
        /// <summary>
        /// Name of the summoner making the request
        /// </summary>
        public string SummonerName { get; set; }
        /// <summary>
        /// List of summoners being requested
        /// </summary>
        public List<RequestedSummoner> RequestedSummoners { get; set; } = new List<RequestedSummoner>();
        /// <summary>
        /// List of all summonerNames excluding the current user
        /// </summary>
        public List<string> SummonerNames { get; set; } = new List<string>();

        public List<SelectListItem> Names { get; set; } = new List<SelectListItem>();

        public string StatusMessage { get; set; }
    }

    public class RequestedSummoner
    {
        public string SummonerName { get; set; }
        public bool IsSub { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RequestedSummoner summoner &&
                   SummonerName == summoner.SummonerName &&
                   IsSub == summoner.IsSub;
        }

        public override int GetHashCode()
        {
            var hashCode = -896505277;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SummonerName);
            hashCode = hashCode * -1521134295 + IsSub.GetHashCode();
            return hashCode;
        }
    }
}
