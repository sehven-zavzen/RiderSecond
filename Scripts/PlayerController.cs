using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneDistance = 2f;
    public float moveSpeed = 10f;
    public LayerMask trackLayer; // Inspector'dan "Track" katmanını seçeceğiz
    
    private int targetLane = 1; 
    private Vector3 targetPosition;

    void Update()
    {
        // Şerit değiştirme kontrolleri
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            if (targetLane > 0) targetLane--;
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            if (targetLane < 2) targetLane++;

        // Hedef X ve Z pozisyonu (Z sabit çünkü yol akıyor)
        float targetX = (targetLane - 1) * laneDistance;
        targetPosition = new Vector3(targetX, transform.position.y, 0);

        // Yatay hareket
        Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        transform.position = new Vector3(newPos.x, transform.position.y, 0);
    }

    // void FixedUpdate()
    // {
    //     RaycastHit hit;
    //     // Işını Player'ın biraz yukarısından (Vector3.up * 2) aşağıya gönderiyoruz
    //     if (Physics.Raycast(transform.position + Vector3.up * 2f, Vector3.down, out hit, 10f, trackLayer))
    //     {
    //         // Sadece yolu bulduğunda Y pozisyonunu güncelle
    //         float groundY = hit.point.y + 0.5f; 
    //         transform.position = new Vector3(transform.position.x, groundY, transform.position.z);

    //         // Eğim ayarı
    //         Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
    //         transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * 10f);
    //     }
    // }

    void FixedUpdate()
    {
        RaycastHit hit;
        // Ray'i Player'ın tam merkezinden değil, biraz yukarısından atıyoruz
        Vector3 origin = transform.position;
        origin.y += 2.0f; 

        // Görsel hata ayıklama: Sahne ekranında kırmızı çizgiyi takip et
        Debug.DrawRay(origin, Vector3.down * 5f, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out hit, 10f, trackLayer))
        {
            // 1. Yükseklik Sabitleme
            float targetY = hit.point.y + 0.5f;
            // Y pozisyonunu sarsıntısız takip etmesi için yumuşatıyoruz
            float smoothY = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * 20f);
            transform.position = new Vector3(transform.position.x, smoothY, transform.position.z);

            // 2. Rotasyon (Eğim) Sabitleme
            // Sadece ileri bakmasını ve yolun eğimine (Normal) uymasını sağlıyoruz
            Quaternion targetRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            
            // ÖNEMLİ: Player'ın sağa sola dönmesini (Y ekseni) engellemek için:
            Vector3 euler = targetRot.eulerAngles;
            // Sadece X (iniş-çıkış) eğimini al, Y (sağ-sol) ve Z (yatış) 0 kalsın
            transform.rotation = Quaternion.Euler(euler.x, 0, 0); 
        }
    }
}