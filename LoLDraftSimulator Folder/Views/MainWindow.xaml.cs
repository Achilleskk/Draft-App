using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using LoLDraftSimulator.Models;
using LoLDraftSimulator.Services;
using LoLDraftSimulator.ViewModels;

namespace LoLDraftSimulator
{
    public partial class MainWindow : Window
    {
        private readonly DataLoader          _loader;
        private readonly DraftEngine         _engine;
        private readonly CounterpickDatabase _db;

        private List<Champion>          _allChampions;
        private List<ChampionPoolEntry> _championPool;
        private DraftState              _draftState;

        private ObservableCollection<ChampionViewModel>  _filteredChampions = new();
        private ObservableCollection<ChampionPoolEntry>  _filteredPool      = new();
        private ObservableCollection<EnemyPickViewModel> _enemyPicks        = new();

        // Champion en cours de sélection (avant validation)
        private ChampionViewModel _pendingChamp = null;

        private int _actionIndex = 0;

        private static readonly bool[] DraftSequence =
        {
            true,  true,  true,  false, false, false,
            true,  false, false, true,  true,  false, false, true, true, false,
            false, false, true,  true,
            true,  false, false, true
        };
        private static readonly bool[] IsBanSequence =
        {
            true,  true,  true,  true,  true,  true,
            false, false, false, false, false, false, false, false, false, false,
            true,  true,  true,  true,
            false, false, false, false
        };

        public MainWindow()
        {
            InitializeComponent();
            _loader       = new DataLoader();
            _allChampions = _loader.LoadAllChampions();
            _championPool = _loader.LoadChampionPool();
            _draftState   = new DraftState();
            _db           = _loader.LoadCounterpickDatabase();
            _engine       = new DraftEngine(_loader.LoadMetaTiers(), _db);

            // Bind les collections ObservableCollection du DraftState directement
            BlueBansControl.ItemsSource       = _draftState.BlueTeamBans;
            RedBansControl.ItemsSource        = _draftState.RedTeamBans;
            BluePicksControl.ItemsSource      = _draftState.BlueTeamPicks;
            RedPicksControl.ItemsSource       = _enemyPicks;
            ChampionGrid.ItemsSource          = _filteredChampions;
            ChampionPoolList.ItemsSource      = _filteredPool;
            PoolChampionPicker.ItemsSource    = _allChampions;
            PoolChampionPicker.DisplayMemberPath = "Name";

            ApplyChampionFilter();
            RefreshDraftUI();
            RefreshPoolUI();
        }

        // ════════════════════════════════════════════════════════
        // DRAFT LOGIC
        // ════════════════════════════════════════════════════════

        // Étape 1 : clic sur un champion → sélection en attente
        private void Champion_Click(object sender, RoutedEventArgs e)
        {
            if (_actionIndex >= DraftSequence.Length) return;
            var vm = (ChampionViewModel)((Button)sender).Tag;

            // Désélectionner l'ancien
            if (_pendingChamp != null) _pendingChamp.IsSelected = false;

            _pendingChamp = vm;
            vm.IsSelected = true;

            // Afficher le champion sélectionné
            SelectedChampName.Text = vm.Name;
            SelectedChampName.Foreground = System.Windows.Media.Brushes.White;

            bool isBan = IsBanSequence[_actionIndex];
            bool isBlue = DraftSequence[_actionIndex];
            string team = isBlue ? "Blue" : "Red";
            string action = isBan ? "BAN" : "PICK";
            SelectedChampAction.Text = $"→ {team} {action}";

            try
            {
                SelectedChampImage.Source = new BitmapImage(new Uri(vm.IconPath));
            }
            catch { }

            ValidateBtn.IsEnabled = true;
        }

        // Étape 2 : clic sur Valider → confirme le ban/pick
        private void ValidateAction_Click(object sender, RoutedEventArgs e)
        {
            if (_pendingChamp == null || _actionIndex >= DraftSequence.Length) return;

            var champ   = _pendingChamp.Champion;
            bool isBlue = DraftSequence[_actionIndex];
            bool isBan  = IsBanSequence[_actionIndex];

            if (isBan)
            {
                if (isBlue) _draftState.BlueTeamBans.Add(champ);
                else         _draftState.RedTeamBans.Add(champ);
            }
            else
            {
                if (isBlue)
                {
                    _draftState.BlueTeamPicks.Add(champ);
                }
                else
                {
                    _draftState.RedTeamPicks.Add(champ);
                    _enemyPicks.Add(new EnemyPickViewModel(champ, _engine.GuessRoles(champ.Id)));
                }
            }

            // Retirer de la grille
            _filteredChampions.Remove(_pendingChamp);

            // Reset sélection
            _pendingChamp = null;
            ValidateBtn.IsEnabled = false;
            SelectedChampName.Text = "Aucun champion sélectionné";
            SelectedChampName.Foreground = new System.Windows.Media.SolidColorBrush(
                System.Windows.Media.Color.FromRgb(0x55, 0x55, 0x6A));
            SelectedChampAction.Text = "";
            SelectedChampImage.Source = null;

            _actionIndex++;
            RefreshDraftUI();
            UpdateBlindPicks();
        }

