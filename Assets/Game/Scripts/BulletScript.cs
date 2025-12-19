using UnityEngine;
namespace bullethell
{
    public class BulletScript : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private float bulletSpeed = 10f;        // default speed (can be overridden)
        [SerializeField] private float lifetime = 2f;            // seconds till despawn
        [SerializeField] private bool alignRotationToDirection = true;

        [Header("Damage")]
        [SerializeField] private float baseDamage = 10f;         // base damage of this bullet
        [SerializeField] private float flatBonus = 0f;           // flat add/subtract (debuff can be negative)
        [SerializeField] private float damageMultiplier = 1f;    // buff/debuff multiplier (e.g., 1.2f for +20%)

        private BulletPool _pool;
        private Vector3 _direction = Vector3.forward;
        private float _deathTime;

        public void SetPool(BulletPool pool) => _pool = pool;

        /// <summary>Current damage after modifiers.</summary>
        public float GetDamage()
        {
            return Mathf.Max(0f, (baseDamage + flatBonus) * damageMultiplier);
        }

        /// ===== Buff/Debuff helpers =====

        /// <summary>Overwrite the base damage (e.g., from weapon level).</summary>
        public void SetBaseDamage(float value) => baseDamage = Mathf.Max(0f, value);

        /// <summary>Add a flat bonus (negative for debuff). Example: +5 or -3.</summary>
        public void AddFlatBonus(float amount) => flatBonus += amount;

        /// <summary>Set absolute multiplier. Example: 1.5 = +50%, 0.8 = -20%.</summary>
        public void SetDamageMultiplier(float multiplier) => damageMultiplier = Mathf.Max(0f, multiplier);

        /// <summary>Multiply current multiplier (stacking buffs/debuffs). Example: *= 1.2f.</summary>
        public void MultiplyDamage(float factor) => damageMultiplier = Mathf.Max(0f, damageMultiplier * factor);

        /// <summary>Clear all modifiers back to defaults (keeps current baseDamage).</summary>
        public void ResetDamageModifiers()
        {
            flatBonus = 0f;
            damageMultiplier = 1f;
        }

        /// ===== Movement / Lifetime =====

        /// <summary>Initializes the bullet with a direction and optional speed override.</summary>
        public void Init(Vector3 direction, float? customSpeed = null)
        {
            _direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : transform.forward;
            if (customSpeed.HasValue) bulletSpeed = customSpeed.Value;

            if (alignRotationToDirection && _direction.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);

            _deathTime = Time.time + lifetime;
        }

        private void OnEnable()
        {
            // Safety: in case the bullet was re-enabled without Init being called yet.
            _deathTime = Time.time + lifetime;
        }

        private void Update()
        {
            // Move in world space
            transform.position += _direction * bulletSpeed * Time.deltaTime;

            // Lifetime check
            if (Time.time >= _deathTime)
            {
                _pool.Return(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Enemy"))
            {
                // Apply damage to enemy if present
                var enemy = other.GetComponentInParent<EnemyStatus>();
                if (enemy != null)
                {
                    enemy.TakeDamage(GetDamage());
                }

                // VFX + return to pool
                if (VFXPool.Instance != null)
                    VFXPool.Instance.Spawn("bulletHit", transform.position);

                _pool.Return(this);
            }
        }
    }
}
