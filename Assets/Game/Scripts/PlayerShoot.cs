using UnityEngine;

namespace bullethell
{
    public class PlayerShoot : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private BulletPool bulletPool;
        [SerializeField] private Transform bulletSpawnTransform;
        [SerializeField] private ParticleSystem shootFX;

        [Header("Firing")]
        [SerializeField] private float fireRate = 8f; // bullets per second
        [SerializeField] private float bulletSpeed = 12f;

        private StageManager stageManager;
        private float tRate;
        private bool isInit;
        private bool shootingEnabled = true; // 🔑 NEW

        // ─────────────────────────────
        public void Init(StageManager inStageManager)
        {
            stageManager = inStageManager;
            ResetFireCooldown();
            isInit = true;
        }

        // ─────────────────────────────
        public void DoUpdate(float dt)
        {
            if (!isInit || !shootingEnabled)
                return;

            if (Input.GetMouseButton(0))
            {
                if (tRate <= 0f)
                {
                    Fire();
                    ResetFireCooldown();
                }
                else
                {
                    tRate -= dt;
                }
            }
            else
            {
                // Optional: reset cooldown when button released
                tRate = Mathf.Min(tRate, 1f / fireRate);
            }
        }

        // ─────────────────────────────
        void Fire()
        {
            BulletScript bullet = bulletPool.Get();
            bullet.transform.SetPositionAndRotation(
                bulletSpawnTransform.position,
                bulletSpawnTransform.rotation
            );

            Vector2 dir = bulletSpawnTransform.right;
            bullet.Init(dir * bulletSpeed);
            bullet.gameObject.SetActive(true);

            if (shootFX)
                shootFX.Play();
        }

        void ResetFireCooldown()
        {
            tRate = 1f / Mathf.Max(0.01f, fireRate);
        }

        public void StopShooting()
        {
            shootingEnabled = false;
            tRate = 0f;

            if (shootFX && shootFX.isPlaying)
                shootFX.Stop();
        }

        public void StartShooting()
        {
            shootingEnabled = true;
            ResetFireCooldown();
        }

        public bool IsShootingEnabled()
        {
            return shootingEnabled;
        }
    }
}
