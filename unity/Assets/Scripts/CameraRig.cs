// CameraRig.cs — Cámara baja en 3/4, pensada para vertical y para una sola mano.
using UnityEngine;

namespace TazosKanto
{
    public class CameraRig : MonoBehaviour
    {
        public Transform Target;          // el tazo en vuelo, si lo hay
        Camera _cam;
        Vector3 _home = new Vector3(0f, 0.62f, -0.86f);
        Vector3 _lookHome = new Vector3(0f, 0.02f, 0.02f);
        Vector3 _lookAt;

        void Awake()
        {
            _cam = GetComponent<Camera>();
            _lookAt = _lookHome;
            transform.position = _home;
        }

        void LateUpdate()
        {
            // FOV corregido por aspecto: en un iPhone vertical, un FOV vertical fijo
            // deja la mesa fuera de plano. Fijamos el FOV horizontal y derivamos el vertical.
            const float horizontalFov = 46f;
            float h = Mathf.Tan(horizontalFov * 0.5f * Mathf.Deg2Rad);
            _cam.fieldOfView = 2f * Mathf.Atan(h / _cam.aspect) * Mathf.Rad2Deg;

            Vector3 desiredLook = Target != null
                ? Vector3.Lerp(_lookHome, Target.position, 0.55f)
                : _lookHome;
            _lookAt = Vector3.Lerp(_lookAt, desiredLook, Time.unscaledDeltaTime * 3.5f);

            Vector3 desiredPos = _home + (Target != null ? new Vector3(_lookAt.x * 0.25f, 0.04f, 0f) : Vector3.zero);
            transform.position = Vector3.Lerp(transform.position, desiredPos, Time.unscaledDeltaTime * 2.5f);
            transform.LookAt(_lookAt);
        }

        public void Follow(Transform t) => Target = t;
    }
}
