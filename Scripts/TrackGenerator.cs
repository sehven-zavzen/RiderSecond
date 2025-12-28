using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrackGenerator : MonoBehaviour
{
    public List<Vector3> controlPoints = new List<Vector3>();
    public int segmentsPerPoint = 20; // Ne kadar yüksekse o kadar pürüzsüz (Smooth)
    public float roadWidth = 6f;

    void Start()
    {
        controlPoints.Clear();

        // 1. Nokta: Başlangıç (Sıfır noktası)
        controlPoints.Add(new Vector3(0, 0, 0));
        // 2. Nokta: Başlangıcı düz tutmak için ileri bir nokta
        controlPoints.Add(new Vector3(0, 0, 10));
        // 3+ Noktalar: Kıvrım başlayabilir
        controlPoints.Add(new Vector3(5, 10, 30));
        controlPoints.Add(new Vector3(-5, -5, 50));
        controlPoints.Add(new Vector3(0, 0, 70));

        GenerateTrack();
    }

    void GenerateTrack()
    {
        Mesh mesh = new Mesh();
        mesh.name = "ProceduralTrack";

        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<Vector2> uv2s = new List<Vector2>(); // <-- UV2: düz hedef X burada
        List<int> triangles = new List<int>();

        // Pürüzsüz yolu hesapla
        List<Vector3> smoothPoints = new List<Vector3>();
        for (int i = 0; i < controlPoints.Count - 1; i++)
        {
            for (int j = 0; j < segmentsPerPoint; j++)
            {
                float t = j / (float)segmentsPerPoint;
                smoothPoints.Add(GetCatmullRomPosition(t, i));
            }
        }

        // Mesh oluşturma
        for (int i = 0; i < smoothPoints.Count; i++)
        {
            Vector3 forward =
                (i < smoothPoints.Count - 1)
                ? (smoothPoints[i + 1] - smoothPoints[i]).normalized
                : (smoothPoints[i] - smoothPoints[i - 1]).normalized;

            // Diklik durumuna göre normal seç (sonra right hesapla)
            Vector3 normal = Vector3.up;
            if (Mathf.Abs(forward.y) > 0.9f)
                normal = Vector3.forward;

            Vector3 right = Vector3.Cross(normal, forward).normalized;

            // Sağ / sol kenar vertexleri
            Vector3 leftV = smoothPoints[i] - right * (roadWidth / 2f);
            Vector3 rightV = smoothPoints[i] + right * (roadWidth / 2f);

            vertices.Add(leftV);
            vertices.Add(rightV);

            // UV0 (texture)
            uvs.Add(new Vector2(0, i * 0.5f));
            uvs.Add(new Vector2(1, i * 0.5f));

            // UV2.x = düz hedef X (object space)
            // Shader yakınlaşınca vertex x'i buna doğru çekecek.
            uv2s.Add(new Vector2(-roadWidth / 2f, 0f));
            uv2s.Add(new Vector2(+roadWidth / 2f, 0f));

            // Triangles
            if (i < smoothPoints.Count - 1)
            {
                int root = i * 2;

                triangles.Add(root);
                triangles.Add(root + 2);
                triangles.Add(root + 1);

                triangles.Add(root + 1);
                triangles.Add(root + 2);
                triangles.Add(root + 3);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(1, uv2s);         // <-- UV2 set
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;

        // Collider
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null)
            collider = gameObject.AddComponent<MeshCollider>();

        collider.sharedMesh = null;   // bazen refresh için iyi olur
        collider.sharedMesh = mesh;

        // Layer
        gameObject.layer = LayerMask.NameToLayer("Track");
    }

    // Pürüzsüz Eğri Matematiği
    Vector3 GetCatmullRomPosition(float t, int index)
    {
        int count = controlPoints.Count;

        Vector3 p0 = controlPoints[Mathf.Max(index - 1, 0)];
        Vector3 p1 = controlPoints[index];
        Vector3 p2 = controlPoints[Mathf.Min(index + 1, count - 1)];
        Vector3 p3 = controlPoints[Mathf.Min(index + 2, count - 1)];

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
}
