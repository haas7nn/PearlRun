using UnityEngine;

public class EnemyProjectile : EnemyBase
{
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f;

    public AudioSource loopAudioSource;
    public AudioSource shootAudioSource;
    public AudioClip shootSound;

    private Transform player;
    private float timer;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;

        if (loopAudioSource != null && !loopAudioSource.isPlaying)
        {
            loopAudioSource.Play();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            Shoot();
            timer = 0f;
        }
    }

    void Shoot()
    {
        if (player == null || projectilePrefab == null || firePoint == null) return;

        if (loopAudioSource != null)
        {
            loopAudioSource.Pause();
        }

        if (shootAudioSource != null && shootSound != null)
        {
            shootAudioSource.PlayOneShot(shootSound);
        }

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        Vector2 direction = player.position - firePoint.position;

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetDirection(direction);
        }

        Invoke(nameof(ResumeLoopSound), 0.5f);
    }

    void ResumeLoopSound()
    {
        if (loopAudioSource != null)
        {
            loopAudioSource.UnPause();
        }
    }
}