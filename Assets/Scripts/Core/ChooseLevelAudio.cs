using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseLevelSimpleAudio : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource bgmSource; // Centang Loop
    public AudioSource sfxSource; // Jangan Loop

    [Header("Clips")]
    public AudioClip levelSelectMusic;
    public AudioClip clickSound;
    public AudioClip lockedSound;
    public AudioClip backSound;

    private void Start()
    {
        // Mainkan musik khusus Choose Level
        if (bgmSource != null && levelSelectMusic != null)
        {
            bgmSource.clip = levelSelectMusic;
            bgmSource.Play();
        }
    }

    // 1. Fungsi untuk tombol Level yang terbuka (Level 1, Level 2, dst)
    // Masukkan nama scene game (misal "Level1") di kolom inspector tombol
    public void OnLevelSelected(string sceneName)
    {
        if (sfxSource != null) sfxSource.PlayOneShot(clickSound);

        // Pindah ke Game
        SceneManager.LoadScene(sceneName);
    }

    // 2. Fungsi untuk tombol Level yang terkunci
    public void OnLockedLevelClicked()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(lockedSound);
    }

    // 3. Fungsi untuk tombol Back kembali ke Main Menu
    public void OnBackClicked()
    {
        if (sfxSource != null) sfxSource.PlayOneShot(backSound);

        // Balik ke menu awal
        // (Nanti MainMenuSimpleAudio akan nyala lagi dari awal)
        SceneManager.LoadScene("MainMenu");
    }
}