        private void RefreshDraftUI()
        {
            // Slots bans vides
            EmptyBlueBansControl.ItemsSource =
                Enumerable.Range(0, Math.Max(0, 5 - _draftState.BlueTeamBans.Count)).ToList();
            EmptyRedBansControl.ItemsSource  =
                Enumerable.Range(0, Math.Max(0, 5 - _draftState.RedTeamBans.Count)).ToList();

            // Slots picks vides
            EmptyBluePicksControl.ItemsSource =
                Enumerable.Range(0, Math.Max(0, 5 - _draftState.BlueTeamPicks.Count)).ToList();
            EmptyRedPicksControl.ItemsSource  =
                Enumerable.Range(0, Math.Max(0, 5 - _draftState.RedTeamPicks.Count)).ToList();

            if (_actionIndex < DraftSequence.Length)
            {
                bool isBlueTurn = DraftSequence[_actionIndex];
                bool isBan      = IsBanSequence[_actionIndex];
                string team     = isBlueTurn ? "🔵 BLUE" : "🔴 RED";
                string action   = isBan ? "BAN" : "PICK";
                PhaseLabel.Text  = $"{team} — {action}";
                ActionLabel.Text = $"{_actionIndex + 1} / 20";
                StatusBar.Text   = $"Action {_actionIndex + 1}/20 — {team} : clique un champion puis VALIDER";
            }
            else
            {
                PhaseLabel.Text  = "✅ DRAFT TERMINÉE";
                ActionLabel.Text = "20 / 20";
                StatusBar.Text   = "Draft complète !";
                ValidateBtn.IsEnabled = false;
            }
        }

