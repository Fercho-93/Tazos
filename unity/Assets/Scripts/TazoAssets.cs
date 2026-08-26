// TazoAssets.cs — Materiales del prototipo, creados por código.
// Placeholder deliberado: los tazos son discos de color hasta la Fase 3.
using System.Collections.Generic;
using UnityEngine;

namespace TazosKanto
{
    public static class TazoAssets
    {
        static readonly Dictionary<int, Material> _materials = new Dictionary<int, Material>();
        static readonly Dictionary<int, PhysicsMaterial> _physics = new Dictionary<int, PhysicsMaterial>();
        static Shader _shader;

        static Shader LitShader => _shader != null
            ? _shader
            : _shader = (Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));

        public static Material TazoMaterial(Color tint, bool faceDown)
        {
            // Cara oculta: dorso neutro. Cara Pokémon: color del arquetipo.
            Color c = faceDown ? new Color(0.62f, 0.58f, 0.52f) : tint;
            int key = c.GetHashCode();
            if (_materials.TryGetValue(key, out var m)) return m;

            m = new Material(LitShader) { enableInstancing = true };
            m.SetColor("_BaseColor", c);
            m.SetColor("_Color", c);
            m.SetFloat("_Smoothness", 0.62f);   // plástico noventero
            m.SetFloat("_Glossiness", 0.62f);
            _materials[key] = m;
            return m;
        }

        /// <summary>Al conseguirlo, el tazo enseña su color: feedback inmediato de "es mío".</summary>
        public static void MarkCollected(Tazo t)
        {
            var mr = t.GetComponent<MeshRenderer>();
            if (mr != null) mr.sharedMaterial = TazoMaterial(t.Profile.Tint, faceDown: false);
        }

        public static PhysicsMaterial PhysicMaterial(float friction, float bounciness)
        {
            int key = Mathf.RoundToInt(friction * 1000f) * 10000 + Mathf.RoundToInt(bounciness * 1000f);
            if (_physics.TryGetValue(key, out var pm)) return pm;

            pm = new PhysicsMaterial("tazo")
            {
                dynamicFriction = friction,
                staticFriction = friction * 1.15f,
                bounciness = bounciness,
                frictionCombine = PhysicsMaterialCombine.Average,
                bounceCombine = PhysicsMaterialCombine.Average
            };
            _physics[key] = pm;
            return pm;
        }
    }
}
