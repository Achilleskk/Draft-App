using System.ComponentModel;
using System.Runtime.CompilerServices;
using LoLDraftSimulator.Models;

namespace LoLDraftSimulator.ViewModels
{
    public class ChampionViewModel : INotifyPropertyChanged
    {
        private readonly Champion _champion;
        private bool _isSelected;

        public ChampionViewModel(Champion champion, bool isBlindPick)
        {
            _champion   = champion;
            IsBlindPick = isBlindPick;
        }

        public string   Id          => _champion.Id;
        public string   Name        => _champion.Name;
        public string   PrimaryRole => _champion.PrimaryRole;
        public string[] Roles       => _champion.Roles;
        public string   IconPath    => _champion.IconPath;
        public Champion Champion    => _champion;
        public bool     IsBlindPick { get; }
        public string   ToolTip     => IsBlindPick ? $"{Name}  🛡 Blind safe" : Name;

        public bool IsSelected
        {
            get => _isSelected;
            set { _isSelected = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
