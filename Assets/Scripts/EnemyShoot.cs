using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EnemyBulletPool bulletPool;
    [SerializeField] private Transform bulletSpawn;
    [SerializeField] private Transform playerTarget; // Assign player transform here

    [Header("Firing")]
    [SerializeField] private float fireRate = 1.5f;
    [SerializeField] private float bulletSpeed = 10f;

    private float _nextFire;

    private void Update()
    {
        if (!bulletPool || !bulletSpawn) return;
        if (Time.time >= _nextFire)
        {
            ShootDownward();
            _nextFire = Time.time + 1f / Mathf.Max(0.001f, fireRate);
        }
    }

    private void ShootDownward()
    {
        var b = bulletPool.Get();
        b.transform.position = bulletSpawn.position;

        // aim straight down if no player assigned
        Vector3 dir = Vector3.down;

        // if player exists, aim directly toward them
        if (playerTarget)
        {
            dir = (playerTarget.position - bulletSpawn.position).normalized;
        }

        b.Init(dir, bulletSpeed);
        b.gameObject.SetActive(true);
    }
}
