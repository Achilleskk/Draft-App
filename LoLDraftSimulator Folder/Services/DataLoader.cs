using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LoLDraftSimulator.Models;

namespace LoLDraftSimulator.Services
{
    public class DataLoader
    {
        private const string ChampionsFile    = "Data/champions.json";
        private const string PoolFile         = "Data/my_champion_pool.json";
        private const string MetaFile         = "Data/current_meta.json";
        private const string CounterpickFile  = "Data/counterpicks.json";

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // ── Champions ──────────────────────────────────────────────────────
        public List<Champion> LoadAllChampions()
        {
            if (!File.Exists(ChampionsFile))
                return GetDefaultChampions();
            string json = File.ReadAllText(ChampionsFile);
            return JsonSerializer.Deserialize<List<Champion>>(json, _jsonOptions) ?? new();
        }

        // ── Champion Pool ──────────────────────────────────────────────────
        public List<ChampionPoolEntry> LoadChampionPool()
        {
            if (!File.Exists(PoolFile)) return new();
            string json = File.ReadAllText(PoolFile);
            return JsonSerializer.Deserialize<List<ChampionPoolEntry>>(json, _jsonOptions) ?? new();
        }

        public void SaveChampionPool(List<ChampionPoolEntry> pool)
        {
            Directory.CreateDirectory("Data");
            File.WriteAllText(PoolFile, JsonSerializer.Serialize(pool, new JsonSerializerOptions { WriteIndented = true }));
        }

        // ── Méta (tier list) ───────────────────────────────────────────────
        public Dictionary<string, int> LoadMetaTiers()
        {
            if (!File.Exists(MetaFile)) return new();
            string json = File.ReadAllText(MetaFile);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json, _jsonOptions) ?? new();
        }

        // ── Base de counterpicks ───────────────────────────────────────────
        public CounterpickDatabase LoadCounterpickDatabase()
        {
            if (!File.Exists(CounterpickFile)) return new();
            string json = File.ReadAllText(CounterpickFile);
            return JsonSerializer.Deserialize<CounterpickDatabase>(json, _jsonOptions) ?? new();
        }

        // ── Données par défaut ─────────────────────────────────────────────
        private List<Champion> GetDefaultChampions() => new()
        {
            new() { Id="Ahri",      Name="Ahri",      PrimaryRole="Mid",     Roles=new[]{"Mid"} },
            new() { Id="Zed",       Name="Zed",        PrimaryRole="Mid",     Roles=new[]{"Mid","Jungle"} },
            new() { Id="LeeSin",    Name="Lee Sin",    PrimaryRole="Jungle",  Roles=new[]{"Jungle"} },
            new() { Id="Jinx",      Name="Jinx",       PrimaryRole="Bot",     Roles=new[]{"Bot"} },
            new() { Id="Thresh",    Name="Thresh",     PrimaryRole="Support", Roles=new[]{"Support"} },
            new() { Id="Darius",    Name="Darius",     PrimaryRole="Top",     Roles=new[]{"Top"} },
            new() { Id="Yasuo",     Name="Yasuo",      PrimaryRole="Mid",     Roles=new[]{"Mid","Bot"} },
            new() { Id="Lux",       Name="Lux",        PrimaryRole="Support", Roles=new[]{"Support","Mid"} },
            new() { Id="Garen",     Name="Garen",      PrimaryRole="Top",     Roles=new[]{"Top"} },
            new() { Id="Ezreal",    Name="Ezreal",     PrimaryRole="Bot",     Roles=new[]{"Bot"} },
            new() { Id="Katarina",  Name="Katarina",   PrimaryRole="Mid",     Roles=new[]{"Mid"} },
            new() { Id="Vi",        Name="Vi",         PrimaryRole="Jungle",  Roles=new[]{"Jungle"} },
            new() { Id="Caitlyn",   Name="Caitlyn",    PrimaryRole="Bot",     Roles=new[]{"Bot"} },
            new() { Id="Morgana",   Name="Morgana",    PrimaryRole="Support", Roles=new[]{"Support","Mid"} },
            new() { Id="Fiora",     Name="Fiora",      PrimaryRole="Top",     Roles=new[]{"Top"} },
            new() { Id="Amumu",     Name="Amumu",      PrimaryRole="Jungle",  Roles=new[]{"Jungle","Support"} },
            new() { Id="Jhin",      Name="Jhin",       PrimaryRole="Bot",     Roles=new[]{"Bot"} },
            new() { Id="Nautilus",  Name="Nautilus",   PrimaryRole="Support", Roles=new[]{"Support"} },
            new() { Id="Irelia",    Name="Irelia",     PrimaryRole="Top",     Roles=new[]{"Top","Mid"} },
            new() { Id="Ekko",      Name="Ekko",       PrimaryRole="Jungle",  Roles=new[]{"Jungle","Mid"} },
        };
    }
}
