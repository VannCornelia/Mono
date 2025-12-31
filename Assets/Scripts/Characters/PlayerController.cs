using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INHERITANCE: PlayerController mewarisi sifat Character
public class PlayerController : Character
{
    [Header("Player Specifics")]
    public float jumpForce = 10f;

    [Header("Audio Settings")]
    public float stepRate = 0.5f; // Jarak waktu antar langkah (makin kecil makin ngebut)
    private float nextStepTime = 0f;

    [Header("Fall Settings")]
    public float fallThreshold = -5f;

    [Header("Combat Settings")]
    public Transform attackPoint;   // Titik pusat serangan (di depan mulut/tangan)
    public float attackRange = 0.5f;// Seberapa jauh jangkauan pukulannya
    public LayerMask enemyLayers;   // Agar kita cuma bisa mukul musuh (bukan tembok/teman)
    public int attackDamage = 1;

    [Header("Ground Detection")]
    public Transform groundCheck;   // Buat object kosong di kaki player
    public float groundCheckRadius = 0.2f;
    public LayerMask Ground;   // Set layer lantai jadi "Ground"

    private bool isGrounded;
    private float horizontalInput;
    public float interactRange = 1f;
    public LayerMask interactLayer;

    // Override Start dari induk
    protected override void Start()
    {
        base.Start(); // Jalankan setup bawaan (Get Rigidbody & Animator)
    }

    void Update()
    {
        // 1. INPUT
        horizontalInput = Input.GetAxisRaw("Horizontal"); // A/D atau Panah Kiri/Kanan

        // Cek Lompat (Spasi)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            SoundManager.instance.PlayJump(); // <--- Panggil Disini
        }

        // Cek Serang (Klik Kanan Mouse)
        if (Input.GetMouseButtonDown(1))
        {
            Attack();
        }

        HandleWalkSound();

        // 2. UPDATE ANIMATION PARAMETERS (Sesuai Screenshot Animator kamu)
        UpdateAnimations();

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckInteraction();
        }

        if (transform.position.y < fallThreshold)
        {
            Die();
        }

        UpdateAnimations();
    }

    void HandleWalkSound()
    {
        // Syarat Bunyi: 
        // 1. Ada input gerakan (horizontalInput tidak 0)
        // 2. Karakter sedang menapak tanah (isGrounded == true)
        if (horizontalInput != 0 && isGrounded)
        {
            // Cek Timer (Supaya suara ada jedanya, tidak seperti senapan mesin)
            if (Time.time >= nextStepTime)
            {
                // Pastikan SoundManager ada sebelum memanggilnya
                if (SoundManager.instance != null)
                {
                    SoundManager.instance.PlaySFX(SoundManager.instance.walkSound);
                }

                // Atur waktu untuk langkah berikutnya
                nextStepTime = Time.time + stepRate;
            }
        }
    }

    void FixedUpdate()
    {
        // 3. PHYSICS MOVEMENT
        // Cek apakah kaki nempel tanah
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, Ground);

        // Gerakkan karakter
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);

        // Balik badan
        Flip(horizontalInput);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void Attack()
    {
        // 1. Mainkan Animasi Serang
        anim.SetTrigger("Melee");

        // 2. Deteksi Musuh di dalam lingkaran serangan
        // Fungsi ini membuat lingkaran imajiner dan mengumpulkan semua benda yang ada di dalamnya
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 3. Berikan Damage ke setiap musuh yang kena
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Memukul: " + enemy.name);

            // Ambil script Character dari musuh dan kurangi darahnya
            Character enemyScript = enemy.GetComponent<Character>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(attackDamage);
            }
        }
        SoundManager.instance.PlayPunch();
    }

    void UpdateAnimations()
    {
        // Parameter: Move (Float)
        // Kita pakai Mathf.Abs biar nilainya selalu positif (0 sampai 1) saat jalan kiri/kanan
        anim.SetFloat("Move", Mathf.Abs(horizontalInput));

        // Parameter: IsJumping (Bool)
        // Kalau TIDAK di tanah = Jumping/Falling
        anim.SetBool("IsJumping", !isGrounded);

        // Parameter: JumpState (Float/Int) - Opsional kalau dipake di BlendTree
        // > 0.1 naik, < -0.1 turun
        anim.SetFloat("JumpState", rb.velocity.y);
    }

    // Wajib ada karena mewarisi Abstract Class Character
    // PlayerController.cs
    // Override fungsi Die dari induk Character
    protected override void Die()
    {
        Debug.Log("Player Mati (Darah Habis / Jatuh)!");

        // 1. Matikan kontrol & fisik
        this.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero; // Stop gerak

        // 2. Mainkan animasi mati (jika ada)
        // anim.SetTrigger("Death"); 

        // 3. Lapor ke GameManager
        if (GameManager.instance != null)
        {
            GameManager.instance.GameOver();
        }
    }

    // Visualisasi Ground Check di Scene Editor biar gampang debug
    void OnDrawGizmosSelected()
    {
        // Gambar lingkaran Ground Check
        if (groundCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Gambar lingkaran Attack Point (Warna Merah)
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }

    void CheckInteraction()
    {
        // Buat lingkaran deteksi
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange, interactLayer);

        foreach (Collider2D hit in hits)
        {
            // Cek apakah benda itu punya script Interactable?
            Interactable interactable = hit.GetComponent<Interactable>();
            if (interactable != null)
            {
                interactable.BaseInteract(); // Panggil fungsi interaksi
                return; // Cukup interaksi sama 1 benda aja
            }
        }
    }

}
