// GameTuning.cs — Todos los números que cambian la sensación, en un sitio.
// Se pueden tocar en vivo desde el HUD de depuración.
namespace TazosKanto
{
    public static class GameTuning
    {
        // --- Lanzamiento ---
        public static float MinSpeed = 4.0f;    // m/s con potencia 0
        public static float MaxSpeed = 14.0f;   // m/s con potencia 1
        public static float MaxElevation = 35f; // grados
        public static float MaxTilt = 45f;      // grados de inclinación (roll) del disco
        public static float MaxSpin = 25f;      // rad/s

        // --- Gesto ---
        public static float DragForFullPower = 0.30f;  // fracción del alto de pantalla
        public static float FlickWindow = 0.08f;       // s de ventana para leer el flick

        // --- Palanca ---
        // 1.0 = física pura (funciona, pero exige precisión de cirujano).
        // 2.2 = compensa lo que PhysX pierde en cuñas de 4 mm. Es el 20% arcade.
        public static float LeverBoost = 2.2f;

        // --- Partida ---
        public static int ThrowsPerMatch = 5;
        public static int StackSize = 10;
        public static int LooseTazos = 6;

        // --- Presentación ---
        public static bool SlowMoEnabled = true;
        public static bool HapticsEnabled = true;
    }
}
