using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float laneDistance = 2f;
    public float moveSpeed = 15f; 
    private int targetLane = 1; 

    void Update()
    {
        // Şerit kontrolü
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            if (targetLane > 0) targetLane--;
        
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            if (targetLane < 2) targetLane++;

        // Hedef Pozisyon: Z mutlaka 0 olmalı çünkü Shader düzleşme merkezini Player'ın Z'sinden alıyor
        float targetX = (targetLane - 1) * laneDistance;
        Vector3 targetPos = new Vector3(targetX, 0.5f, 0f); 

        // Pozisyonu ve Rotasyonu güncelle
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.identity; // Rotasyonu daima düz tut
    }
}