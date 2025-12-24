using UnityEngine;

namespace BulletHell
{
    public class PlayerShoot : MonoBehaviour
    {
        public enum ShootType
        {
            Normal,
            Triple,
            Laser
        }

        [Header("Refs")]
        [SerializeField] private BulletPool bulletPool;

        [Tooltip("Index 0 = center, 1 = left, 2 = right")]
        [SerializeField] private Transform[] bulletSpawnPoints;

        [SerializeField] private ParticleSystem shootFX;

        [Header("Laser")]
        [SerializeField] private GameObject laserObject;
        [SerializeField] private Transform laserSpawnPoint;

        [Header("Firing")]
        [SerializeField] private float fireRate = 8f;
        [SerializeField] private float bulletSpeed = 12f;

        [Header("Mode")]
        [SerializeField] private ShootType shootType = ShootType.Normal;

        private StageManager stageManager;
        private float tRate;
        private bool isInit;
        private bool shootingEnabled = true;

        // 🔑 LASER STATE
        private bool isLaserActive;

        // ─────────────────────────────
        public void Init(StageManager inStageManager)
        {
            stageManager = inStageManager;
            ResetFireCooldown();
            isInit = true;

            if (laserObject)
                laserObject.SetActive(false);
        }

        // ─────────────────────────────
        public void DoUpdate(float dt)
        {
            if (!isInit)
                return;

            // 🔑 ALWAYS HANDLE LASER INPUT FIRST
            if (shootType == ShootType.Laser)
            {
                HandleLaserInput();
            }

            if (!shootingEnabled)
                return;

            if (shootType == ShootType.Laser)
                return;

            // ─────────────────────────────
            // NORMAL / TRIPLE
            if (Input.GetMouseButton(0))
            {
                if (tRate <= 0f)
                {
                    FireBullets();
                    ResetFireCooldown();
                }
                else
                {
                    tRate -= dt;
                }
            }
        }

        // ─────────────────────────────
        void FireBullets()
        {
            switch (shootType)
            {
                case ShootType.Normal:
                    FireFromSpawn(0);
                    break;

                case ShootType.Triple:
                    FireFromSpawn(0);
                    FireFromSpawn(1);
                    FireFromSpawn(2);
                    break;
            }

            if (shootFX)
                shootFX.Play();
        }

        void FireFromSpawn(int index)
        {
            if (bulletSpawnPoints == null || index >= bulletSpawnPoints.Length)
                return;

            Transform spawn = bulletSpawnPoints[index];

            BulletScript bullet = bulletPool.Get();
            bullet.transform.SetPositionAndRotation(spawn.position, spawn.rotation);

            Vector2 dir = spawn.right;
            bullet.Init(dir * bulletSpeed);
            bullet.gameObject.SetActive(true);
        }

        // ─────────────────────────────
        // LASER (ROBUST)
        // ─────────────────────────────

        void HandleLaserInput()
        {
            bool holding = Input.GetMouseButton(0);

            if (holding && !isLaserActive)
            {
                StartLaser();
            }
            else if (!holding && isLaserActive)
            {
                StopLaser();
            }
        }

        void StartLaser()
        {
            isLaserActive = true;

            if (laserObject)
            {
                laserObject.transform.position = laserSpawnPoint.position;
                laserObject.SetActive(true);
            }
        }

        void StopLaser()
        {
            isLaserActive = false;

            if (laserObject)
                laserObject.SetActive(false);
        }

        // ─────────────────────────────
        void ResetFireCooldown()
        {
            tRate = 1f / Mathf.Max(0.01f, fireRate);
        }

        // ─────────────────────────────
        // PUBLIC API
        // ─────────────────────────────

        public void SetShootType(ShootType type)
        {
            if (shootType == ShootType.Laser)
                StopLaser();

            shootType = type;
            ResetFireCooldown();
        }

        public void StopShooting()
        {
            shootingEnabled = false;
            tRate = 0f;
            StopLaser();

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
