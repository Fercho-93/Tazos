// TazoProfile.cs — Traduce un arquetipo a parámetros físicos reales del disco.
// Todo en unidades SI. Cambiar estos números cambia el juego; no hay otra capa oculta.
using UnityEngine;

namespace TazosKanto
{
    public struct TazoProfile
    {
        public float Radius;        // m
        public float Thickness;     // m
        public float Mass;          // kg
        public float Bevel;         // 0..1 - cuánto se afila el canto (1 = filo de cuchilla)
        public float Dome;          // m - alzada del centro respecto al borde. Crea el hueco de la palanca.
        public float Friction;
        public float Bounciness;
        public float DragCoef;      // arrastre aerodinámico sobre el área proyectada
        public Color Tint;

        public const float BaseRadius = 0.040f;      // 4 cm, tazo clásico
        public const float BaseThickness = 0.0040f;  // 4 mm
        public const float BaseMass = 0.0075f;       // 7,5 g

        public static TazoProfile For(Archetype a)
        {
            // masa, grosor, bisel, domo(mm), fricción, rebote, arrastre, color
            switch (a)
            {
                case Archetype.Feather:
                    return Make(0.55f, 0.80f, 0.45f, 0.9f, 0.22f, 0.55f, 1.35f, new Color(0.75f, 0.80f, 0.95f));
                case Archetype.Light:
                    return Make(0.75f, 0.95f, 0.50f, 0.7f, 0.34f, 0.38f, 1.10f, new Color(0.98f, 0.85f, 0.30f));
                case Archetype.Balanced:
                    return Make(1.00f, 1.00f, 0.45f, 0.6f, 0.40f, 0.30f, 1.00f, new Color(0.85f, 0.85f, 0.88f));
                case Archetype.Dense:
                    return Make(1.30f, 1.25f, 0.35f, 0.5f, 0.48f, 0.20f, 0.90f, new Color(0.40f, 0.60f, 0.85f));
                case Archetype.Heavy:
                    return Make(1.80f, 1.55f, 0.25f, 0.4f, 0.60f, 0.10f, 0.80f, new Color(0.25f, 0.30f, 0.45f));
                case Archetype.Rock:
                    return Make(1.60f, 1.35f, 0.30f, 0.4f, 0.72f, 0.06f, 0.85f, new Color(0.55f, 0.45f, 0.35f));
                case Archetype.Blade:
                    // Muy fino y con canto agresivo: entra donde nadie entra.
                    return Make(0.80f, 0.55f, 0.95f, 0.8f, 0.24f, 0.18f, 1.05f, new Color(0.55f, 0.85f, 0.55f));
                case Archetype.Precision:
                    return Make(1.00f, 1.00f, 0.50f, 0.6f, 0.42f, 0.16f, 0.95f, new Color(0.70f, 0.55f, 0.90f));
                default:
                    return Make(1f, 1f, 0.45f, 0.6f, 0.40f, 0.30f, 1.0f, Color.white);
            }
        }

        static TazoProfile Make(float massMul, float thickMul, float bevel, float domeMm,
                                float friction, float bounce, float drag, Color tint)
        {
            return new TazoProfile
            {
                Radius = BaseRadius,
                Thickness = BaseThickness * thickMul,
                Mass = BaseMass * massMul,
                Bevel = bevel,
                Dome = domeMm * 0.001f,
                Friction = friction,
                Bounciness = bounce,
                DragCoef = drag,
                Tint = tint
            };
        }

        /// <summary>Perfil de una especie concreta, con los retoques a mano de los tazos emblemáticos.</summary>
        public static TazoProfile ForSpecies(int number)
        {
            var species = Pokedex.Get(number);
            var p = For(species.Archetype);

            switch (species.Name)
            {
                case "Snorlax":   // el demoledor: masa bruta, cero rebote
                    p.Mass *= 1.25f; p.Bounciness = 0.05f; p.Friction = 0.65f; p.Bevel = 0.15f; break;
                case "Geodude":   // pesado y con mordida: no resbala, empuja
                    p.Mass *= 1.10f; p.Friction = 0.80f; p.Bounciness = 0.04f; break;
                case "Scyther":   // el palanquero: el canto más fino del juego
                    p.Thickness *= 0.80f; p.Bevel = 1.0f; p.Friction = 0.20f; break;
                case "Pikachu":   // ligero y preciso: poco daño, mucho control
                    p.DragCoef = 0.85f; p.Bounciness = 0.32f; break;
                case "Mewtwo":    // el equilibrio perfecto, bueno en todo, óptimo en nada
                    p.Mass = BaseMass * 1.05f; p.Friction = 0.44f; p.Bounciness = 0.18f; p.DragCoef = 0.82f; break;
            }
            return p;
        }
    }
}
