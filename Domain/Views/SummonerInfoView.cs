using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Views
{
    public class SummonerInfoView
    {
        [Required]
        public string SummonerName { get; set; }
        [Required]
        public SummonerRoleEnum Role { get; set; }

        public SummonerRoleEnum OffRole { get; set; }
        [Required]
        public TierDivisionEnum TierDivision { get; set; }
        public TierDivisionEnum? PreviousSeasonTierDivision { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Value must be positive")]
        public int CurrentLp { get; set; }
        [Required]
        public string OpGgUrl { get; set; }
        public bool IsValid { get; set; } = false;
        public bool IsSubOnly { get; set; }
        public Guid UserId { get; set; }

        public SummonerRoleEnum TeamRole { get; set; }

        public List<AlternateAccountView> AlternateAccounts { get; set; } = new List<AlternateAccountView>();

        public string StatusMessage { get; set; }

        //Remove empty views
        public void RemoveEmptyViewsForDb()
        {
            for (var i = 0; i < AlternateAccounts.Count; i++)
            {
                var alternateAccount = AlternateAccounts[i];
                if (string.IsNullOrEmpty(alternateAccount.AlternateName) ||
                    string.IsNullOrEmpty(alternateAccount.OpGgUrlLink))
                {
                    AlternateAccounts.Remove(alternateAccount);
                    i--;
                }
            }
        }
    }
}
