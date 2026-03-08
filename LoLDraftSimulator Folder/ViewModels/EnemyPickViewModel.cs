using System.Collections.Generic;
using System.Linq;
using LoLDraftSimulator.Models;

namespace LoLDraftSimulator.ViewModels
{
    /// <summary>
    /// Champion ennemi enrichi avec les rôles probables devinés depuis la base counterpicks.
    /// </summary>
    public class EnemyPickViewModel
    {
        private readonly Champion _champion;

        public EnemyPickViewModel(Champion champion, List<string> guessedRoles)
        {
            _champion    = champion;
            GuessedRoles = guessedRoles ?? new();
        }

        public string   Id          => _champion.Id;
        public string   Name        => _champion.Name;
        public string   PrimaryRole => _champion.PrimaryRole;
        public string   IconPath    => _champion.IconPath;
        public Champion Champion    => _champion;

        // Rôles potentiels (ex: ["Top", "Mid"])
        public List<string> GuessedRoles { get; }

        // Affichage compact : "Top / Mid"
        public string RolesLabel => GuessedRoles.Count > 0
            ? string.Join(" / ", GuessedRoles)
            : PrimaryRole;

        // Couleur par rôle principal
        public string RoleColor => GuessedRoles.FirstOrDefault() switch
        {
            "Top"     => "#F0A030",
            "Jungle"  => "#44BB44",
            "Mid"     => "#8888FF",
            "Bot"     => "#FF6666",
            "Support" => "#44CCCC",
            _         => "#AAAAAA"
        };

        // Emojis de rôles pour l'affichage icône
        public string RolesEmoji => string.Join("", GuessedRoles.Select(r => r switch
        {
            "Top"     => "⬆",
            "Jungle"  => "🌲",
            "Mid"     => "⚡",
            "Bot"     => "🎯",
            "Support" => "💊",
            _         => "?"
        }));
    }
}
