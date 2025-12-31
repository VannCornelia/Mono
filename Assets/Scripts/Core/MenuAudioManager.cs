using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuSimpleAudio : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource bgmSource; // Buat Lagu (Centang Loop di Inspector)
    public AudioSource sfxSource; // Buat SFX (Jangan Centang Loop)

    [Header("Clips")]
    public AudioClip backgroundMusic;
    public AudioClip startButtonSound;

    private void Start()
    {
        // Langsung mainkan musik saat menu dibuka
        if (bgmSource != null && backgroundMusic != null)
        {
            bgmSource.clip = backgroundMusic;
            bgmSource.Play();
        }
    }

    // Pasang ini di Tombol "Play" / "Start"
    public void OnStartGameClicked()
    {
        // Bunyi SFX
        if (sfxSource != null) sfxSource.PlayOneShot(startButtonSound);

        // Langsung pindah ke Choose Level
        // (Musik Main Menu akan otomatis mati karena scene-nya diganti)
        SceneManager.LoadScene("ChooseLevel"); // Pastikan nama scene sesuai
    }

    public void ClickSound()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(startButtonSound);
    }
}