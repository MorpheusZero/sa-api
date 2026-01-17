namespace SoulArenasAPI.Util;

public class UsernameGenerator
{
    private static readonly Random _random = new Random();
    private static readonly string[] _adjectives =
    [
        "Swift", "Brave", "Clever", "Mighty", "Fierce", "Nimble", "Bold", "Wise", "Loyal", "Silent",
        "Dark", "Frozen", "Shadow", "Azure", "Golden", "Silver", "Mystic", "Raging", "Wild", "Noble",
        "Savage", "Epic", "Divine", "Iron", "Steel", "Deadly", "Lethal", "Fatal", "Doom", "Chaos",
        "Cosmic", "Lunar", "Solar", "Storm", "Venom", "Toxic", "Cursed", "Bright", "Dread", "Grim",
        "Primal", "Royal", "Arcane", "Feral", "Bleak", "Ashen", "Neon", "Omega", "Alpha", "Prime"
    ];

    private static readonly string[] _nouns =
    [
        "Lion", "Eagle", "Wolf", "Tiger", "Dragon", "Bear", "Shark", "Falcon", "Knight", "Hunter",
        "Ranger", "Ninja", "Titan", "Demon", "Angel", "Spirit", "Reaper", "Slayer", "Viper", "Cobra",
        "Raven", "Hawk", "Sphinx", "Hydra", "Kraken", "Blaze", "Storm", "Meteor", "Ghost", "Blade",
        "Fang", "Claw", "Scythe", "Spear", "Sword", "Axe", "Mage", "Monk", "Wrath", "Forge",
        "Frost", "Flame", "Beast", "Wyrm", "Drake", "Fiend", "Golem", "Spectre", "Wraith", "Ogre"
    ];

    public static string GenerateUsername()
    {
        var adjective = _adjectives[_random.Next(_adjectives.Length)];
        var noun = _nouns[_random.Next(_nouns.Length)];
        var number = _random.Next(100, 999);

        return $"{adjective}{noun}#{number}";
    }
}
