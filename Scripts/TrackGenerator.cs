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
        // 2. Nokta: Başlangıcı düz tutmak için ileri bir nokta (Burada yol düz kalır)
        controlPoints.Add(new Vector3(0, 0, 10)); 
        // 3. Nokta: Artık kıvrım başlayabilir
        controlPoints.Add(new Vector3(5, 10, 30));
        controlPoints.Add(new Vector3(-5, -5, 50));
        controlPoints.Add(new Vector3(0, 0, 70));
        
        GenerateTrack();
    }

    void GenerateTrack()
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>(); // Şeritler için UV şart
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

        // Mesh Oluşturma
        for (int i = 0; i < smoothPoints.Count; i++)
        {
            Vector3 forward = (i < smoothPoints.Count - 1) ? (smoothPoints[i + 1] - smoothPoints[i]).normalized : (smoothPoints[i] - smoothPoints[i - 1]).normalized;
            Vector3 normal = Vector3.up; 
            Vector3 right = Vector3.Cross(normal, forward).normalized;
            // Eğer yol çok dikleşirse yamulmaması için:
            if (forward.y > 0.9f || forward.y < -0.9f) normal = Vector3.forward;

            // Vertexleri ekle (Sağ ve Sol kenar)
            vertices.Add(smoothPoints[i] - right * (roadWidth / 2f));
            vertices.Add(smoothPoints[i] + right * (roadWidth / 2f));

            // UV'leri ekle (Texture'ın nasıl görüneceği)
            // x: 0 sol, 1 sağ kenar | y: i ise yol boyunca uzanma
            uvs.Add(new Vector2(0, i * 0.5f)); 
            uvs.Add(new Vector2(1, i * 0.5f));

            if (i < smoothPoints.Count - 1)
            {
                int root = i * 2;
                triangles.Add(root); triangles.Add(root + 2); triangles.Add(root + 1);
                triangles.Add(root + 1); triangles.Add(root + 2); triangles.Add(root + 3);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;

        // 1. Objenin üzerinde MeshCollider var mı kontrol et, yoksa ekle
        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null) 
        {
            collider = gameObject.AddComponent<MeshCollider>();
        }

        // 2. Oluşturduğumuz pürüzsüz mesh'i collider'a ata
        collider.sharedMesh = mesh;
        
        // 3. Önemli: Katmanı kodla da garantiye alalım
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