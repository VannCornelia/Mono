using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishLineTrigger : MonoBehaviour
{
    [Header("Status Pintu")]
    public bool isLocked = true; // Dikendalikan oleh QuizManager

    [Header("Visual")]
    public GameObject visualTerkunci; // Masukkan gambar gembok/gelap
    public GameObject visualTerbuka;  // Masukkan gambar pintu terang

    // [Header("UI Peringatan")] 
    // Variable warningTextUI KITA HAPUS karena sudah dipindah ke GameManager

    private void Start()
    {
        UpdateVisuals();
    }

    // Fungsi ini dipanggil QuizManager saat semua soal terjawab
    public void UnlockPortal()
    {
        isLocked = false;
        UpdateVisuals();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (isLocked)
            {
                // --- PERUBAHAN DISINI ---
                // Panggil notifikasi "Task Not Complete" dari GameManager
                if (GameManager.instance != null)
                {
                    GameManager.instance.ShowTaskIncomplete();
                }
            }
            else
            {
                // Panggil fungsi LevelComplete() milik GameManager
                if (GameManager.instance != null)
                {
                    GameManager.instance.LevelComplete();
                }
            }
        }
    }

    void UpdateVisuals()
    {
        if (visualTerkunci != null) visualTerkunci.SetActive(isLocked);
        if (visualTerbuka != null) visualTerbuka.SetActive(!isLocked);
    }
}