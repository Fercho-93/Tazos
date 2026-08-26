// Bootstrap.cs — Monta el prototipo entero por código al arrancar, en cualquier escena.
// Así no hay ninguna escena que mantener: iterar es cambiar un número y darle a Play.
using UnityEngine;

namespace TazosKanto
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Launch()
        {
            if (Object.FindFirstObjectByType<GameSession>() != null) return;

            ConfigurePhysics();
            ConfigureScreen();

            var table = BuildTable();
            var cam = BuildCamera();
            BuildLights();

            var root = new GameObject("TazosKanto");
            var session = root.AddComponent<GameSession>();
            session.Camera = cam;
            root.AddComponent<Feel>();

            var launch = new GameObject("LaunchPoint").transform;
            launch.SetParent(root.transform);
            launch.position = new Vector3(0f, 0.055f, -0.62f);
            session.LaunchPoint = launch;

            var thrower = root.AddComponent<ThrowController>();
            thrower.Session = session;
            thrower.LaunchPoint = launch;

            var hud = root.AddComponent<Hud>();
            hud.Session = session;
            hud.Throw = thrower;

            session.StartMatch();
        }

        static void ConfigurePhysics()
        {
            // Un disco de 4 mm a 14 m/s recorre 23 cm por fotograma a 60 Hz.
            // 120 Hz + colisión continua es el mínimo para que la cuña de la palanca exista.
            Time.fixedDeltaTime = 1f / 120f;
            Physics.defaultSolverIterations = 12;
            Physics.defaultSolverVelocityIterations = 4;
            Physics.defaultContactOffset = 0.0005f;   // 0,5 mm: acorde a la escala del tazo
            Physics.sleepThreshold = 0.0005f;
            Physics.bounceThreshold = 0.35f;
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
        }

        static void ConfigureScreen()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            Screen.orientation = ScreenOrientation.Portrait;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        static GameObject BuildTable()
        {
            var table = GameObject.CreatePrimitive(PrimitiveType.Cube);
            table.name = "Mesa";
            table.transform.localScale = new Vector3(1.4f, 0.06f, 1.6f);
            table.transform.position = new Vector3(0f, -0.03f, 0f);
            table.GetComponent<MeshRenderer>().sharedMaterial =
                TazoAssets.TazoMaterial(new Color(0.30f, 0.24f, 0.18f), faceDown: false);
            table.GetComponent<Collider>().material = TazoAssets.PhysicMaterial(0.55f, 0.10f);

            // Bordes: que un tiro pasado no pierda el tazo fuera de cámara.
            BuildWall(new Vector3(0f, 0.03f, 0.82f), new Vector3(1.4f, 0.12f, 0.04f));
            BuildWall(new Vector3(0f, 0.03f, -0.82f), new Vector3(1.4f, 0.12f, 0.04f));
            BuildWall(new Vector3(0.72f, 0.03f, 0f), new Vector3(0.04f, 0.12f, 1.6f));
            BuildWall(new Vector3(-0.72f, 0.03f, 0f), new Vector3(0.04f, 0.12f, 1.6f));
            return table;
        }

        static void BuildWall(Vector3 pos, Vector3 scale)
        {
            var w = GameObject.CreatePrimitive(PrimitiveType.Cube);
            w.name = "Borde";
            w.transform.position = pos;
            w.transform.localScale = scale;
            w.GetComponent<MeshRenderer>().sharedMaterial =
                TazoAssets.TazoMaterial(new Color(0.22f, 0.17f, 0.13f), faceDown: false);
            w.GetComponent<Collider>().material = TazoAssets.PhysicMaterial(0.5f, 0.25f);
        }

        static CameraRig BuildCamera()
        {
            var go = new GameObject("Camera");
            var cam = go.AddComponent<Camera>();
            cam.backgroundColor = new Color(0.09f, 0.10f, 0.13f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.nearClipPlane = 0.02f;
            cam.farClipPlane = 20f;
            go.tag = "MainCamera";
            go.AddComponent<AudioListener>();
            return go.AddComponent<CameraRig>();
        }

        static void BuildLights()
        {
            var key = new GameObject("KeyLight").AddComponent<Light>();
            key.type = LightType.Directional;
            key.transform.rotation = Quaternion.Euler(52f, -28f, 0f);
            key.intensity = 1.15f;
            key.color = new Color(1f, 0.96f, 0.88f);
            key.shadows = LightShadows.Soft;
            key.shadowStrength = 0.65f;

            var fill = new GameObject("FillLight").AddComponent<Light>();
            fill.type = LightType.Directional;
            fill.transform.rotation = Quaternion.Euler(20f, 150f, 0f);
            fill.intensity = 0.35f;
            fill.color = new Color(0.75f, 0.82f, 1f);
            fill.shadows = LightShadows.None;

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.28f, 0.28f, 0.33f);
        }
    }
}
