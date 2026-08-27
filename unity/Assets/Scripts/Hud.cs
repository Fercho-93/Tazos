// Hud.cs — HUD de prototipo con IMGUI. Feo a propósito: en la Fase 0 lo único que
// importa es leer el estado y poder tocar los números de física en el móvil.
using UnityEngine;

namespace TazosKanto
{
    public class Hud : MonoBehaviour
    {
        public GameSession Session;
        public ThrowController Throw;
        bool _showTuning;

        void OnGUI()
        {
            float s = Screen.height / 900f;                 // escala por DPI
            GUI.matrix = Matrix4x4.Scale(new Vector3(s, s, 1f));
            float w = Screen.width / s;
            var big = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold };
            var mid = new GUIStyle(GUI.skin.label) { fontSize = 22 };

            int loadout = GameSession.Loadout[Session.LoadoutIndex % GameSession.Loadout.Length];
            GUI.Label(new Rect(20, 16, w - 40, 40),
                $"Tiros: {Session.ThrowsLeft}    Conseguidos: {Session.CollectedThisMatch}    Pokédex: {Collection.Total}/151", big);
            GUI.Label(new Rect(20, 56, w - 40, 32), "Lanzador: " + Pokedex.Get(loadout).Label, mid);

            if (!string.IsNullOrEmpty(Session.LastCombo))
            {
                var combo = new GUIStyle(GUI.skin.label)
                { fontSize = 46, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                GUI.Label(new Rect(0, 130, w, 60), Session.LastCombo, combo);
            }

            if (Throw.Aiming)
            {
                GUI.Label(new Rect(20, 200, w - 40, 32),
                    $"Potencia {Throw.Power * 100f:0}%   Ángulo {Throw.Elevation:0}°   " +
                    $"Inclinación {Throw.Tilt:0}°   Spin {Throw.Spin:0.0}", mid);
                DrawBar(new Rect(20, 236, (w - 40) * Throw.Power, 14));
            }

            float bottom = 900f - 150f;
            if (GUI.Button(new Rect(20, bottom, 210, 60), "Cambiar tazo"))
                Session.LoadoutIndex++;
            if (GUI.Button(new Rect(240, bottom, 210, 60), "Nueva partida"))
                Session.StartMatch();
            if (GUI.Button(new Rect(w - 130, bottom, 110, 60), _showTuning ? "Cerrar" : "Física"))
                _showTuning = !_showTuning;

            if (Session.MatchOver)
            {
                var over = new GUIStyle(GUI.skin.label)
                { fontSize = 34, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
                GUI.Label(new Rect(0, 300, w, 50), $"Fin: {Session.CollectedThisMatch} Pokémon", over);
                string list = "";
                foreach (int n in Session.CollectedNumbers) list += Pokedex.Get(n).Label + "   ";
                GUI.Label(new Rect(40, 350, w - 80, 200),
                    list, new GUIStyle(GUI.skin.label) { fontSize = 20, wordWrap = true });
            }

            if (_showTuning) DrawTuning(w);
        }

        void DrawTuning(float w)
        {
            GUI.Box(new Rect(w - 400, 260, 380, 358), "Física en vivo");
            float y = 300f;
            GameTuning.LeverBoost = Slider(w, ref y, "Palanca (1 = física pura)", GameTuning.LeverBoost, 1f, 4f);
            GameTuning.EdgeTumbleBoost = Slider(w, ref y, "Vuelco en canto (1 = física pura)", GameTuning.EdgeTumbleBoost, 1f, 5f);
            GameTuning.MaxSpeed = Slider(w, ref y, "Velocidad máx (m/s)", GameTuning.MaxSpeed, 6f, 22f);
            GameTuning.MaxSpin = Slider(w, ref y, "Spin máx (rad/s)", GameTuning.MaxSpin, 0f, 60f);
            GameTuning.MaxTilt = Slider(w, ref y, "Inclinación máx (°)", GameTuning.MaxTilt, 0f, 80f);
            if (GUI.Button(new Rect(w - 380, y + 10, 200, 44), "Borrar Pokédex")) Collection.Reset();
        }

        static float Slider(float w, ref float y, string label, float value, float min, float max)
        {
            GUI.Label(new Rect(w - 380, y, 340, 24), $"{label}: {value:0.00}");
            float v = GUI.HorizontalSlider(new Rect(w - 380, y + 26, 340, 20), value, min, max);
            y += 58f;
            return v;
        }

        static void DrawBar(Rect r)
        {
            GUI.color = new Color(1f, 0.85f, 0.2f);
            GUI.DrawTexture(r, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
