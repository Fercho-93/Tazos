// Collection.cs — Pokédex persistente. 151 bits en PlayerPrefs, nada más por ahora.
using System.Text;
using UnityEngine;

namespace TazosKanto
{
    public static class Collection
    {
        const string Key = "tazos.pokedex";
        static bool[] _owned;

        static bool[] Owned
        {
            get
            {
                if (_owned == null)
                {
                    _owned = new bool[Pokedex.Count];
                    string s = PlayerPrefs.GetString(Key, "");
                    for (int i = 0; i < Mathf.Min(s.Length, Pokedex.Count); i++)
                        _owned[i] = s[i] == '1';
                }
                return _owned;
            }
        }

        public static bool Has(int number) => Owned[number - 1];
        public static int Total
        {
            get { int n = 0; foreach (bool b in Owned) if (b) n++; return n; }
        }

        public static void Register(int number)
        {
            if (Owned[number - 1]) return;
            Owned[number - 1] = true;
            Save();
        }

        static void Save()
        {
            var sb = new StringBuilder(Pokedex.Count);
            foreach (bool b in Owned) sb.Append(b ? '1' : '0');
            PlayerPrefs.SetString(Key, sb.ToString());
            PlayerPrefs.Save();
        }

        public static void Reset()
        {
            _owned = new bool[Pokedex.Count];
            Save();
        }
    }
}
