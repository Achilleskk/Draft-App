using System.Collections.ObjectModel;

namespace LoLDraftSimulator.Models
{
    public class DraftState
    {
        // ObservableCollection → WPF se met à jour automatiquement
        public ObservableCollection<Champion> BlueTeamBans  { get; set; } = new();
        public ObservableCollection<Champion> RedTeamBans   { get; set; } = new();
        public ObservableCollection<Champion> BlueTeamPicks { get; set; } = new();
        public ObservableCollection<Champion> RedTeamPicks  { get; set; } = new();
        public bool IsUserBlueSide { get; set; } = true;
    }
}
