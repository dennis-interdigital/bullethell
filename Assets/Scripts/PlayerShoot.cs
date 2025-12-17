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

    StageManager stageManager;

    float tRate;

    public void Init(StageManager inStageManager)
    {
        stageManager = inStageManager;

        ResetFireCooldown();
    }

    public void DoUpdate(float dt)
    {
        //// Hold left mouse to keep firing
        //if (Input.GetMouseButton(0) && Time.time >= _nextFireTime)
        //{
        //    Fire();
        //    _nextFireTime = Time.time + (1f / Mathf.Max(0.0001f, fireRate));
        //}

        if (Input.GetMouseButton(0))
        {
            if (tRate <= 0f)
            {
                BulletScript bullet = bulletPool.Get();
                bullet.transform.SetPositionAndRotation(bulletSpawnTransform.position, bulletSpawnTransform.rotation);

                Vector2 dir = bulletSpawnTransform.right;
                bullet.Init(dir * bulletSpeed);
                bullet.gameObject.SetActive(true);

                ResetFireCooldown();
            }
            else
            {
                tRate -= dt;
            }
        }
    }

    void ResetFireCooldown()
    {
        tRate = 1f / fireRate;
    }
}
