using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game State")]
    public bool isGameActive = true;
    public int score = 0;
    public int targetScoreToWin = 3;

    [Header("UI Info / Tutorial")]
    public GameObject infoPanel;

    [Header("UI References")]
    public GameObject panelGelap;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public GameObject pauseMenuPanel;

    [Header("UI Notifications")]
    public GameObject notifPlusNyawa;
    public GameObject notifMinNyawa;
    public GameObject notifTaskIncomplete;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            // DontDestroyOnLoad(gameObject); // Biarkan mati jika kamu drag UI manual per scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ResetGameUI();
    }

    private void ResetGameUI()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (panelGelap != null) panelGelap.SetActive(false);

        Time.timeScale = 1f;
        isGameActive = true;

        // 2. LOGIKA BARU: Munculkan Info Panel saat Start
        if (infoPanel != null)
        {
            OpenInfo(); // Panggil fungsi buka info
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // --- FUNGSI UTAMA ---

    public void AddScore(int amount)
    {
        score += amount;
    }

    // Parameter delay (default 0 = langsung mati)
    public void GameOver(float delay = 0)
    {
        if (!isGameActive) return;

        if (delay > 0)
        {
            StartCoroutine(GameOverSequence(delay));
        }
        else
        {
            ExecuteGameOver();
        }
    }

    IEnumerator GameOverSequence(float delay)
    {
        // Tunggu sekian detik sebelum menampilkan Game Over
        yield return new WaitForSeconds(delay);
        ExecuteGameOver();
    }

    private void ExecuteGameOver()
    {
        isGameActive = false;
        Debug.Log("GAME OVER");

        // SAYA UNCOMMENT INI AGAR SUARA KELUAR
        if (SoundManager.instance != null)
        {
            // Pastikan kamu punya variabel 'hurtSound' di SoundManager
            // Atau ganti dengan 'wrongSound' jika lebih cocok
            SoundManager.instance.PlaySFX(SoundManager.instance.hurtSound);
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (panelGelap != null) panelGelap.SetActive(true);

        Time.timeScale = 0f;
    }

    public void LevelComplete()
    {
        if (!isGameActive) return;

        isGameActive = false;
        Debug.Log("LEVEL COMPLETED!");

        // SAYA UNCOMMENT INI AGAR SUARA KELUAR
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlaySFX(SoundManager.instance.levelCompleteSound);
        }

        if (victoryPanel != null) victoryPanel.SetActive(true);
        if (panelGelap != null) panelGelap.SetActive(true);

        Time.timeScale = 0f;

        SoundManager.instance.PlayWin();
    }

    public void OpenInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);

            // PENTING: Pause game supaya karakter "diem aja"
            Time.timeScale = 0f;

            // Opsional: Matikan panel gelap background jika mau
            if (panelGelap != null) panelGelap.SetActive(true);
        }
    }

    public void TogglePause()
    {
        if (pauseMenuPanel != null && isGameActive)
        {
            bool isPaused = pauseMenuPanel.activeSelf;
            pauseMenuPanel.SetActive(!isPaused);

            if (panelGelap != null)
                panelGelap.SetActive(!isPaused);

            Time.timeScale = !isPaused ? 0f : 1f;
        }
    }

    // --- FUNGSI TOMBOL UI ---

    public void PlayAgain()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToChooseLevel()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("ChooseLevel");
    }

    public void BackToMenu()
    {
        PlayButtonSound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ResumeGame()
    {
        PlayButtonSound();
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (panelGelap != null) panelGelap.SetActive(false);

        Time.timeScale = 1f;
    }

    public void PlayButtonSound()
    {
        SoundManager.instance.PlayButtonClick();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public void CloseInfo()
    {
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);

            // RESUME: Game jalan lagi
            Time.timeScale = 1f;

            if (panelGelap != null) panelGelap.SetActive(false);
        }
    }

    public void ShowPlusNyawa()
    {
        StartCoroutine(ShowNotificationRoutine(notifPlusNyawa));
    }

    public void ShowMinNyawa()
    {
        StartCoroutine(ShowNotificationRoutine(notifMinNyawa));
    }

    public void ShowTaskIncomplete()
    {
        StartCoroutine(ShowNotificationRoutine(notifTaskIncomplete));
    }

    // Coroutine: Munculkan UI, tunggu 1.5 detik, lalu matikan
    IEnumerator ShowNotificationRoutine(GameObject notifObj)
    {
        if (notifObj != null)
        {
            notifObj.SetActive(true);

            // Tunggu 1.5 detik (sesuaikan durasi sesuai selera)
            yield return new WaitForSeconds(1.5f);

            notifObj.SetActive(false);
        }
    }
}