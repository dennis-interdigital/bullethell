using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BulletPool bulletPool;
    [SerializeField] private Transform bulletSpawnTransform; // attach a child transform here
    [SerializeField] private ParticleSystem shootFX;

    [Header("Firing")]
    [SerializeField] private float fireRate = 8f; // bullets per second
    [SerializeField] private float bulletSpeed = 12f;

    private float _nextFireTime = 0f;

    public void DoUpdate()
    {
        // Hold left mouse to keep firing
        if (Input.GetMouseButton(0) && Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + (1f / Mathf.Max(0.0001f, fireRate));
        }
    }

    private void Fire()
    {
        //shootFX.Play();
        BulletScript bullet = bulletPool.Get();
        bullet.transform.SetPositionAndRotation(bulletSpawnTransform.position, bulletSpawnTransform.rotation);

        // Move along the spawn's "right" (good default for 2D sprites facing right)
        Vector2 dir = bulletSpawnTransform.right;
        bullet.Init(dir * bulletSpeed);
        bullet.gameObject.SetActive(true);
    }
}
