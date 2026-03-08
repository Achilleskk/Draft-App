namespace LoLDraftSimulator.Models
{
    public class Champion
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PrimaryRole { get; set; }
        public string[] Roles { get; set; }

        // Chemin vers le fichier image sur le disque (relatif à l'exe)
        public string IconPath =>
            System.IO.Path.Combine(
                System.AppDomain.CurrentDomain.BaseDirectory,
                "Assets", "Icons", $"{Id}.png");
    }
}