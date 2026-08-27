// Tazo.cs — Un disco. Rigidbody puro: nada de aquí decide si se voltea,
// sólo aporta aerodinámica y la amplificación del par en contactos de cuña.
using UnityEngine;

namespace TazosKanto
{
    [RequireComponent(typeof(Rigidbody))]
    public class Tazo : MonoBehaviour
    {
        public int Number;                 // 1..151
        public TazoProfile Profile;
        public bool IsProjectile;          // el tazo lanzado por el jugador
        public bool Collected;             // ya contabilizado en un turno anterior

        Rigidbody _rb;
        float _restTimer;

        /// <summary>Referencia global de todos los tazos vivos (16-20 elementos, sin coste).</summary>
        public static readonly System.Collections.Generic.List<Tazo> Live = new System.Collections.Generic.List<Tazo>();

        public Rigidbody Body => _rb;
        public Species Species => Pokedex.Get(Number);

        /// <summary>Boca arriba = la cara del Pokémon mira al cielo.</summary>
        public bool FaceUp => Vector3.Dot(transform.up, Vector3.up) > 0.55f;

        /// <summary>Quieto de verdad durante un rato: ya podemos juzgar su cara.</summary>
        public bool Settled => _restTimer > 0.35f;

        /// <summary>Cuánto se ha levantado un borde respecto a la mesa. Un tazo
        /// medio alzado es un objetivo mucho más fácil para la palanca después.</summary>
        public float TiltDegrees => Vector3.Angle(transform.up, Vector3.up);

        void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            Live.Add(this);
        }

        void OnDestroy() => Live.Remove(this);

        public void Configure(int number, TazoProfile profile)
        {
            Number = number;
            Profile = profile;
            _rb.mass = profile.Mass;
            _rb.linearDamping = 0.04f;
            _rb.angularDamping = 0.06f;
            _rb.sleepThreshold = 0.0005f;             // que no se duerma antes de decidir su cara
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // Un disco fino tiene un tensor de inercia muy plano; lo dejamos que lo calcule
            // PhysX a partir de la malla convexa, que es justo lo que da el cabeceo bonito.
        }

        void FixedUpdate()
        {
            _restTimer = (_rb.linearVelocity.sqrMagnitude < 0.0025f &&
                          _rb.angularVelocity.sqrMagnitude < 0.16f)
                ? _restTimer + Time.fixedDeltaTime
                : 0f;

            if (!IsProjectile && _restTimer > 0f) return;   // los de la mesa no vuelan
            ApplyAerodynamics();
        }

        /// <summary>Arrastre según el área proyectada (de canto vuela, de plano frena)
        /// más una fuerza tipo Magnus que curva el tiro con spin lateral.</summary>
        void ApplyAerodynamics()
        {
            Vector3 v = _rb.linearVelocity;
            float speed = v.magnitude;
            if (speed < 0.2f) return;

            Vector3 dir = v / speed;
            float faceOn = Mathf.Abs(Vector3.Dot(transform.up, dir));   // 1 = de plano contra el aire
            float area = Mathf.PI * Profile.Radius * Profile.Radius * Mathf.Lerp(0.12f, 1f, faceOn);
            const float airDensity = 1.225f;

            Vector3 drag = -dir * (0.5f * airDensity * Profile.DragCoef * area * speed * speed);
            _rb.AddForce(drag, ForceMode.Force);

            Vector3 magnus = Vector3.Cross(_rb.angularVelocity, v) * (Profile.Mass * 0.0016f);
            _rb.AddForce(magnus, ForceMode.Force);
        }

        void OnCollisionEnter(Collision c)
        {
            HandleWedge(c);
            HandleEdgeTumble(c);
        }

        void OnCollisionStay(Collision c)
        {
            HandleWedge(c);
            HandleEdgeTumble(c);
        }