        private void UpdateBlindPicks()
        {
            var role = (RoleFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (role == "Tous les rôles") role = null;

            BlindPickRoleLabel.Text = string.IsNullOrEmpty(role) ? "— Tous rôles" : $"— {role}";

            int count = int.TryParse(
                (RecoCountCombo?.SelectedItem as ComboBoxItem)?.Content?.ToString(), out int n) ? n : 3;

            // Exclure les champions déjà utilisés/bannis
            var taken = _draftState.BlueTeamBans
                .Concat(_draftState.RedTeamBans)
                .Concat(_draftState.BlueTeamPicks)
                .Concat(_draftState.RedTeamPicks)
                .Select(c => c.Id).ToHashSet();

            var available = _allChampions.Where(c => !taken.Contains(c.Id)).ToList();

            var recs = _engine.GetTopRecommendations(
                _draftState, available, _championPool, role, count);

            // Priorité aux blind picks, sinon tous
            var blindOnly = recs.Where(r => r.IsBlind).ToList();
            BlindPicksControl.ItemsSource = blindOnly.Count > 0 ? blindOnly : recs;
        }

        private void ResetDraft_Click(object sender, RoutedEventArgs e)
        {
            _actionIndex  = 0;
            _pendingChamp = null;
            _enemyPicks.Clear();

            // Recréer un DraftState frais et rebinder
            _draftState = new DraftState { IsUserBlueSide = BlueSideRadio.IsChecked == true };
            BlueBansControl.ItemsSource  = _draftState.BlueTeamBans;
            RedBansControl.ItemsSource   = _draftState.RedTeamBans;
            BluePicksControl.ItemsSource = _draftState.BlueTeamPicks;

            ValidateBtn.IsEnabled = false;
            SelectedChampName.Text = "Aucun champion sélectionné";
            SelectedChampAction.Text = "";
            SelectedChampImage.Source = null;
            BlindPicksControl.ItemsSource = null;

            ApplyChampionFilter();
            RefreshDraftUI();
            StatusBar.Text = "Draft réinitialisée.";
        }

        // ════════════════════════════════════════════════════════
        // FILTRES
        // ════════════════════════════════════════════════════════
        private void ApplyChampionFilter()
        {
            if (_draftState == null || _allChampions == null) return;

            var search = SearchBox?.Text;
            var role   = (RoleFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var taken = _draftState.BlueTeamBans
                .Concat(_draftState.RedTeamBans)
                .Concat(_draftState.BlueTeamPicks)
                .Concat(_draftState.RedTeamPicks)
                .Select(c => c.Id).ToHashSet();

            var filtered = _allChampions
                .Where(c => !taken.Contains(c.Id))
                .Where(c => string.IsNullOrEmpty(search) || search == "Rechercher..."
                         || c.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Where(c => role == null || role == "Tous les rôles"
                         || (c.Roles != null && c.Roles.Contains(role)))
                .OrderBy(c => c.Name)
                .Select(c => new ChampionViewModel(c, _engine.IsBlindPick(c.Id)))
                .ToList();

            _filteredChampions.Clear();
            foreach (var vm in filtered) _filteredChampions.Add(vm);

            UpdateBlindPicks();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (SearchBox.Text == "Rechercher...")
            {
                SearchBox.Text = "";
                SearchBox.Foreground = System.Windows.Media.Brushes.White;
            }
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                SearchBox.Text = "Rechercher...";
                SearchBox.Foreground = new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(0x7A, 0x7A, 0x9A));
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
            => ApplyChampionFilter();

        private void RoleFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
            => ApplyChampionFilter();

        // ════════════════════════════════════════════════════════
        // CHAMPION POOL
        // ════════════════════════════════════════════════════════
        private void MasterySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MasteryValueLabel == null) return;
            int val = (int)e.NewValue;
            MasteryValueLabel.Text = val.ToString();
            if (MasteryDescLabel != null)
                MasteryDescLabel.Text = val switch
                {
                    1 or 2  => "Novice",
                    3 or 4  => "Apprenti",
                    5 or 6  => "Compétent",
                    7 or 8  => "Expert",
                    9 or 10 => "One-trick 🏆",
                    _       => ""
                };
        }

        private void AddToPool_Click(object sender, RoutedEventArgs e)
        {
            if (PoolChampionPicker.SelectedItem is not Champion champ)
            {
                MessageBox.Show("Sélectionne un champion.", "Champion manquant",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var role    = (PoolRolePicker.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Mid";
            int mastery = (int)MasterySlider.Value;
            var existing = _championPool.FirstOrDefault(p => p.ChampionId == champ.Id && p.Role == role);
            if (existing != null)
            {
                existing.Mastery    = mastery;
                existing.IsMainPick = MainPickCheck.IsChecked == true;
            }
            else
            {
                _championPool.Add(new ChampionPoolEntry
                {
                    ChampionId   = champ.Id,
                    ChampionName = champ.Name,
                    Role         = role,
                    Mastery      = mastery,
                    IsMainPick   = MainPickCheck.IsChecked == true
                });
            }
            RefreshPoolUI();
            StatusBar.Text = $"{champ.Name} ({role}) — {mastery}/10 ajouté !";
        }

        private void RemoveFromPool_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button)?.Tag is ChampionPoolEntry entry)
            {
                _championPool.Remove(entry);
                RefreshPoolUI();
            }
        }

        private void SavePool_Click(object sender, RoutedEventArgs e)
        {
            _loader.SaveChampionPool(_championPool);
            StatusBar.Text = "Champion Pool sauvegardé !";
            MessageBox.Show("Sauvegardé ✔", "Pool", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void PoolRoleFilter_Changed(object sender, SelectionChangedEventArgs e)
            => RefreshPoolUI();

        private void RefreshPoolUI()
        {
            if (_championPool == null || _filteredPool == null) return;
            var roleFilter = (PoolRoleFilter?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            var filtered   = _championPool
                .Where(p => roleFilter == null || roleFilter == "Tous les rôles" || p.Role == roleFilter)
                .OrderByDescending(p => p.Mastery).ThenBy(p => p.ChampionName).ToList();
            _filteredPool.Clear();
            foreach (var entry in filtered) _filteredPool.Add(entry);
            if (PoolStatsLabel != null)
            {
                var lines = _championPool.GroupBy(p => p.Role).OrderBy(g => g.Key)
                    .Select(g => $"{g.Key}  ·  {g.Count()} champ(s)  ·  moy {g.Average(p => p.Mastery):F1}/10");
                PoolStatsLabel.Text = lines.Any() ? string.Join("\n", lines) : "Aucun champion.";
            }
        }
    }
}
