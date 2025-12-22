using UnityEngine;

namespace bullethell
{
    public class EnemyBulletScript : MonoBehaviour
    {
        public enum BulletOwner
        {
            Enemy,
            Boss
        }

        [Header("Bullet Settings")]
        [SerializeField] private float bulletSpeed = 10f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private bool alignRotationToDirection = true;

        [Header("Damage")]
        [SerializeField] private float baseDamage = 10f;
        [SerializeField] private float flatBonus = 0f;
        [SerializeField] private float damageMultiplier = 1f;

        [Header("VFX")]
        [SerializeField] private string hitVFXKey = "enemyBulletHit";

        [Header("Owner")]
        [SerializeField] private BulletOwner owner = BulletOwner.Enemy;

        private EnemyBulletPool pool;
        private Vector3 direction = Vector3.down;
        private float deathTime;

        // ─────────────────────────────
        // POOL
        // ─────────────────────────────
        public void SetPool(EnemyBulletPool pool)
        {
            this.pool = pool;
        }

        // ─────────────────────────────
        // INIT
        // ─────────────────────────────
        public void Init(
            Vector3 dir,
            float? customSpeed,
            BulletOwner owner)
        {
            this.owner = owner;

            direction = dir.sqrMagnitude > 0.0001f
                ? dir.normalized
                : Vector3.down;

            if (customSpeed.HasValue)
                bulletSpeed = customSpeed.Value;

            if (alignRotationToDirection)
                transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

            deathTime = Time.time + lifetime;
        }

        void Update()
        {
            transform.position += direction * bulletSpeed * Time.deltaTime;

            if (Time.time >= deathTime)
            {
                Return();
            }
        }
        public float GetDamage()
        {
            return Mathf.Max(0f, (baseDamage + flatBonus) * damageMultiplier);
        }

        public void SetBaseDamage(float value)
        {
            baseDamage = Mathf.Max(0f, value);
        }

        public void AddFlatBonus(float amount)
        {
            flatBonus += amount;
        }

        public void SetDamageMultiplier(float multiplier)
        {
            damageMultiplier = Mathf.Max(0f, multiplier);
        }

        public void MultiplyDamage(float factor)
        {
            damageMultiplier = Mathf.Max(0f, damageMultiplier * factor);
        }

        public void ResetDamageModifiers()
        {
            flatBonus = 0f;
            damageMultiplier = 1f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            PlayerStatus player = other.GetComponentInParent<PlayerStatus>();
            if (player != null)
            {
                player.TakeDamage(GetDamage());
            }

            if (VFXPool.Instance != null && !string.IsNullOrEmpty(hitVFXKey))
            {
                VFXPool.Instance.Spawn(hitVFXKey, transform.position);
            }

            Return();
        }
        
        public void Return()
        {
            if (!pool) return;

            if (owner == BulletOwner.Boss)
                pool.ReturnBossBullet(this);
            else
                pool.ReturnEnemyBullet(this);
        }
    }
}
