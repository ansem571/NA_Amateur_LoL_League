using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Views.RiotApi
{
    public class MatchDto
    {
        public int SeasonId { get; set; }
        public int QueueId { get; set; }
        public long GameId { get; set; }
        public List<ParticipantIdentityDto> ParticipantIdentities { get; set; }
        public string GameVersion { get; set; }
        public string PlatformId { get; set; }
        public string GameMode { get; set; }
        public int MapId { get; set; }
        public string GameType { get; set; }
        public List<TeamStatsDto> Teams { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public long GameDuration { get; set; }
        public long GameCreation { get; set; }
    }

    public class ParticipantIdentityDto
    {
        public PlayerDto Player { get; set; }
        public int ParticipantId { get; set; }
    }

    public class PlayerDto
    {
        public string CurrentPlatformId { get; set; }
        public string SummonerName { get; set; }
        public string MatchHistoryUri { get; set; }
        public string PlatformId { get; set; }
        public string CurrentAccountId { get; set; }
        public int ProfileIcon { get; set; }
        public string SummonerId { get; set; }
        public string AccountId { get; set; }
    }

    public class TeamStatsDto
    {
        public bool FirstDragon { get; set; }
        public bool FirstInhibitor { get; set; }
        public List<TeamBansDto> Bans { get; set; }
        public int BaronKills { get; set; }
        public bool FirstRiftHerald { get; set; }
        public bool FirstBaron { get; set; }
        public bool FirstBlood { get; set; }
        public int TeamId { get; set; }
        public bool FirstTower { get; set; }
        public int VilemawKills { get; set; }
        public int InhibitorKills { get; set; }
        public int TowerKills { get; set; }
        public int DominionVictoryScore { get; set; }
        public string Win { get; set; }
        public int DragonKills { get; set; }
    }

    public class TeamBansDto
    {
        public int PickTurn { get; set; }
        public int ChampionId { get; set; }
    }

    public class ParticipantDto
    {
        public ParticipantStatsDto Stats { get; set; }
        public int ParticipantId { get; set; }
        public List<RuneDto> Runes { get; set; }
        public ParticipantTimelineDto Timeline { get; set; }
        public int TeamId { get; set; }
        public int Spell2Id { get; set; }
        public List<MasteryDto> Masteries { get; set; }
        public string HighestAchievedSeasonTier { get; set; }
        public int Spell1Id { get; set; }
        public int ChampionId { get; set; }
    }

    public class ParticipantStatsDto
    {
        public bool FirstBloodAssist { get; set; }
        public long VisionScore { get; set; }
        public long MagicDamageDealtToChampions { get; set; }
        public long DamageDealtToObjectives { get; set; }
        public int TotalTimeCrowdControlDealt { get; set; }
        public int LongestTimeSpentLiving { get; set; }
        public int Perk1Var1 { get; set; }
        public int Perk1Var3 { get; set; }
        public int Perk1Var2  { get; set; }
        public int TripleKills { get; set; }
        public int Perk3Var3 { get; set; }
        public int NodeNeutralizeAssist { get; set; }
        public int Perk3Var2 { get; set; }
        public int PlayerScore9 { get; set; }
        public int PlayerScore8 { get; set; }
        public int Kills { get; set; }
        public int PlayerScore1 { get; set; }
        public int PlayerScore0 { get; set; }
        public int PlayerScore3 { get; set; }
        public int PlayerScore2 { get; set; }
        public int PlayerScore5 { get; set; }
        public int PlayerScore4 { get; set; }
        public int PlayerScore7 { get; set; }
        public int PlayerScore6 { get; set; }
        public int Perk5Var1 { get; set; }
        public int Perk5Var3 { get; set; }
        public int Perk5Var2 { get; set; }
        public int TotalScoreRank { get; set; }
        public int NeutralMinionsKilled { get; set; }
        public long DamageDealtToTurrets { get; set; }
        public long PhysicalDamageDealtToChampions { get; set; }
        public int NodeCapture { get; set; }
        public int LargestMultiKill { get; set; }
        public int Perk2Var2 { get; set; }
        public int Perk2Var3 { get; set; }
        public int TotalUnitsHealed { get; set; }
        public int Perk2Var1 { get; set; }
        public int Perk4Var1 { get; set; }
        public int Perk4Var2 { get; set; }
        public int Perk4Var3 { get; set; }
        public int WardsKilled { get; set; }
        public int LargestCriticalStrike { get; set; }
        public int LargestKillingSpree { get; set; }
        public int QuadraKills { get; set; }
        public int TeamObjective { get; set; }
        public long MagicDamageDealt { get; set; }
        public int Item2 { get; set; }
        public int Item3 { get; set; }
        public int Item0 { get; set; }
        public int NeutralMinionsKilledTeamJungle { get; set; }
        public int Item6 { get; set; }
        public int Item4 { get; set; }
        public int Item5 { get; set; }
        public int Perk1 { get; set; }
        public int Perk0 { get; set; }
        public int Perk3 { get; set; }
        public int Perk2 { get; set; }
        public int Perk5 { get; set; }
        public int Perk4 { get; set; }
        public int Perk3Var1 { get; set; }
        public long DamageSelfMitigated { get; set; }
        public long MagicalDamageTaken { get; set; }
        public bool FirstInhibitorKill { get; set; }
        public long TrueDamageTaken { get; set; }
        public int NodeNeutralize { get; set; }
        public int Assists { get; set; }
        public int CombatPlayerScore { get; set; }
        public int PerkPrimaryStyle { get; set; }
        public int GoldSpent { get; set; }
        public long TrueDamageDealt { get; set; }
        public int ParticipantId { get; set; }
        public long TotalDamageTaken { get; set; }
        public long PhysicalDamageDealt { get; set; }
        public int SightWardsBoughtInGame { get; set; }
        public long TotalDamageDealtToChampions { get; set; }
        public long PhysicalDamageTaken { get; set; }
        public int TotalPlayerScore { get; set; }
        public bool Win { get; set; }
        public int ObjectivePlayerScore { get; set; }
        public long TotalDamageDealt { get; set; }
        public int Item1 { get; set; }
        public int NeutralMinionsKilledEnemyJungle { get; set; }
        public int Deaths { get; set; }
        public int WardsPlaced { get; set; }
        public int PerkSubStyle { get; set; }
        public int TurretKills { get; set; }
        public bool FirstBloodKill { get; set; }
        public long TrueDamageDealtToChampions { get; set; }
        public int GoldEarned { get; set; }
        public int KillingSprees { get; set; }
        public int UnrealKills { get; set; }
        public int AltarsCaptured { get; set; }
        public bool FirstTowerAssist { get; set; }
        public bool FirstTowerKill { get; set; }
        public int ChampLevel { get; set; }
        public int DoubleKills { get; set; }
        public int NodeCaptureAssist { get; set; }
        public int InhibitorKills { get; set; }
        public bool FirstInhibitorAssist { get; set; }
        public int Perk0Var1 { get; set; }
        public int Perk0Var2 { get; set; }
        public int Perk0Var3 { get; set; }
        public int VisionWardsBoughtInGame { get; set; }
        public int AltarsNeutralized { get; set; }
        public int PentaKills { get; set; }
        public long TotalHeal { get; set; }
        public int TotalMinionsKilled { get; set; }
        public long TimeCCingOthers { get; set; }
    }

    public class RuneDto
    {
        public int RuneId { get; set; }
        public int Rank { get; set; }
    }

    public class MasteryDto
    {
        public int MasteryId { get; set; }
        public int Rank { get; set; }
    }

    public class ParticipantTimelineDto
    {
        public string Lane { get; set; }
        public int ParticipantId { get; set; }
        public Dictionary<string, double> CsDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> GoldPerMinDeltas { get; set; }
        public Dictionary<string, double> XpDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> CreepsPerMinDeltas { get; set; }
        public Dictionary<string, double> XpPerMinDeltas { get; set; }
        public string Role { get; set; }
        public Dictionary<string, double> DamageTakenDiffPerMinDeltas { get; set; }
        public Dictionary<string, double> DamageTakenPerMinDeltas { get; set; }
    }
}
