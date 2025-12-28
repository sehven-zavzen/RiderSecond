using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneDistance = 2f;
    public float moveSpeed = 15f; 
    public LayerMask trackLayer; 
    
    private int targetLane = 1; // 0: Sol, 1: Orta, 2: Sağ

    void Update()
    {
        // Girdileri sadece burada yakalıyoruz
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            if (targetLane > 0) targetLane--;
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            if (targetLane < 2) targetLane++;
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        
        // 1. ADIM: Hedef şeridin merkezden ne kadar uzakta olduğunu hesapla (-2, 0, 2)
        float desiredLaneOffset = (targetLane - 1) * laneDistance;

        // 2. ADIM: Raycast'i Player'ın olduğu bölgeyi kapsayacak şekilde yukarıdan atıyoruz
        // X koordinatını 0 yerine transform.position.x yaparak virajlarda yolu kaçırmasını engelliyoruz
        Vector3 rayOrigin = new Vector3(transform.position.x, 25f, 0); 
        Debug.DrawRay(rayOrigin, Vector3.down * 50f, Color.cyan);

        // Track katmanındaki yolu bul
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 50f, trackLayer))
        {
            // Yolun o andaki fiziksel merkez/yüzey X koordinatı
            float roadSurfaceX = hit.point.x;

            // Player'ın olması gereken gerçek X: Yolun yüzeyi + Şerit farkı
            float finalTargetX = roadSurfaceX + desiredLaneOffset;

            // Yüksekliği yoldan al (Hafif bir offset ile yola gömülmeyi engelle)
            float targetY = hit.point.y + 0.5f;

            // 3. ADIM: Yumuşak Hareket Uygulama
            // Z daima 0 çünkü yol bize doğru akıyor (Sonsuz yol illüzyonu)
            Vector3 targetPos = new Vector3(finalTargetX, targetY, 0);
            
            // X ve Y pozisyonlarını aynı anda yumuşakça takip et
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);

            // 4. ADIM: Rotasyon (Eğim)
            // hit.normal ile yolun dik açısını alıyoruz
            Quaternion slopeRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            Vector3 euler = slopeRot.eulerAngles;
            
            // Sadece X (yokuş) ve Z (viraj yatışı) eğimlerini kullan
            // Audiosurf hissi için Y (sağa sola bakma) sabit kalmalı
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler.x, 0, euler.z), Time.deltaTime * 10f);
        }
    }
}