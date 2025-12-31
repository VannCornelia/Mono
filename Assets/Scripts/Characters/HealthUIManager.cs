using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIManager : MonoBehaviour
{
    public static HealthUIManager instance;

    [Header("Daftar Gambar Hati")]
    public GameObject[] hatiObjects; // Masukkan Heart1, Heart2, Heart3 ke sini

    void Awake()
    {
        instance = this;
    }

    public void UpdateHati(int sisaNyawa)
    {
        // Loop semua hati
        for (int i = 0; i < hatiObjects.Length; i++)
        {
            if (i < sisaNyawa)
            {
                // Kalau index lebih kecil dari sisa nyawa, NYALAKAN
                hatiObjects[i].SetActive(true);
            }
            else
            {
                // Kalau sudah hilang nyawanya, MATIKAN
                hatiObjects[i].SetActive(false);
            }
        }
    }
}