using UnityEngine;

[ExecuteInEditMode]
public class ShaderGlobalParam : MonoBehaviour
{
    public Transform playerTransform;
    [Range(10, 100)] public float flattenDistance = 30f; // Editörden test edebilmek için buraya aldık
    public float flattenLookAhead = 15f;
    public bool flattenYToZero = true;

    void Update()
    {
        // 1. Pozisyonu belirle
        Vector3 pos = (playerTransform != null) ? playerTransform.position : transform.position;

        // 2. Shader'a Global verileri gönder
        // ÖNEMLİ: Shader kodundaki isimle (alt tire dahil) birebir aynı olmalı
        Shader.SetGlobalVector("_PlayerWorldPos", pos);
        Shader.SetGlobalFloat("_FlattenDistance", flattenDistance);
        Shader.SetGlobalFloat("_FlattenLookAhead", flattenLookAhead);
        Shader.SetGlobalFloat("_FlattenYToZero", flattenYToZero ? 1f : 0f);
    }

    // Sahne ekranında görsel bir yardımcı çizgi çizer (Test için)
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = (playerTransform != null) ? playerTransform.position : transform.position;
        Gizmos.DrawWireSphere(pos, 1f);
    }
}