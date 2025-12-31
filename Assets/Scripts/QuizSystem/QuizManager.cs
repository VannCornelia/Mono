using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public static QuizManager instance;
    private Character playerScript;

    [Header("UI Quiz (Input)")]
    public GameObject popupQuiz;
    public GameObject panelGelap;
    public Image imageWadahSoal;
    public TextMeshProUGUI textOpsiA;
    public TextMeshProUGUI textOpsiB;

    [Header("UI Feedback (Output)")]
    public GameObject panelBenar; // Masukkan Panel yang berisi Gambar Tabel Benar + Tombol Lanjut
    public Image imagePenjelasanBenar;
    public GameObject panelSalah; // Masukkan Gambar Banner "SALAH"

    [Header("UI HUD")]
    public TextMeshProUGUI textProgress;

    [Header("Game Logic")]
    public FinishLineTrigger finishPortal;
    public int totalKuis = 3;

    [Header("Referensi Lain")]
    public CameraFollow cameraScript;

    private int kuisTerselesaikan = 0;
    private QuestionStation altarAktif;
    private bool kunciJawabanIsA;
    private bool isAnswering = false; // Cegah spam tombol

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateHUD();

        // Pastikan semua UI popup mati di awal
        if (popupQuiz != null) popupQuiz.SetActive(false);
        if (panelGelap != null) panelGelap.SetActive(false);
        if (panelBenar != null) panelBenar.SetActive(false);
        if (panelSalah != null) panelSalah.SetActive(false);

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerScript = playerObj.GetComponent<Character>();

        if (cameraScript == null)
            cameraScript = Camera.main.GetComponent<CameraFollow>();
    }

    // --- LOGIKA UTAMA ---

    public void BukaKuis(Sprite gambarSoal, string txtA, string txtB, bool isA, QuestionStation station)
    {
        if (popupQuiz != null) popupQuiz.SetActive(true);
        if (panelGelap != null) panelGelap.SetActive(true);

        if (imageWadahSoal != null)
        {
            imageWadahSoal.sprite = gambarSoal;
            imageWadahSoal.preserveAspect = true;
        }

        if (textOpsiA != null) textOpsiA.text = txtA;
        if (textOpsiB != null) textOpsiB.text = txtB;

        kunciJawabanIsA = isA;
        altarAktif = station;
        isAnswering = false; // Reset status menjawab

        Time.timeScale = 0; // Pause game saat mikir

        SoundManager.instance.PlayPopup();
    }

    public void Jawab(bool pilihA)
    {
        if (isAnswering) return; // Cegah pemain menekan tombol 2x
        isAnswering = true;

        // Tutup soal dulu
        if (popupQuiz != null) popupQuiz.SetActive(false);

        // Panel gelap JANGAN dimatikan dulu biar transisi mulus

        if (pilihA == kunciJawabanIsA)
        {
            Debug.Log("JAWABAN BENAR!");
            HandleJawabanBenar();
        }
        else
        {
            Debug.Log("JAWABAN SALAH!");
            StartCoroutine(HandleJawabanSalahSequence());
        }
    }

    // --- HANDLING BENAR & SALAH ---

    void HandleJawabanBenar()
    {
        // 1. Mainkan Suara Benar
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.correctSound);
        }

        // 2. Ganti Gambar Penjelasan sesuai Altar yang sedang aktif
        if (altarAktif != null && imagePenjelasanBenar != null)
        {
            if (altarAktif.gambarPenjelasan != null)
            {
                imagePenjelasanBenar.sprite = altarAktif.gambarPenjelasan;
                imagePenjelasanBenar.preserveAspect = true;
            }
        }

        // 3. Munculkan Panel Tabel Benar
        if (panelBenar != null) panelBenar.SetActive(true);

        if (GameManager.instance != null)
        {
            GameManager.instance.ShowPlusNyawa();
        }

        // 4. Logika Gameplay
        if (altarAktif != null) altarAktif.MatikanAltar();
        TambahProgress();
        if (playerScript != null) playerScript.Heal(1);
    }

    // Fungsi ini dipasang di Tombol "Lanjut" pada Panel Benar
    public void TutupPanelBenar()
    {
        if (panelBenar != null) panelBenar.SetActive(false);
        if (panelGelap != null) panelGelap.SetActive(false);

        isAnswering = false;
        Time.timeScale = 1; // RESUME GAME
    }

    IEnumerator HandleJawabanSalahSequence()
    {
        // 1. Munculkan UI Salah
        if (panelSalah != null) panelSalah.SetActive(true);

        // TAMBAHKAN INI: Tampilkan -1 Nyawa
        if (GameManager.instance != null)
        {
            GameManager.instance.ShowMinNyawa();
        }

        // Sound & Shake...
        if (SoundManager.instance != null) SoundManager.instance.PlaySFX(SoundManager.instance.wrongSound);
        if (cameraScript != null) cameraScript.TriggerShake(0.5f, 0.3f);

        // 2. TUNGGU DULU (JANGAN kurangi nyawa dulu!)
        // Biarkan pemain menatap kesalahannya selama 2 detik
        yield return new WaitForSecondsRealtime(2f);

        // 3. TUTUP UI SALAH (Penting! Hilangkan ini sebelum Game Over muncul)
        if (panelSalah != null) panelSalah.SetActive(false);

        // 4. BARU SEKARANG BERI HUKUMAN
        if (playerScript != null)
        {
            // Panggil fungsi damage
            playerScript.TakeDamage(1);

            // Cek apakah pemain masih hidup setelah kena damage?
            if (playerScript.GetCurrentHealth() > 0)
            {
                // Jika MASIH HIDUP:
                // Tutup layar gelap dan lanjutkan game
                if (panelGelap != null) panelGelap.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                // Jika MATI:
                // Kita panggil Game Over disini untuk memastikan (jaga-jaga kalau script Character kamu lupa manggil)
                // Karena UI Salah sudah ditutup di poin no 3, tampilannya bakal bersih!
                if (GameManager.instance != null)
                {
                    GameManager.instance.GameOver();
                }
            }
        }
    }

    public void PlayButtonSound()
    {
        SoundManager.instance.PlayButtonClick();
    }

    // --- LOGIKA PROGRESS ---
    void TambahProgress()
    {
        kuisTerselesaikan++;
        UpdateHUD();
        if (kuisTerselesaikan >= totalKuis) BukaGerbang();
    }

    void UpdateHUD()
    {
        if (textProgress != null) textProgress.text = "Misi: " + kuisTerselesaikan + "/" + totalKuis;
    }

    void BukaGerbang()
    {
        if (finishPortal != null) finishPortal.UnlockPortal();
    }
}