namespace LoLDraftSimulator.Models
{
    public class ChampionPoolEntry
    {
        public string ChampionId { get; set; }
        public string ChampionName { get; set; }
        public string Role { get; set; }       // Le rôle sur lequel tu le joues
        public int Mastery { get; set; }       // Note de 1 à 10 (ta maîtrise)
        public bool IsMainPick { get; set; }   // Ton pick principal sur ce rôle

        // Propriété calculée : label lisible pour l'UI
        public string MasteryLabel => Mastery switch
        {
            1 or 2 => "Novice",
            3 or 4 => "Apprenti",
            5 or 6 => "Compétent",
            7 or 8 => "Expert",
            9 or 10 => "One-trick",
            _ => "Non noté"
        };
    }
}
