using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Status Karakter")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected int maxHealth = 3;
    protected int currentHealth;

    [Header("Audio")]
    public AudioClip hitSound;

    protected Rigidbody2D rb;
    protected Animator anim;
    protected SpriteRenderer sr;
    protected bool isFacingRight = true; // Tambahan: Biar tau arah hadap

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        UpdateHealthUI(); // Update UI saat mulai
    }

    // Fungsi abstrak yang harus diisi oleh Player/Musuh nanti

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(gameObject.name + " Kena Damage. Sisa: " + currentHealth);

        UpdateHealthUI(); // Update UI Hati

        // 2. Mainkan Suara Sakit (Sesuai siapa yang kena pukul)
        if (hitSound != null)
        {
            // Kita pakai fungsi PlayVoice yang tadi kita buat (Jalur Mulut)
            // Jadi tidak akan menabrak suara pedang
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayVoice(hitSound);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        SoundManager.instance.PlayHurt();
    }

    // --- FUNGSI BARU: MENAMBAH DARAH ---
    public virtual void Heal(int amount)
    {
        // Hanya nambah darah kalau belum penuh
        if (currentHealth < maxHealth)
        {
            currentHealth += amount;

            // Jangan sampai lebih dari max
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            Debug.Log(gameObject.name + " Sembuh! Sisa: " + currentHealth);
            UpdateHealthUI(); // Update UI Hati
        }
    }

    // Fungsi Pembantu buat Update UI (Biar gak nulis ulang)
    protected void UpdateHealthUI()
    {
        if (gameObject.CompareTag("Player") && HealthUIManager.instance != null)
        {
            HealthUIManager.instance.UpdateHati(currentHealth);
        }
    }

    protected abstract void Die();

    // --- PERBAIKAN FUNGSI FLIP ---
    public void Flip(float _velocity)
    {
        // Kalau gerak ke kanan TAPI muka ke kiri -> Balik Kanan
        if (_velocity > 0.1f && !isFacingRight)
        {
            TurnAround();
        }
        // Kalau gerak ke kiri TAPI muka ke kanan -> Balik Kiri
        else if (_velocity < -0.1f && isFacingRight)
        {
            TurnAround();
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void TurnAround()
    {
        isFacingRight = !isFacingRight;
        
        // Simpan ukuran asli, cuma ubah X nya jadi negatif/positif
        Vector3 newScale = transform.localScale;
        newScale.x *= -1; 
        transform.localScale = newScale;
    }
}