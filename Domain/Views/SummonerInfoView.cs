﻿using System.Collections.Generic;
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
        [Required]
        public TierDivisionEnum TierDivision { get; set; }
        [Required]
        public int CurrentLp { get; set; }
        [Required]
        public string OpGgUrl { get; set; }
        [Required]
        public bool IsValid { get; set; }

        public List<AlternateAccountView> AlternateAccounts { get; set; }

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
