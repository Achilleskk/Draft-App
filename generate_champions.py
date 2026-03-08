"""
generate_champions.py
─────────────────────
Lance ce script depuis le dossier racine de ton projet LoLDraftSimulator.
Il lit tous les .png dans Assets/Icons/ et génère Data/champions.json complet.

Usage : python generate_champions.py
"""

import os, json

# ── Rôles connus par champion (Id DataDragon → rôles) ─────────────────────────
ROLES = {
    "Aatrox":        ["Top"],
    "Ahri":          ["Mid"],
    "Akali":         ["Mid", "Top"],
    "Akshan":        ["Mid", "Top"],
    "Alistar":       ["Support"],
    "Ambessa":       ["Top", "Mid"],
    "Amumu":         ["Jungle", "Support"],
    "Anivia":        ["Mid"],
    "Annie":         ["Mid", "Support"],
    "Aphelios":      ["Bot"],
    "Ashe":          ["Bot", "Support"],
    "AurelionSol":   ["Mid"],
    "Aurora":        ["Mid", "Top"],
    "Azir":          ["Mid"],
    "Bard":          ["Support"],
    "Belveth":       ["Jungle"],
    "Blitzcrank":    ["Support"],
    "Brand":         ["Support", "Mid"],
    "Braum":         ["Support"],
    "Briar":         ["Jungle"],
    "Caitlyn":       ["Bot"],
    "Camille":       ["Top"],
    "Cassiopeia":    ["Mid"],
    "Chogath":       ["Top"],
    "Corki":         ["Mid"],
    "Darius":        ["Top"],
    "Diana":         ["Jungle", "Mid"],
    "Draven":        ["Bot"],
    "DrMundo":       ["Top", "Jungle"],
    "Ekko":          ["Jungle", "Mid"],
    "Elise":         ["Jungle"],
    "Evelynn":       ["Jungle"],
    "Ezreal":        ["Bot"],
    "Fiddlesticks":  ["Jungle", "Support"],
    "Fiora":         ["Top"],
    "Fizz":          ["Mid"],
    "Galio":         ["Mid", "Support"],
    "Gangplank":     ["Top"],
    "Garen":         ["Top"],
    "Gnar":          ["Top"],
    "Gragas":        ["Jungle", "Top"],
    "Graves":        ["Jungle"],
    "Gwen":          ["Top"],
    "Hecarim":       ["Jungle"],
    "Heimerdinger":  ["Mid", "Bot", "Support"],
    "Hwei":          ["Mid", "Support"],
    "Illaoi":        ["Top"],
    "Irelia":        ["Top", "Mid"],
    "Ivern":         ["Jungle"],
    "Janna":         ["Support"],
    "JarvanIV":      ["Jungle"],
    "Jax":           ["Top", "Jungle"],
    "Jayce":         ["Top", "Mid"],
    "Jhin":          ["Bot"],
    "Jinx":          ["Bot"],
    "Kaisa":         ["Bot"],
    "Kalista":       ["Bot"],
    "Karma":         ["Support", "Mid"],
    "Karthus":       ["Jungle", "Mid"],
    "Kassadin":      ["Mid"],
    "Katarina":      ["Mid"],
    "Kayle":         ["Top"],
    "Kayn":          ["Jungle"],
    "Kennen":        ["Top"],
    "Khazix":        ["Jungle"],
    "Kindred":       ["Jungle"],
    "Kled":          ["Top"],
    "KogMaw":        ["Bot"],
    "KSante":        ["Top"],
    "Leblanc":       ["Mid"],
    "LeeSin":        ["Jungle"],
    "Leona":         ["Support"],
    "Lillia":        ["Jungle"],
    "Lissandra":     ["Mid"],
    "Lucian":        ["Bot", "Mid"],
    "Lulu":          ["Support", "Top"],
    "Lux":           ["Support", "Mid"],
    "Malphite":      ["Top", "Support"],
    "Malzahar":      ["Mid"],
    "Maokai":        ["Top", "Support"],
    "MasterYi":      ["Jungle"],
    "Milio":         ["Support"],
    "MissFortune":   ["Bot"],
    "MonkeyKing":    ["Jungle", "Top"],
    "Mordekaiser":   ["Top"],
    "Morgana":       ["Support", "Mid"],
    "Naafiri":       ["Mid"],
    "Nami":          ["Support"],
    "Nasus":         ["Top"],
    "Nautilus":      ["Support"],
    "Neeko":         ["Mid", "Support"],
    "Nidalee":       ["Jungle"],
    "Nilah":         ["Bot"],
    "Nocturne":      ["Jungle"],
    "Nunu":          ["Jungle"],
    "Olaf":          ["Jungle", "Top"],
    "Orianna":       ["Mid"],
    "Ornn":          ["Top"],
    "Pantheon":      ["Support", "Top", "Mid"],
    "Poppy":         ["Top", "Jungle"],
    "Pyke":          ["Support"],
    "Qiyana":        ["Mid"],
    "Quinn":         ["Top"],
    "Rakan":         ["Support"],
    "Rammus":        ["Jungle"],
    "RekSai":        ["Jungle"],
    "Rell":          ["Support"],
    "Renata":        ["Support"],
    "Renekton":      ["Top"],
    "Rengar":        ["Jungle", "Top"],
    "Riven":         ["Top"],
    "Rumble":        ["Top", "Mid"],
    "Ryze":          ["Mid", "Top"],
    "Samira":        ["Bot"],
    "Sejuani":       ["Jungle"],
    "Senna":         ["Support", "Bot"],
    "Seraphine":     ["Support", "Mid"],
    "Sett":          ["Top"],
    "Shaco":         ["Jungle"],
    "Shen":          ["Top"],
    "Shyvana":       ["Jungle"],
    "Singed":        ["Top"],
    "Sion":          ["Top"],
    "Sivir":         ["Bot"],
    "Skarner":       ["Jungle"],
    "Smolder":       ["Bot"],
    "Sona":          ["Support"],
    "Soraka":        ["Support"],
    "Swain":         ["Support", "Mid"],
    "Sylas":         ["Mid", "Jungle"],
    "Syndra":        ["Mid"],
    "TahmKench":     ["Support", "Top"],
    "Taliyah":       ["Jungle", "Mid"],
    "Talon":         ["Mid", "Jungle"],
    "Taric":         ["Support"],
    "Teemo":         ["Top"],
    "Thresh":        ["Support"],
    "Tristana":      ["Bot"],
    "Trundle":       ["Jungle", "Top"],
    "Tryndamere":    ["Top"],
    "TwistedFate":   ["Mid"],
    "Twitch":        ["Bot", "Jungle"],
    "Udyr":          ["Jungle"],
    "Urgot":         ["Top"],
    "Varus":         ["Bot", "Mid"],
    "Vayne":         ["Bot", "Top"],
    "Veigar":        ["Mid", "Support"],
    "Velkoz":        ["Support", "Mid"],
    "Vex":           ["Mid"],
    "Vi":            ["Jungle"],
    "Viego":         ["Jungle"],
    "Viktor":        ["Mid"],
    "Vladimir":      ["Mid", "Top"],
    "Volibear":      ["Jungle", "Top"],
    "Warwick":       ["Jungle"],
    "Xayah":         ["Bot"],
    "Xerath":        ["Support", "Mid"],
    "XinZhao":       ["Jungle"],
    "Yasuo":         ["Mid", "Bot"],
    "Yone":          ["Mid", "Top"],
    "Yorick":        ["Top"],
    "Yuumi":         ["Support"],
    "Zac":           ["Jungle"],
    "Zed":           ["Mid"],
    "Zeri":          ["Bot"],
    "Ziggs":         ["Bot", "Mid"],
    "Zilean":        ["Support"],
    "Zoe":           ["Mid"],
    "Zyra":          ["Support"],
}

