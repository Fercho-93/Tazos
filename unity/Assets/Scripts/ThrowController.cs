// ThrowController.cs — Los cinco canales de control con un solo pulgar.
//   dirección  = ángulo del arrastre (se lanza al contrario, como un tirachinas)
//   potencia   = longitud del arrastre
//   elevación  = componente vertical del flick al soltar
//   inclinación= componente lateral de ese mismo flick
//   spin       = curvatura acumulada del recorrido del dedo
using System.Collections.Generic;
using UnityEngine;

namespace TazosKanto
{
    public class ThrowController : MonoBehaviour
    {
        public GameSession Session;
        public Transform LaunchPoint;

        struct Sample { public Vector2 Pos; public float Time; }

        readonly List<Sample> _path = new List<Sample>();
        Vector2 _start;
        bool _dragging;

        // Estado leído por el HUD para dibujar la previsualización.
        public bool Aiming => _dragging;
        public float Power { get; private set; }
        public float Azimuth { get; private set; }
        public float Elevation { get; private set; }
        public float Tilt { get; private set; }
        public float Spin { get; private set; }

        void Update()
        {
            if (!Session.CanThrow) { _dragging = false; return; }

            if (TryGetPointer(out Vector2 pos, out bool down, out bool up))
            {
                if (down) BeginDrag(pos);
                else if (_dragging) ContinueDrag(pos);
                if (up && _dragging) Release(pos);
            }
        }

        void BeginDrag(Vector2 pos)
        {
            _dragging = true;
            _start = pos;
            _path.Clear();
            _path.Add(new Sample { Pos = pos, Time = Time.unscaledTime });
        }

        void ContinueDrag(Vector2 pos)
        {
            _path.Add(new Sample { Pos = pos, Time = Time.unscaledTime });
            if (_path.Count > 128) _path.RemoveAt(0);
            Evaluate(pos);
        }

        void Release(Vector2 pos)
        {
            _dragging = false;
            Evaluate(pos);
            if (Power < 0.08f) return;   // toque suelto: no cuenta como tiro
            Session.Launch(Azimuth, Elevation, Tilt, Power, Spin);
        }

        void Evaluate(Vector2 pos)
        {
            Vector2 drag = pos - _start;
            float full = Screen.height * GameTuning.DragForFullPower;
            Power = Mathf.Clamp01(drag.magnitude / full);

            // Se lanza en sentido contrario al arrastre.
            Vector2 aim = -drag;
            Azimuth = (aim.sqrMagnitude > 1f) ? Mathf.Atan2(aim.x, aim.y) * Mathf.Rad2Deg : 0f;

            // Flick: velocidad del dedo en los últimos milisegundos.
            Vector2 flick = FlickVelocity();
            float norm = Screen.height;                      // px/s → fracción de pantalla/s
            float lift = Mathf.Clamp01(-flick.y / (norm * 2.5f));   // hacia abajo = globo
            Elevation = Mathf.Lerp(2f, GameTuning.MaxElevation, lift);
            Tilt = Mathf.Clamp(flick.x / (norm * 1.6f), -1f, 1f) * GameTuning.MaxTilt;

            // Spin: cuánto ha girado el vector de arrastre a lo largo del recorrido.
            Spin = Mathf.Clamp(PathCurvature(), -1f, 1f) * GameTuning.MaxSpin;
        }

        Vector2 FlickVelocity()
        {
            if (_path.Count < 2) return Vector2.zero;
            float now = _path[_path.Count - 1].Time;
            int i = _path.Count - 1;
            while (i > 0 && now - _path[i].Time < GameTuning.FlickWindow) i--;
            float dt = Mathf.Max(1e-3f, now - _path[i].Time);
            return (_path[_path.Count - 1].Pos - _path[i].Pos) / dt;
        }

        /// <summary>Suma de los giros del vector de arrastre, normalizada. Un arrastre
        /// recto da 0; una "C" da spin fuerte, con el signo del lado hacia el que curva.</summary>
        float PathCurvature()
        {
            if (_path.Count < 4) return 0f;
            float total = 0f, length = 0f;
            for (int i = 2; i < _path.Count; i++)
            {
                Vector2 a = _path[i - 1].Pos - _path[i - 2].Pos;
                Vector2 b = _path[i].Pos - _path[i - 1].Pos;
                if (a.sqrMagnitude < 4f || b.sqrMagnitude < 4f) continue;
                total += Mathf.Sign(a.x * b.y - a.y * b.x) * Vector2.Angle(a, b);
                length += b.magnitude;
            }
            if (length < 20f) return 0f;
            return total / 180f;
        }

        static bool TryGetPointer(out Vector2 pos, out bool down, out bool up)
        {
            if (Input.touchCount > 0)
            {
                Touch t = Input.GetTouch(0);
                pos = t.position;
                down = t.phase == TouchPhase.Began;
                up = t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled;
                return true;
            }
            // Ratón: iterar en el editor sin tener que compilar al móvil cada vez.
            pos = Input.mousePosition;
            down = Input.GetMouseButtonDown(0);
            up = Input.GetMouseButtonUp(0);
            return down || up || Input.GetMouseButton(0);
        }
    }
}
