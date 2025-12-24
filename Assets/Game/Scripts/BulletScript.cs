using UnityEngine;

namespace BulletHell
{
    public class BulletScript : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private bool alignRotationToDirection = true;

        [Header("Type")]
        [SerializeField] private bool isProjectile = true;

        [Header("Laser Damage")]
        [SerializeField] private float laserDamageInterval = 0.2f;

        [Header("Damage")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float flatBonus = 0f;
        [SerializeField] private float damageMultiplier = 1f;

        private BulletPool _pool;
        private Vector3 _direction = Vector3.forward;

        // Projectile only
        private float _deathTime;

        // Laser only
        private float nextLaserDamageTime;

        public void SetPool(BulletPool pool) => _pool = pool;

        // ─────────────────────────────
        public float GetDamage()
        {
            return Mathf.Max(0f, (baseDamage + flatBonus) * damageMultiplier);
        }

        // ─────────────────────────────
        // INIT
        // ─────────────────────────────

        public void Init(Vector3 direction, float? customSpeed = null)
        {
            _direction = direction.sqrMagnitude > 0.0001f
                ? direction.normalized
                : transform.forward;

            if (customSpeed.HasValue)
                bulletSpeed = customSpeed.Value;

            if (isProjectile && alignRotationToDirection)
                transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);

            if (isProjectile)
                _deathTime = Time.time + lifetime;

            nextLaserDamageTime = 0f;
        }

        private void OnEnable()
        {
            if (isProjectile)
                _deathTime = Time.time + lifetime;

            nextLaserDamageTime = 0f;
        }

        // ─────────────────────────────
        private void Update()
        {
            if (isProjectile)
            {
                transform.position += _direction * bulletSpeed * Time.deltaTime;

                if (Time.time >= _deathTime)
                    _pool.Return(this);
            }
        }

        // ─────────────────────────────
        private void OnTriggerEnter(Collider other)
        {
            // Projectile = instant hit
            if (!isProjectile) return;

            if (other.CompareTag("Enemy"))
            {
                DamageEnemy(other);
                SpawnHitVFX(transform.position);
                _pool.Return(this);
            }
            else if (other.CompareTag("Boss"))
            {
                DamageBoss(other);
                SpawnHitVFX(transform.position);
                _pool.Return(this);
            }
        }

        // ─────────────────────────────
        private void OnTriggerStay(Collider other)
        {
            // Laser = damage over time
            if (isProjectile) return;
            if (Time.time < nextLaserDamageTime) return;

            if (other.CompareTag("Enemy"))
            {
                DamageEnemy(other);
                SpawnHitVFX(other.transform.position);
                nextLaserDamageTime = Time.time + laserDamageInterval;
            }
            else if (other.CompareTag("Boss"))
            {
                DamageBoss(other);
                SpawnHitVFX(other.transform.position);
                nextLaserDamageTime = Time.time + laserDamageInterval;
            }
        }

        // ─────────────────────────────
        void DamageEnemy(Collider other)
        {
            var enemy = other.GetComponentInParent<EnemyStatus>();
            if (enemy != null)
                enemy.TakeDamage(GetDamage());
        }

        void DamageBoss(Collider other)
        {
            var boss = other.GetComponentInParent<BossStatus>();
            if (boss != null)
                boss.TakeDamage(GetDamage());
        }

        void SpawnHitVFX(Vector3 pos)
        {
            if (VFXPool.Instance != null)
                VFXPool.Instance.Spawn("bulletHit", pos);
        }
    }
}
