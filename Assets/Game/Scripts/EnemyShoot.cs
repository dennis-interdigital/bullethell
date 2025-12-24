using UnityEngine;
namespace BulletHell
{
    public class EnemyShoot : MonoBehaviour
    {
        public enum BulletPattern
        {
            Downward,
            Aimed,
            Ring,
            Spiral,
            Fan
        }

        [Header("Refs")]
        [SerializeField] EnemyBulletPool bulletPool;
        [SerializeField] Transform bulletSpawn;
        [SerializeField] Transform playerTarget;

        [Header("Pattern")]
        public BulletPattern pattern = BulletPattern.Ring;

        [Header("Firing")]
        public float fireRate = 1.5f;
        public float bulletSpeed = 6f;

        [Header("Ring / Fan")]
        public int bulletCount = 12;
        public float fanAngle = 60f;

        [Header("Spiral")]
        public float spiralRotateSpeed = 90f;

        float nextFireTime;
        float spiralAngle;

        bool isInit = false;

        public void Init(StageManager stageManager)
        {
            bulletPool = stageManager.enemyBulletPool;
            isInit = true;
        }

        void Update()
        {
            if (isInit)
            {
                if (!bulletPool || !bulletSpawn) return;

                if (Time.time >= nextFireTime)
                {
                    Fire();
                    nextFireTime = Time.time + 1f / Mathf.Max(0.01f, fireRate);
                }
            }
        }

        void Fire()
        {
            switch (pattern)
            {
                case BulletPattern.Downward:
                    ShootSingle(Vector3.down);
                    break;

                case BulletPattern.Aimed:
                    ShootSingle(GetAimedDirection());
                    break;

                case BulletPattern.Ring:
                    ShootRing();
                    break;

                case BulletPattern.Spiral:
                    ShootSpiral();
                    break;

                case BulletPattern.Fan:
                    ShootFan();
                    break;
            }
        }

        // ─────────────────────────────
        // PATTERNS
        // ─────────────────────────────

        void ShootSingle(Vector3 dir)
        {
            var b = bulletPool.GetEnemyBullet();
            b.transform.position = bulletSpawn.position;
            b.Init(dir.normalized, bulletSpeed, EnemyBulletScript.BulletOwner.Enemy);
            b.gameObject.SetActive(true);
        }

        void ShootRing()
        {
            float step = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = step * i;
                Vector3 dir = DirFromAngle(angle);
                ShootSingle(dir);
            }
        }

        void ShootSpiral()
        {
            float step = 360f / bulletCount;

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = spiralAngle + step * i;
                Vector3 dir = DirFromAngle(angle);
                ShootSingle(dir);
            }

            spiralAngle += spiralRotateSpeed * Time.deltaTime;
        }

        void ShootFan()
        {
            Vector3 baseDir = playerTarget ? GetAimedDirection() : Vector3.down;

            float startAngle = -fanAngle * 0.5f;
            float step = fanAngle / Mathf.Max(1, bulletCount - 1);

            for (int i = 0; i < bulletCount; i++)
            {
                float angle = startAngle + step * i;
                Vector3 dir = Quaternion.Euler(0, 0, angle) * baseDir;
                ShootSingle(dir);
            }
        }

        // ─────────────────────────────
        // HELPERS
        // ─────────────────────────────

        Vector3 GetAimedDirection()
        {
            if (!playerTarget)
                return Vector3.down;

            return (playerTarget.position - bulletSpawn.position).normalized;
        }

        Vector3 DirFromAngle(float angle)
        {
            float rad = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
        }
    }
}
