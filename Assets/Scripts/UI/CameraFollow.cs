using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    [Header("Camera Settings")]
    public float smoothSpeed = 5f;
    public float xOffset = 2f;
    public float yOffset = 1f;

    [Header("Dead Zone")]
    public float deadZoneX = 2f;

    [Header("Batasan (Optional)")]
    public bool useBottomLimit = true;
    public float minY = 0f;

    // --- VARIABEL BARU UNTUK SHAKE ---
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f; // Kekuatan getaran

    void LateUpdate()
    {
        if (target == null) return;

        // 1. LOGIKA FOLLOW (Sama seperti sebelumnya)
        float targetX = transform.position.x;
        float playerX = target.position.x + xOffset;
        float deltaX = playerX - transform.position.x;

        if (Mathf.Abs(deltaX) > deadZoneX)
        {
            if (deltaX > 0) targetX = playerX - deadZoneX;
            else targetX = playerX + deadZoneX;
        }

        float targetY = target.position.y + yOffset;
        if (useBottomLimit)
        {
            targetY = Mathf.Clamp(targetY, minY, Mathf.Infinity);
        }

        Vector3 finalPos = new Vector3(targetX, targetY, -10);

        // 2. LOGIKA SHAKE (INI YANG BARU)
        if (shakeDuration > 0)
        {
            // Buat posisi acak kecil di sekitar kamera
            Vector3 shakeOffset = Random.insideUnitCircle * shakeMagnitude;

            // Tambahkan ke posisi akhir
            finalPos += shakeOffset;

            // Kurangi waktu durasi getar
            shakeDuration -= Time.deltaTime;
        }

        // 3. TERAPKAN POSISI
        transform.position = Vector3.Lerp(transform.position, finalPos, smoothSpeed * Time.deltaTime);
    }

    // --- FUNGSI YANG DIPANGGIL DARI LUAR ---
    public void TriggerShake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeMagnitude = magnitude;
    }

    // Debug Visual
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(deadZoneX * 2, 10, 0));
    }
}