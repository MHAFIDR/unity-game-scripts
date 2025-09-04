using UnityEngine;
using System.Collections; // Wajib ada untuk menggunakan Coroutine

public class BombController : MonoBehaviour
{
    public float detonationDelay = 3f; // Waktu tunda dalam detik
    public float explosionRadius = 5f;
    public GameObject explosionEffect;
    public AudioClip explosionSound;
    private bool isDetonated = false;

    // Fungsi ini tetap dipanggil oleh PlayerInteraction saat menekan 'E'
    public void Detonate()
    {
        if (isDetonated) return;
        isDetonated = true;

        // Nonaktifkan collider agar tidak terdeteksi lagi
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Mulai hitung mundur untuk meledak
        StartCoroutine(ExplodeCoroutine());
    }

    // Coroutine untuk menangani ledakan setelah jeda
    private IEnumerator ExplodeCoroutine()
    {
        // Tunggu sesuai waktu tunda
        yield return new WaitForSeconds(detonationDelay);

        // --- Logika ledakan dimulai di sini ---
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in colliders)
        {
            DestructibleGate gate = nearbyObject.GetComponent<DestructibleGate>();
            if (gate != null)
            {
                gate.Shatter();
            }
        }

        // Hancurkan objek bom setelah semua logika ledakan selesai
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}