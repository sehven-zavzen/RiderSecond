using UnityEngine;

public class TrackScroller : MonoBehaviour
{
    public float scrollSpeed = 5f;
    private Material trackMaterial;
    private Vector2 offset = Vector2.zero;

    void Start()
    {
        // MeshRenderer'dan materyali al
        trackMaterial = GetComponent<MeshRenderer>().material;
    }

    // void Update()
    // {
    //     // Offset değerini hesapla
    //     // Eğer yol ters akıyorsa -= yerine += deneyebilirsin
    //     offset.y -= Time.deltaTime * scrollSpeed;
        
    //     // URP shader'larında ana doku genellikle _BaseMap ismini kullanır
    //     // Bu satır dokuyu fiziksel olarak kaydırır
    //     trackMaterial.SetTextureOffset("_BaseMap", offset);
        
    //     // Standart shader'lar için yedek olarak bırakabilirsin:
    //     // trackMaterial.SetTextureOffset("_MainTex", offset);
    // }

    void Update()
    {
        // Y ekseninde (boyuna) kaydır
        offset.y += Time.deltaTime * scrollSpeed;
        
        // URP kullanıyorsan _BaseMap, Standart ise _MainTex
        trackMaterial.SetTextureOffset("_BaseMap", offset);
    }
}