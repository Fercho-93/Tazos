// PokedexData.cs — Los 151 Pokémon de Kanto y su arquetipo de tazo.
// Datos puros: sin lógica de juego. El arquetipo determina el perfil físico
// (masa, grosor, canto, fricción, rebote) en TazoProfile.
using System.Collections.Generic;

namespace TazosKanto
{
    /// <summary>Familia física de tazo. No es un tipo Pokémon: es cómo se comporta el disco.</summary>
    public enum Archetype
    {
        Feather,    // F - patina y salta, impredecible
        Light,      // L - preciso, poco daño
        Balanced,   // B - todoterreno
        Dense,      // D - empuja sin descontrolarse
        Heavy,      // H - revienta pilas
        Rock,       // R - no rebota, arrasa
        Blade,      // S - muy fino, rey de la palanca
        Precision   // P - spin estable, trayectoria limpia
    }

    public readonly struct Species
    {
        public readonly int Number;
        public readonly string Name;
        public readonly Archetype Archetype;

        public Species(int number, string name, Archetype archetype)
        {
            Number = number;
            Name = name;
            Archetype = archetype;
        }

        /// <summary>"001 Bulbasaur"</summary>
        public string Label => Number.ToString("000") + " " + Name;
    }

    public static class Pokedex
    {
        public const int Count = 151;