        /// <summary>Palanca. No es un minijuego ni un dado: buscamos contactos que
        /// realmente estén POR DEBAJO del plano medio del otro tazo (o sea, dentro de
        /// la cuña) y amplificamos el par que ese contacto ya está generando.
        /// A 4 mm de grosor y 120 Hz, PhysX se come buena parte de ese empuje.</summary>
        void HandleWedge(Collision c)
        {
            if (!IsProjectile) return;
            var target = c.collider.GetComponentInParent<Tazo>();
            if (target == null || target == this || target.FaceUp) return;

            float boost = GameTuning.LeverBoost;
            if (boost <= 1.001f) return;   // 1.0 = física pura, sin ayuda

            Vector3 up = target.transform.up;
            Vector3 center = target.Body.worldCenterOfMass;

            for (int i = 0; i < c.contactCount; i++)
            {
                var p = c.GetContact(i);
                Vector3 rel = p.point - center;

                float depth = Vector3.Dot(rel, up);                        // <0 = por debajo del plano medio
                float radial = Vector3.ProjectOnPlane(rel, up).magnitude;  // dentro del disco
                if (depth > -target.Profile.Thickness * 0.15f) continue;
                if (radial > target.Profile.Radius * 1.05f) continue;

                // Sólo si venimos entrando: una cuña que ya sale no hace palanca.
                Vector3 relVel = _rb.linearVelocity - target.Body.linearVelocity;
                if (Vector3.Dot(relVel, ProjectHorizontal(rel)) > -0.05f) continue;

                // El fulcro es el borde opuesto; el brazo es el radio hasta el contacto.
                Vector3 impulse = Vector3.up * (relVel.magnitude * _rb.mass * (boost - 1f) * 0.5f);
                target.Body.AddForceAtPosition(impulse, p.point, ForceMode.Impulse);
                _rb.AddForceAtPosition(-impulse * 0.35f, p.point, ForceMode.Impulse);
                Feel.OnWedge(p.point);
                return;   // un empujón de cuña por contacto, no uno por punto
            }
        }

        /// <summary>Compensa la flexión que pierde el disco rígido al recibir un
        /// impacto legítimo en el canto. Sólo añade par al objetivo: nunca impulso
        /// lineal, por lo que no inventa distancia ni velocidad de desplazamiento.</summary>
        void HandleEdgeTumble(Collision c)
        {
            if (!IsProjectile) return;
            var target = c.collider.GetComponentInParent<Tazo>();
            if (target == null || target == this || target.IsProjectile || target.FaceUp) return;

            float boost = GameTuning.EdgeTumbleBoost;
            if (boost <= 1.001f) return;

            Vector3 up = target.transform.up;
            Vector3 center = target.Body.worldCenterOfMass;
            for (int i = 0; i < c.contactCount; i++)
            {
                var p = c.GetContact(i);
                Vector3 arm = p.point - center;
                Vector3 radial = Vector3.ProjectOnPlane(arm, up);
                if (radial.magnitude < target.Profile.Radius * 0.78f) continue;

                Vector3 incoming = Vector3.ProjectOnPlane(_rb.linearVelocity - target.Body.linearVelocity, up);
                if (incoming.sqrMagnitude < 0.01f) continue;
                if (Vector3.Dot(incoming, radial) >= -0.02f) continue;

                Vector3 extraForce = incoming.normalized * (_rb.mass * incoming.magnitude * 0.12f);
                Vector3 extraTorque = Vector3.Cross(arm, extraForce) * (boost - 1f);
                if (extraTorque.sqrMagnitude < 1e-8f) continue;

                target.Body.AddTorque(extraTorque, ForceMode.Impulse);
                return; // Un único empujón de giro por contacto, no uno por punto.
            }
        }

        static Vector3 ProjectHorizontal(Vector3 v)
        {
            v.y = 0f;
            return v.sqrMagnitude > 1e-8f ? v.normalized : Vector3.zero;
        }
    }
}
