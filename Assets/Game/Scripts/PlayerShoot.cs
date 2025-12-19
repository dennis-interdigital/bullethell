using UnityEngine;
namespace bullethell
{
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

        public bool isInit = false;

        public void Init(StageManager inStageManager)
        {
            stageManager = inStageManager;

            ResetFireCooldown();

            isInit = true;
        }

        public void DoUpdate(float dt)
        {
            if (isInit)
            {
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
        }

        void ResetFireCooldown()
        {
            tRate = 1f / fireRate;
        }
    }
}
