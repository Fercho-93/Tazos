// GameSession.cs — El bucle de una partida: reparto, 5 tiros, juicio y recuento.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TazosKanto
{
    public class GameSession : MonoBehaviour
    {
        public Transform LaunchPoint;
        public CameraRig Camera;

        public int ThrowsLeft { get; private set; }
        public int CollectedThisMatch { get; private set; }
        public string LastCombo { get; private set; } = "";
        public bool MatchOver => ThrowsLeft <= 0 && !_resolving;
        public bool CanThrow => ThrowsLeft > 0 && !_resolving;
        public readonly List<int> CollectedNumbers = new List<int>();

        /// <summary>El tazo con el que lanza el jugador. Los 151 valen; estos cinco
        /// son los del prototipo porque cubren los cinco estilos de tiro.</summary>
        public static readonly int[] Loadout = { 143, 74, 123, 25, 150 }; // Snorlax, Geodude, Scyther, Pikachu, Mewtwo
        public int LoadoutIndex;

        readonly List<Tazo> _table = new List<Tazo>();
        Tazo _projectile;
        bool _resolving;

        public void StartMatch()
        {
            foreach (var t in _table) if (t) Destroy(t.gameObject);
            _table.Clear();
            if (_projectile) Destroy(_projectile.gameObject);

            CollectedNumbers.Clear();
            CollectedThisMatch = 0;
            LastCombo = "";
            ThrowsLeft = GameTuning.ThrowsPerMatch;
            _resolving = false;

            Deal();
        }

        /// <summary>Pila central ligeramente desordenada + satélites sueltos alrededor.
        /// Todos boca abajo: la gracia es darles la vuelta.</summary>
        void Deal()
        {
            var used = new HashSet<int>();
            int Pick()
            {
                int n;
                do { n = Random.Range(1, Pokedex.Count + 1); } while (!used.Add(n));
                return n;
            }

            float y = 0f;
            for (int i = 0; i < GameTuning.StackSize; i++)
            {
                int number = Pick();
                var p = TazoProfile.ForSpecies(number);
                Vector2 jitter = Random.insideUnitCircle * 0.006f;
                y += p.Thickness + 0.0004f;
                var t = Spawn(number, new Vector3(jitter.x, y, jitter.y), Random.Range(0f, 360f), faceDown: true);
                _table.Add(t);
            }

            for (int i = 0; i < GameTuning.LooseTazos; i++)
            {
                int number = Pick();
                float ang = Random.Range(0f, Mathf.PI * 2f);
                float dist = Random.Range(0.10f, 0.26f);
                var pos = new Vector3(Mathf.Cos(ang) * dist, 0.004f, Mathf.Sin(ang) * dist);
                _table.Add(Spawn(number, pos, Random.Range(0f, 360f), faceDown: true));
            }
        }

        Tazo Spawn(int number, Vector3 pos, float yaw, bool faceDown)
        {
            var profile = TazoProfile.ForSpecies(number);
            var go = new GameObject(Pokedex.Get(number).Label);
            go.transform.position = pos;
            go.transform.rotation = Quaternion.Euler(faceDown ? 180f : 0f, yaw, 0f);

            var mesh = TazoMeshFactory.Build(profile);
            var mf = go.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;
            var mr = go.AddComponent<MeshRenderer>();
            mr.sharedMaterial = TazoAssets.TazoMaterial(profile.Tint, faceDown);

            var col = go.AddComponent<MeshCollider>();
            col.sharedMesh = mesh;
            col.convex = true;
            col.material = TazoAssets.PhysicMaterial(profile.Friction, profile.Bounciness);

            var rb = go.AddComponent<Rigidbody>();
            var tazo = go.AddComponent<Tazo>();
            tazo.Configure(number, profile);
            rb.linearVelocity = Vector3.zero;
            return tazo;
        }

        public void Launch(float azimuthDeg, float elevationDeg, float tiltDeg, float power, float spin)
        {
            if (!CanThrow) return;

            if (_projectile) Destroy(_projectile.gameObject);
            int number = Loadout[LoadoutIndex % Loadout.Length];
            _projectile = Spawn(number, LaunchPoint.position, 0f, faceDown: false);
            _projectile.IsProjectile = true;

            Quaternion yaw = Quaternion.Euler(0f, azimuthDeg, 0f);
            Vector3 flat = yaw * Vector3.forward;
            Vector3 dir = Quaternion.AngleAxis(-elevationDeg, Vector3.Cross(Vector3.up, flat)) * flat;

            // La inclinación es un giro del disco sobre su eje de vuelo: el canto
            // que entra primero baja, y eso es lo que permite colarse en la cuña.
            _projectile.transform.rotation = Quaternion.AngleAxis(tiltDeg, flat) * yaw;

            float speed = Mathf.Lerp(GameTuning.MinSpeed, GameTuning.MaxSpeed, power);
            _projectile.Body.linearVelocity = dir * speed;
            _projectile.Body.angularVelocity = _projectile.transform.up * spin;

            ThrowsLeft--;
            Camera.Follow(_projectile.transform);
            StartCoroutine(ResolveTurn());
        }

        IEnumerator ResolveTurn()
        {
            _resolving = true;
            yield return new WaitForSeconds(0.4f);

            float timeout = Time.time + 8f;
            while (Time.time < timeout && !EverythingSettled())
                yield return new WaitForSeconds(0.1f);

            int flipped = 0;
            foreach (var t in _table)
            {
                if (t == null || t.Collected || !t.FaceUp) continue;
                t.Collected = true;
                flipped++;
                CollectedNumbers.Add(t.Number);
                Collection.Register(t.Number);
                TazoAssets.MarkCollected(t);
            }

            CollectedThisMatch += flipped;
            LastCombo = ComboName(flipped);
            if (flipped > 0) Feel.OnFlips(flipped);

            Camera.Follow(null);
            _resolving = false;
        }

        bool EverythingSettled()
        {
            foreach (var t in Tazo.Live)
                if (t != null && !t.Settled) return false;
            return true;
        }

        static string ComboName(int n)
        {
            switch (n)
            {
                case 0: return "";
                case 1: return "Flip!";
                case 2: return "DOUBLE FLIP";
                case 3: return "TRIPLE FLIP";
                case 4: return "QUAD FLIP";
                default: return "AVALANCHE x" + n;
            }
        }
    }
}
