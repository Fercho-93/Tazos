// Feel.cs — Cámara lenta, háptica y sonido. El 20% arcade que se ve, no el que se juega.
using System.Collections;
using UnityEngine;

namespace TazosKanto
{
    public class Feel : MonoBehaviour
    {
        static Feel _instance;
        AudioSource _audio;
        float _nextWedgeSfx;

        void Awake()
        {
            _instance = this;
            _audio = gameObject.AddComponent<AudioSource>();
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f;
        }

        /// <summary>Un tazo se ha colado bajo el borde de otro: chasquido seco y toque háptico.</summary>
        public static void OnWedge(Vector3 point)
        {
            if (_instance == null || Time.unscaledTime < _instance._nextWedgeSfx) return;
            _instance._nextWedgeSfx = Time.unscaledTime + 0.08f;
            Haptic(0.25f);
        }

        /// <summary>Final de turno con vuelcos: cámara lenta corta y golpe háptico por flip.</summary>
        public static void OnFlips(int count)
        {
            if (_instance == null) return;
            _instance.StartCoroutine(_instance.FlipBurst(count));
        }

        IEnumerator FlipBurst(int count)
        {
            if (GameTuning.SlowMoEnabled && count >= 2)
            {
                Time.timeScale = 0.35f;
                Time.fixedDeltaTime = 1f / 120f * Time.timeScale;
                yield return new WaitForSecondsRealtime(0.45f + 0.1f * count);
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 1f / 120f;
            }
            for (int i = 0; i < Mathf.Min(count, 5); i++)
            {
                Haptic(0.4f + 0.12f * i);
                yield return new WaitForSecondsRealtime(0.09f);
            }
        }

        static void Haptic(float strength)
        {
            if (!GameTuning.HapticsEnabled) return;
#if UNITY_IOS && !UNITY_EDITOR
            // Fase 1: sustituir por Core Haptics (UIImpactFeedbackGenerator) vía plugin nativo.
            Handheld.Vibrate();
#elif UNITY_ANDROID && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }
    }
}
