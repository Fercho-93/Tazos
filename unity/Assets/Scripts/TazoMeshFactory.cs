// TazoMeshFactory.cs — Genera la malla del disco: canto biselado + domo suave.
// La silueta es lo que hace posible la palanca, así que se genera con cuidado
// y el MeshCollider convexo usa exactamente la misma geometría.
using UnityEngine;

namespace TazosKanto
{
    public static class TazoMeshFactory
    {
        const int Segments = 32;   // suficiente para el contacto y barato en móvil

        public static Mesh Build(TazoProfile p)
        {
            float r = p.Radius;
            float h = p.Thickness * 0.5f;
            // El bisel come radio en el canto: cuanto más afilado, más lejos del borde
            // empieza a estrecharse el disco, y más fácil es que otro tazo se cuele debajo.
            float bevelIn = Mathf.Lerp(0.10f, 0.32f, p.Bevel) * r;
            float rimH = h * Mathf.Lerp(0.85f, 0.10f, p.Bevel); // altura del canto

            // Anillos: centro, meseta, inicio de bisel, borde. Espejo arriba/abajo.
            float[] radii = { 0f, r * 0.45f, r - bevelIn, r };
            float[] tops  = { h + p.Dome, h + p.Dome * 0.55f, h, rimH };

            var verts = new System.Collections.Generic.List<Vector3>();
            var tris = new System.Collections.Generic.List<int>();
            var uvs = new System.Collections.Generic.List<Vector2>();

            // Cara superior e inferior (la inferior es el espejo, con domo invertido)
            int topStart = AddCap(verts, uvs, tris, radii, tops, +1);
            int botStart = AddCap(verts, uvs, tris, radii, tops, -1);

            // Canto: une el último anillo de arriba con el de abajo
            int topRim = topStart + 1 + (radii.Length - 2) * Segments;
            int botRim = botStart + 1 + (radii.Length - 2) * Segments;
            for (int s = 0; s < Segments; s++)
            {
                int s2 = (s + 1) % Segments;
                tris.Add(topRim + s); tris.Add(botRim + s); tris.Add(botRim + s2);
                tris.Add(topRim + s); tris.Add(botRim + s2); tris.Add(topRim + s2);
            }

            var mesh = new Mesh { name = "Tazo" };
            mesh.SetVertices(verts);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(tris, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        static int AddCap(System.Collections.Generic.List<Vector3> verts,
                          System.Collections.Generic.List<Vector2> uvs,
                          System.Collections.Generic.List<int> tris,
                          float[] radii, float[] heights, int sign)
        {
            int start = verts.Count;
            verts.Add(new Vector3(0f, heights[0] * sign, 0f));
            uvs.Add(new Vector2(0.5f, 0.5f));

            for (int ring = 1; ring < radii.Length; ring++)
            {
                for (int s = 0; s < Segments; s++)
                {
                    float ang = s / (float)Segments * Mathf.PI * 2f;
                    float x = Mathf.Cos(ang), z = Mathf.Sin(ang);
                    verts.Add(new Vector3(x * radii[ring], heights[ring] * sign, z * radii[ring]));
                    float u = radii[ring] / radii[radii.Length - 1] * 0.5f;
                    uvs.Add(new Vector2(0.5f + x * u, 0.5f + z * u));
                }
            }

            // Abanico central
            int ring1 = start + 1;
            for (int s = 0; s < Segments; s++)
            {
                int s2 = (s + 1) % Segments;
                if (sign > 0) { tris.Add(start); tris.Add(ring1 + s); tris.Add(ring1 + s2); }
                else          { tris.Add(start); tris.Add(ring1 + s2); tris.Add(ring1 + s); }
            }
            // Anillos intermedios
            for (int ring = 1; ring < radii.Length - 1; ring++)
            {
                int a = start + 1 + (ring - 1) * Segments;
                int b = a + Segments;
                for (int s = 0; s < Segments; s++)
                {
                    int s2 = (s + 1) % Segments;
                    if (sign > 0)
                    {
                        tris.Add(a + s); tris.Add(b + s); tris.Add(b + s2);
                        tris.Add(a + s); tris.Add(b + s2); tris.Add(a + s2);
                    }
                    else
                    {
                        tris.Add(a + s); tris.Add(b + s2); tris.Add(b + s);
                        tris.Add(a + s); tris.Add(a + s2); tris.Add(b + s2);
                    }
                }
            }
            return start;
        }
    }
}
