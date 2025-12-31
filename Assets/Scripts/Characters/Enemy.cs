using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character 
{
    [Header("Enemy AI")]
    public float detectionRange = 6f; 
    public float stopDistance = 1.5f; 
    public int damageToPlayer = 1;

    [Header("Enemy Sounds")]   // Masukkan suara monster kesakitan disini
    public AudioClip enemyAttackSound;

    [Header("Combat")]
    public float attackCooldown = 2f; 
    private float lastAttackTime;     
    
    private Transform playerTarget;   

    protected override void Start()
    {
        base.Start(); 
        
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p) playerTarget = p.transform;
    }

    void Update()
    {
        if (playerTarget == null || currentHealth <= 0) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distToPlayer <= stopDistance)
        {
            AttackBehavior();
        }
        else if (distToPlayer <= detectionRange)
        {
            ChaseBehavior();
        }
        else
        {
            IdleBehavior();
        }
    }

    void IdleBehavior()
    {
        rb.velocity = Vector2.zero;
        anim.SetFloat("Speed", 0); 
    }

    void ChaseBehavior()
    {
        Vector2 direction = (playerTarget.position - transform.position).normalized;
        
        // Gerak
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        
        // --- REVERSE FLIP ---
        ReverseFlip(direction.x);
    }

    void AttackBehavior()
    {
        rb.velocity = Vector2.zero;
        anim.SetFloat("Speed", 0);

        // Cek Cooldown Serangan
        if (Time.time > lastAttackTime + attackCooldown)
        {
            anim.SetTrigger("Attack");

            // --- PINDAHKAN KODINGAN SUARA KE SINI ---
            // Supaya dia hanya bunyi SEKALI saat animasi attack dimulai
            if (enemyAttackSound != null)
            {
                SoundManager.instance.PlayWeapon(enemyAttackSound);
            }
            // -----------------------------------------

            lastAttackTime = Time.time;

            if (playerTarget.GetComponent<Character>() != null)
            {
                playerTarget.GetComponent<Character>().TakeDamage(damageToPlayer);
            }
        }


        // --- REVERSE FLIP SAAT NYERANG ---
        float directionToPlayer = playerTarget.position.x - transform.position.x;
        ReverseFlip(directionToPlayer);
    }

    // FUNGSI KHUSUS UNTUK SPRITE HADAP KIRI
    void ReverseFlip(float directionX)
    {
        // Ambil skala saat ini (biar gak ngereset ukuran yang sudah diatur di Inspector)
        Vector3 currentScale = transform.localScale;

        // Kalau target ada di KANAN (positif) -> Balik jadi -ScaleX
        if (directionX > 0.1f)
        {
            // Pastikan X negatif (hadap kanan untuk sprite kidal)
            currentScale.x = -Mathf.Abs(currentScale.x);
            transform.localScale = currentScale;
        }
        // Kalau target ada di KIRI (negatif) -> Balik jadi +ScaleX
        else if (directionX < -0.1f)
        {
            // Pastikan X positif (hadap kiri/normal)
            currentScale.x = Mathf.Abs(currentScale.x);
            transform.localScale = currentScale;
        }
    }

    protected override void Die()
    {
        Debug.Log("Musuh Mati!");
        anim.SetTrigger("Death");

        // 1. STOP GERAKAN (Rem Mendadak)
        rb.velocity = Vector2.zero;

        // 2. MATIKAN GRAVITASI (PENTING!)
        // Ubah jadi Kinematic supaya dia melayang diam di posisi terakhir
        rb.isKinematic = true;

        // 3. MATIKAN COLLIDER
        // Supaya Player bisa jalan melewati mayat musuh (gak kesandung)
        GetComponent<Collider2D>().enabled = false;

        // 4. MATIKAN OTAK MUSUH
        this.enabled = false;

        // 5. HAPUS OBJEK (Setelah 2 detik)
        Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}