using System.Collections.Generic;
using System.Linq;
using LoLDraftSimulator.Models;

namespace LoLDraftSimulator.Services
{
    public class DraftEngine
    {
        private readonly Dictionary<string, int> _metaTiers;
        private readonly CounterpickDatabase     _db;

        public DraftEngine(Dictionary<string, int> metaTiers, CounterpickDatabase db)
        {
            _metaTiers = metaTiers;
            _db        = db;
        }

        public bool IsBlindPick(string championId)
            => _db.BlindPicks.Contains(championId);

        public List<string> GuessRoles(string championId)
            => _db.RoleGuesses.TryGetValue(championId, out var roles) ? roles : new();

        public List<ScoredChampion> GetTopRecommendations(
            DraftState state,
            List<Champion> allChampions,
            List<ChampionPoolEntry> pool,
            string roleNeeded,
            int topN = 3)
        {
            var taken = state.BlueTeamBans
                .Concat(state.RedTeamBans)
                .Concat(state.BlueTeamPicks)
                .Concat(state.RedTeamPicks)
                .Select(c => c.Id)
                .ToHashSet();

            var available = allChampions.Where(c => !taken.Contains(c.Id)).ToList();

            if (!string.IsNullOrEmpty(roleNeeded))
                available = available
                    .Where(c => c.Roles != null && c.Roles.Contains(roleNeeded))
                    .ToList();

            var poolDict = pool.ToDictionary(p => $"{p.ChampionId}_{p.Role}", p => p);
            var enemyIds = state.RedTeamPicks.Select(e => e.Id).ToHashSet();
            var allyIds  = state.BlueTeamPicks.Select(a => a.Id).ToHashSet();

            var scored = available.Select(champ =>
            {
                double score = 0;

                if (_metaTiers.TryGetValue(champ.Id, out int tier))
                    score += (6 - tier) * 2.0;

                string poolKey = $"{champ.Id}_{roleNeeded}";
                ChampionPoolEntry entry = null;
                if (poolDict.TryGetValue(poolKey, out entry))
                {
                    score += entry.Mastery * 1.5;
                    if (entry.IsMainPick) score += 5;
                }

                List<string> countersEnemies = new();
                if (_db.Counters.TryGetValue(champ.Id, out var counterEntry))
                {
                    countersEnemies = counterEntry.Counters.Where(e => enemyIds.Contains(e)).ToList();
                    score += countersEnemies.Count * 4.0;
                    score -= counterEntry.CounteredBy.Count(e => enemyIds.Contains(e)) * 3.0;
                }

                score += CalculateSynergyBonus(champ, allyIds);

                if (enemyIds.Count == 0 && IsBlindPick(champ.Id))
                    score += 5;

                return new ScoredChampion
                {
                    Champion        = champ,
                    Score           = score,
                    PoolEntry       = entry,
                    IsBlind         = IsBlindPick(champ.Id),
                    CountersEnemies = countersEnemies
                };
            })
            .OrderByDescending(s => s.Score)
            .Take(topN)
            .ToList();

            return scored;
        }

        private double CalculateSynergyBonus(Champion candidate, HashSet<string> allyIds)
        {
            var synergies = new Dictionary<string, string[]>
            {
                ["Amumu"]  = new[] { "Jinx", "Caitlyn", "Jhin" },
                ["Yasuo"]  = new[] { "Amumu", "Nautilus", "Thresh" },
                ["Thresh"] = new[] { "Jinx", "Caitlyn", "Ezreal" },
            };
            if (synergies.TryGetValue(candidate.Id, out var s))
                return s.Count(x => allyIds.Contains(x)) * 2.0;
            return 0;
        }
    }

    public class ScoredChampion
    {
        public Champion          Champion        { get; set; }
        public double            Score           { get; set; }
        public ChampionPoolEntry PoolEntry       { get; set; }
        public bool              IsBlind         { get; set; }
        public List<string>      CountersEnemies { get; set; } = new();

        public string ScoreLabel    => $"Score : {Score:F1}";
        public string MasteryLabel  => PoolEntry != null
            ? $"Maîtrise : {PoolEntry.Mastery}/10 — {PoolEntry.MasteryLabel}"
            : "Non dans ton pool";
        public string CounterLabel  => CountersEnemies.Count > 0
            ? $"⚔ Counter : {string.Join(", ", CountersEnemies)}"
            : "";
        public string BlindLabel    => IsBlind ? "🛡 Blind safe" : "";
    }
}
