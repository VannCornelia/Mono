using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Untuk Musik Latar (Looping)
    public AudioSource sfxSource;   // Untuk Efek Suara (Sekali main)  
    public AudioSource bgmSource;
    public AudioSource playerVoiceSource; // KHUSUS: Hurt, Jump, Die (Suara Mulut)
    public AudioSource weaponSource;

    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public AudioClip windSound;

    [Header("Player SFX")]
    public AudioClip jumpSound;
    public AudioClip hurtSound;
    public AudioClip walkSound; // (Opsional)
    public AudioClip punchSound;
    public AudioClip knifeSound;

    [Header("Quiz SFX")]
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip popupOpenSound; // Suara kertas terbuka

    [Header("Game SFX")]
    public AudioClip levelCompleteSound;
    public AudioClip buttonClickSound;
    public AudioClip doorOpenSound;

    void Awake()
    {
        // Cek apakah instance sudah ada
        if (instance == null)
        {
            instance = this;
            // KUNCI: Agar object tidak hancur saat load scene baru
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Jika instance sudah ada (dari scene sebelumnya),
            // hancurkan duplikat yang baru ini agar tidak bentrok.
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Putar musik background otomatis saat game mulai
        PlayBacksound(windSound);
    }
    public void PlayWalkStep()
    {
        // 1. Ubah Pitch (Nada) sedikit secara acak
        // Angka 0.8f (berat/rendah) sampai 1.2f (cempreng/tinggi)
        sfxSource.pitch = Random.Range(0.85f, 1.15f);

        // 2. Mainkan suaranya
        if (walkSound != null)
        {
            sfxSource.PlayOneShot(walkSound);
        }
    }

    // --- FUNGSI UNTUK MEMUTAR SUARA ---
    public void PlayBacksound(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true; // PENTING: Supaya angin tidak berhenti
        bgmSource.Play();
    }

    // 1. Putar Musik (Looping)
    public void PlayMusic(AudioClip clip)
    {
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = true; // Musik berulang terus
            musicSource.Play();
        }
    }

    public void PlayVoice(AudioClip clip)
    {
        if (playerVoiceSource != null && clip != null)
        {
            // Opsional: Matikan suara lama biar suara baru jelas
            // playerVoiceSource.Stop(); 

            playerVoiceSource.pitch = Random.Range(0.9f, 1.1f); // Variasi sedikit
            playerVoiceSource.PlayOneShot(clip);
        }
    }

    // 2. FUNGSI UNTUK SENJATA (Attack)
    public void PlayWeapon(AudioClip clip)
    {
        if (weaponSource != null && clip != null)
        {
            // Opsional: Random Pitch biar sabetan pedang terasa beda-beda
            weaponSource.pitch = Random.Range(0.85f, 1.15f);
            weaponSource.PlayOneShot(clip);
        }
    }

    // 2. Putar SFX (Sekali bunyi, bisa ditumpuk)
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    // 3. Fungsi Spesifik (Biar gampang dipanggil script lain)
    // 1. Player Sounds
    public void PlayJump() => PlaySFX(jumpSound);
    public void PlayHurt()
    {
        // FITUR TAMBAHAN: Kalau kena damage, hentikan suara senjata!
        // Supaya terdengar seolah serangan kita "batal" karena kesakitan
        if (weaponSource != null) weaponSource.Stop();

        PlayVoice(hurtSound);
    }
    public void PlayWalk() => PlaySFX(walkSound);
    public void PlayPunch() => PlaySFX(punchSound);
    public void PlayKnife() => PlaySFX(knifeSound);

    // 2. Quiz Sounds
    public void PlayCorrect() => PlaySFX(correctSound);
    public void PlayWrong() => PlaySFX(wrongSound);
    public void PlayPopup() => PlaySFX(popupOpenSound);

    // 3. Game Sounds
    public void PlayButtonClick() => PlaySFX(buttonClickSound);
    public void PlayDoorOpen() => PlaySFX(doorOpenSound);
    public void PlayWin()
    {
        musicSource.Stop(); // Matikan musik seram saat menang
        PlaySFX(levelCompleteSound);
    }
}
