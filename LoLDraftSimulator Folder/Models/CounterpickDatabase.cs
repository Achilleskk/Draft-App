using System.Collections.Generic;

namespace LoLDraftSimulator.Models
{
    public class CounterEntry
    {
        public List<string> CounteredBy { get; set; } = new();
        public List<string> Counters    { get; set; } = new();
    }

    public class CounterpickDatabase
    {
        public List<string>                     BlindPicks  { get; set; } = new();
        public Dictionary<string, CounterEntry> Counters    { get; set; } = new();
        public Dictionary<string, List<string>> RoleGuesses { get; set; } = new();
    }
}