        /// <summary>Índice 0 = #001 Bulbasaur ... índice 150 = #151 Mew.</summary>
        public static readonly IReadOnlyList<Species> All = new[]
        {
            new Species(1, "Bulbasaur", Archetype.Balanced),
            new Species(2, "Ivysaur", Archetype.Balanced),
            new Species(3, "Venusaur", Archetype.Dense),
            new Species(4, "Charmander", Archetype.Light),
            new Species(5, "Charmeleon", Archetype.Balanced),
            new Species(6, "Charizard", Archetype.Dense),
            new Species(7, "Squirtle", Archetype.Balanced),
            new Species(8, "Wartortle", Archetype.Balanced),
            new Species(9, "Blastoise", Archetype.Dense),
            new Species(10, "Caterpie", Archetype.Light),
            new Species(11, "Metapod", Archetype.Dense),
            new Species(12, "Butterfree", Archetype.Feather),
            new Species(13, "Weedle", Archetype.Light),
            new Species(14, "Kakuna", Archetype.Dense),
            new Species(15, "Beedrill", Archetype.Blade),
            new Species(16, "Pidgey", Archetype.Feather),
            new Species(17, "Pidgeotto", Archetype.Feather),
            new Species(18, "Pidgeot", Archetype.Feather),
            new Species(19, "Rattata", Archetype.Light),
            new Species(20, "Raticate", Archetype.Balanced),
            new Species(21, "Spearow", Archetype.Feather),
            new Species(22, "Fearow", Archetype.Feather),
            new Species(23, "Ekans", Archetype.Light),
            new Species(24, "Arbok", Archetype.Balanced),
            new Species(25, "Pikachu", Archetype.Light),
            new Species(26, "Raichu", Archetype.Balanced),
            new Species(27, "Sandshrew", Archetype.Balanced),
            new Species(28, "Sandslash", Archetype.Blade),
            new Species(29, "Nidoran♀", Archetype.Light),
            new Species(30, "Nidorina", Archetype.Balanced),
            new Species(31, "Nidoqueen", Archetype.Dense),
            new Species(32, "Nidoran♂", Archetype.Light),
            new Species(33, "Nidorino", Archetype.Balanced),
            new Species(34, "Nidoking", Archetype.Dense),
            new Species(35, "Clefairy", Archetype.Light),
            new Species(36, "Clefable", Archetype.Balanced),
            new Species(37, "Vulpix", Archetype.Light),
            new Species(38, "Ninetales", Archetype.Balanced),
            new Species(39, "Jigglypuff", Archetype.Feather),
            new Species(40, "Wigglytuff", Archetype.Feather),
            new Species(41, "Zubat", Archetype.Feather),
            new Species(42, "Golbat", Archetype.Feather),
            new Species(43, "Oddish", Archetype.Light),
            new Species(44, "Gloom", Archetype.Balanced),
            new Species(45, "Vileplume", Archetype.Balanced),
            new Species(46, "Paras", Archetype.Light),
            new Species(47, "Parasect", Archetype.Balanced),
            new Species(48, "Venonat", Archetype.Light),
            new Species(49, "Venomoth", Archetype.Feather),
            new Species(50, "Diglett", Archetype.Light),
            new Species(51, "Dugtrio", Archetype.Balanced),
            new Species(52, "Meowth", Archetype.Light),
            new Species(53, "Persian", Archetype.Balanced),
            new Species(54, "Psyduck", Archetype.Balanced),
            new Species(55, "Golduck", Archetype.Balanced),
            new Species(56, "Mankey", Archetype.Light),
            new Species(57, "Primeape", Archetype.Balanced),
            new Species(58, "Growlithe", Archetype.Balanced),
            new Species(59, "Arcanine", Archetype.Dense),
            new Species(60, "Poliwag", Archetype.Light),
            new Species(61, "Poliwhirl", Archetype.Balanced),
            new Species(62, "Poliwrath", Archetype.Dense),
            new Species(63, "Abra", Archetype.Light),
            new Species(64, "Kadabra", Archetype.Precision),
            new Species(65, "Alakazam", Archetype.Precision),
            new Species(66, "Machop", Archetype.Balanced),
            new Species(67, "Machoke", Archetype.Dense),
            new Species(68, "Machamp", Archetype.Dense),
            new Species(69, "Bellsprout", Archetype.Light),
            new Species(70, "Weepinbell", Archetype.Light),
            new Species(71, "Victreebel", Archetype.Balanced),
            new Species(72, "Tentacool", Archetype.Light),
            new Species(73, "Tentacruel", Archetype.Dense),
            new Species(74, "Geodude", Archetype.Rock),
            new Species(75, "Graveler", Archetype.Rock),
            new Species(76, "Golem", Archetype.Rock),
            new Species(77, "Ponyta", Archetype.Balanced),
            new Species(78, "Rapidash", Archetype.Balanced),
            new Species(79, "Slowpoke", Archetype.Dense),
            new Species(80, "Slowbro", Archetype.Dense),
            new Species(81, "Magnemite", Archetype.Precision),
            new Species(82, "Magneton", Archetype.Precision),
            new Species(83, "Farfetch'd", Archetype.Blade),
            new Species(84, "Doduo", Archetype.Light),
            new Species(85, "Dodrio", Archetype.Balanced),
            new Species(86, "Seel", Archetype.Dense),
            new Species(87, "Dewgong", Archetype.Dense),
            new Species(88, "Grimer", Archetype.Dense),
            new Species(89, "Muk", Archetype.Heavy),
            new Species(90, "Shellder", Archetype.Blade),
            new Species(91, "Cloyster", Archetype.Rock),
            new Species(92, "Gastly", Archetype.Feather),
            new Species(93, "Haunter", Archetype.Feather),
            new Species(94, "Gengar", Archetype.Balanced),
            new Species(95, "Onix", Archetype.Rock),
            new Species(96, "Drowzee", Archetype.Balanced),
            new Species(97, "Hypno", Archetype.Dense),
            new Species(98, "Krabby", Archetype.Blade),
            new Species(99, "Kingler", Archetype.Blade),
            new Species(100, "Voltorb", Archetype.Feather),
            new Species(101, "Electrode", Archetype.Feather),
            new Species(102, "Exeggcute", Archetype.Light),
            new Species(103, "Exeggutor", Archetype.Dense),
            new Species(104, "Cubone", Archetype.Light),
            new Species(105, "Marowak", Archetype.Balanced),
            new Species(106, "Hitmonlee", Archetype.Balanced),
            new Species(107, "Hitmonchan", Archetype.Balanced),
            new Species(108, "Lickitung", Archetype.Dense),
            new Species(109, "Koffing", Archetype.Feather),
            new Species(110, "Weezing", Archetype.Feather),
            new Species(111, "Rhyhorn", Archetype.Rock),
            new Species(112, "Rhydon", Archetype.Rock),
            new Species(113, "Chansey", Archetype.Balanced),
            new Species(114, "Tangela", Archetype.Balanced),
            new Species(115, "Kangaskhan", Archetype.Dense),
            new Species(116, "Horsea", Archetype.Light),
            new Species(117, "Seadra", Archetype.Balanced),
            new Species(118, "Goldeen", Archetype.Light),
            new Species(119, "Seaking", Archetype.Balanced),
            new Species(120, "Staryu", Archetype.Blade),
            new Species(121, "Starmie", Archetype.Blade),
            new Species(122, "Mr.", Archetype.Blade),
            new Species(123, "Scyther", Archetype.Blade),
            new Species(124, "Jynx", Archetype.Balanced),
            new Species(125, "Electabuzz", Archetype.Balanced),
            new Species(126, "Magmar", Archetype.Balanced),
            new Species(127, "Pinsir", Archetype.Blade),
            new Species(128, "Tauros", Archetype.Heavy),
            new Species(129, "Magikarp", Archetype.Light),
            new Species(130, "Gyarados", Archetype.Heavy),
            new Species(131, "Lapras", Archetype.Heavy),
            new Species(132, "Ditto", Archetype.Balanced),
            new Species(133, "Eevee", Archetype.Light),
            new Species(134, "Vaporeon", Archetype.Balanced),
            new Species(135, "Jolteon", Archetype.Light),
            new Species(136, "Flareon", Archetype.Balanced),
            new Species(137, "Porygon", Archetype.Precision),
            new Species(138, "Omanyte", Archetype.Blade),
            new Species(139, "Omastar", Archetype.Blade),
            new Species(140, "Kabuto", Archetype.Blade),
            new Species(141, "Kabutops", Archetype.Blade),
            new Species(142, "Aerodactyl", Archetype.Blade),
            new Species(143, "Snorlax", Archetype.Heavy),
            new Species(144, "Articuno", Archetype.Precision),
            new Species(145, "Zapdos", Archetype.Precision),
            new Species(146, "Moltres", Archetype.Precision),
            new Species(147, "Dratini", Archetype.Light),
            new Species(148, "Dragonair", Archetype.Balanced),
            new Species(149, "Dragonite", Archetype.Dense),
            new Species(150, "Mewtwo", Archetype.Precision),
            new Species(151, "Mew", Archetype.Feather),
        };

        public static Species Get(int number) => All[number - 1];

        public static int IndexOf(string name)
        {
            for (int i = 0; i < All.Count; i++)
                if (All[i].Name == name) return i;
            return -1;
        }
    }
}
