using UnityEngine;
using System.Collections.Generic;

public class TrackSpawner : MonoBehaviour
{
    public GameObject trackPrefab; 
    public int initialTrackCount = 5; 
    public float trackLength = 70f; // JSON'daki son Z değerinle aynı olmalı
    public float moveSpeed = 5f; 

    private List<GameObject> activeTracks = new List<GameObject>();

    void Start()
    {
        // Önce listeyi temizleyelim
        activeTracks.Clear();

        // Başlangıçta yolları oluştur
        for (int i = 0; i < initialTrackCount; i++)
        {
            SpawnTrack(i * trackLength);
        }
    }

    void Update()
    {
        // Hata koruması: Liste boşsa işlem yapma
        if (activeTracks.Count == 0) return;

        // Tüm yolları hareket ettir
        for (int i = 0; i < activeTracks.Count; i++)
        {
            if (activeTracks[i] != null)
                activeTracks[i].transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }

        // En öndeki yol (index 0) oyuncunun arkasına geçti mi?
        // Z pozisyonu -trackLength'ten küçükse sil ve yeni ekle
        if (activeTracks[0].transform.position.z < -trackLength)
        {
            DeleteOldAndSpawnNew();
        }
    }

    void SpawnTrack(float zPos)
    {
        GameObject go = Instantiate(trackPrefab, new Vector3(0, 0, zPos), Quaternion.identity);
        go.transform.parent = this.transform; // Düzenli durması için manager'ın altına koy
        activeTracks.Add(go);
    }

    void DeleteOldAndSpawnNew()
    {
        if (activeTracks.Count == 0) return;

        GameObject oldTrack = activeTracks[0];
        activeTracks.RemoveAt(0);
        Destroy(oldTrack);

        // Yeni yolu, listenin sonundaki elemanın Z pozisyonuna göre hesapla
        float lastZ = activeTracks[activeTracks.Count - 1].transform.position.z;
        SpawnTrack(lastZ + trackLength);
    }
}