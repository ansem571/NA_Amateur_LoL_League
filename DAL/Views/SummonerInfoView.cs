using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using DAL.Enums;
using Newtonsoft.Json;

namespace DAL.Views
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
    }
}
