using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Wajib ada untuk pindah scene

public class MenuManager : MonoBehaviour
{

    [Header("UI References")]
    public GameObject panelGelap;
    public GameObject CreditPanel;

    // Fungsi untuk pindah ke Scene "ChooseLevel"
    public void GoToChooseLevel()
    {
        SceneManager.LoadScene("ChooseLevel"); // Pastikan nama scene sama persis!
    }

    // Fungsi untuk masuk ke Gameplay (Level 1)
    public void PlayLevel1()
    {
        SceneManager.LoadScene("MainGame"); // Pastikan nama scene sama persis!
    }

    public void PlayLevel2()
    {
        SceneManager.LoadScene("Level2"); // Pastikan nama scene sama persis!
    }

    // Fungsi untuk kembali ke Main Menu (misal dari Choose Level)
    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCredit()
    {
        if (CreditPanel != null)
        {
            CreditPanel.SetActive(true);

            // PENTING: Pause game supaya karakter "diem aja"
            Time.timeScale = 0f;

            // Opsional: Matikan panel gelap background jika mau
            if (panelGelap != null) panelGelap.SetActive(true);
        }
    }

    public void CloseInfo()
    {
        if (CreditPanel != null)
        {
            CreditPanel.SetActive(false);

            // RESUME: Game jalan lagi
            Time.timeScale = 1f;

            if (panelGelap != null) panelGelap.SetActive(false);
        }
    }

    // Fungsi Keluar Game
    public void QuitGame()
    {
        Debug.Log("Keluar dari Game...");

        // Logika agar tombol Exit berfungsi saat di-Play di Unity Editor
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
