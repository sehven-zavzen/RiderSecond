using UnityEngine;

[ExecuteInEditMode] // Editörde de sonucu görebilmek için
public class ShaderManager : MonoBehaviour
{
    public Transform playerTransform;

    void Update()
    {
        if (playerTransform != null)
        {
            // Shader'daki o özel ismi bulur ve Player'ın pozisyonunu global olarak basar
            Shader.SetGlobalVector("_PlayerWorldPos", playerTransform.position);
        }
    }
}