# Noms d'affichage (quand le nom affiché diffère de l'Id)
DISPLAY_NAMES = {
    "AurelionSol":  "Aurelion Sol",
    "DrMundo":      "Dr. Mundo",
    "JarvanIV":     "Jarvan IV",
    "KogMaw":       "Kog'Maw",
    "KSante":       "K'Sante",
    "LeeSin":       "Lee Sin",
    "MasterYi":     "Master Yi",
    "MissFortune":  "Miss Fortune",
    "MonkeyKing":   "Wukong",
    "RekSai":       "Rek'Sai",
    "Renata":       "Renata Glasc",
    "TahmKench":    "Tahm Kench",
    "TwistedFate":  "Twisted Fate",
    "XinZhao":      "Xin Zhao",
    "Belveth":      "Bel'Veth",
    "Chogath":      "Cho'Gath",
    "Khazix":       "Kha'Zix",
    "Leblanc":      "LeBlanc",
    "Nunu":         "Nunu & Willump",
    "Velkoz":       "Vel'Koz",
}

def main():
    icons_dir = os.path.join("Assets", "Icons")

    if not os.path.isdir(icons_dir):
        print(f"❌ Dossier introuvable : {icons_dir}")
        print("   Lance ce script depuis la racine du projet.")
        return

    png_files = sorted([f for f in os.listdir(icons_dir) if f.endswith(".png")])
    print(f"✅ {len(png_files)} icônes trouvées dans {icons_dir}")

    champions = []
    unknown   = []

    for filename in png_files:
        champ_id = filename.replace(".png", "")
        roles    = ROLES.get(champ_id)

        if roles is None:
            unknown.append(champ_id)
            roles = ["Mid"]  # fallback

        display_name = DISPLAY_NAMES.get(champ_id, champ_id)

        champions.append({
            "Id":          champ_id,
            "Name":        display_name,
            "PrimaryRole": roles[0],
            "Roles":       roles
        })

    # Écriture du fichier
    os.makedirs("Data", exist_ok=True)
    out_path = os.path.join("Data", "champions.json")
    with open(out_path, "w", encoding="utf-8") as f:
        json.dump(champions, f, ensure_ascii=False, indent=2)

    print(f"✅ {len(champions)} champions écrits dans {out_path}")

    if unknown:
        print(f"\n⚠️  {len(unknown)} champions sans rôles définis (mis à 'Mid' par défaut) :")
        for u in unknown:
            print(f"   - {u}")
        print("\nAjoute-les dans le dictionnaire ROLES du script si besoin.")

if __name__ == "__main__":
    main()
