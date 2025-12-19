using UnityEngine;
namespace bullethell
{
    public class EnemyBulletScript : MonoBehaviour
    {
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

        private EnemyBulletPool _pool;
        private Vector3 _direction = Vector3.down; // Default downward along Y axis
        private float _deathTime;

        public void SetPool(EnemyBulletPool pool) => _pool = pool;

        public float GetDamage() => Mathf.Max(0f, (baseDamage + flatBonus) * damageMultiplier);

        public void SetBaseDamage(float value) => baseDamage = Mathf.Max(0f, value);
        public void AddFlatBonus(float amount) => flatBonus += amount;
        public void SetDamageMultiplier(float multiplier) => damageMultiplier = Mathf.Max(0f, multiplier);
        public void MultiplyDamage(float factor) => damageMultiplier = Mathf.Max(0f, damageMultiplier * factor);
        public void ResetDamageModifiers() { flatBonus = 0f; damageMultiplier = 1f; }

        public void Init(Vector3 direction, float? customSpeed = null)
        {
            // ensure it goes downward along Y axis
            _direction = direction.sqrMagnitude > 0.0001f ? direction.normalized : Vector3.down;
            if (customSpeed.HasValue) bulletSpeed = customSpeed.Value;

            if (alignRotationToDirection)
                transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);

            _deathTime = Time.time + lifetime;
        }

        private void OnEnable()
        {
            _deathTime = Time.time + lifetime;
        }

        private void Update()
        {
            // Move downward (Y axis)
            transform.position += _direction * bulletSpeed * Time.deltaTime;

            if (Time.time >= _deathTime)
                _pool.Return(this);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag.Equals("Player"))
            {
                var player = other.GetComponentInParent<PlayerStatus>();
                if (player != null)
                {
                    player.TakeDamage(GetDamage());
                }

                if (VFXPool.Instance != null && !string.IsNullOrEmpty(hitVFXKey))
                    VFXPool.Instance.Spawn(hitVFXKey, transform.position);

                _pool.Return(this);
            }

        }
    }
}
