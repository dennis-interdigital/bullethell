using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
namespace bullethell
{
    public class EnemyStatus : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField] private EnemyStats stats; // ScriptableObject defining enemy data
        [Min(0.1f)] public float healthMultiplier = 1f;

        [Header("Runtime")]
        [SerializeField] private float currentHealth;

        [Header("UI Settings")]
        [SerializeField] private EnemyHealthUI healthUIPrefab;  // Health bar prefab
        [SerializeField] private Transform healthUITargetTransform;
        [SerializeField] private Canvas worldCanvas;            // World-space canvas to attach to
        private EnemyHealthUI healthUI;                         // Runtime instance

        [Header("VFX Settings")]
        [SerializeField] private string deathVFXKey = "explosion"; // VFX key in VFXPool

        [Header("Events")]
        public UnityEvent onDeath;

        [Header("Hit Feedback")]
        [SerializeField] private Vector3 hitPunchScale = new Vector3(0.12f, 0.12f, 0f);
        [SerializeField] private float hitPunchDuration = 0.12f;
        [Header("Hit Rotation Feedback")]
        [SerializeField] private float hitRotateAngle = 8f; // degrees (small!)

        private Tween hitTween;
        private Vector3 originalScale;
        private Quaternion originalRotation;




        public EnemyStats Stats => stats;
        public float MaxHealth => (stats ? stats.maxHealth : 1f) * Mathf.Max(0.1f, healthMultiplier);
        public float CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0f;

        private StageManager stageManager;

        private void Awake()
        {
            originalScale = transform.localScale;
            originalRotation = transform.localRotation;
        }

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            worldCanvas = stageManager.worldCanvas;
            if (healthUIPrefab && worldCanvas)
            {
                healthUI = Instantiate(healthUIPrefab, worldCanvas.transform);
                healthUI.target = healthUITargetTransform;
            }

            ResetHealth();
        }

        private void OnDestroy()
        {
            if (healthUI)
                Destroy(healthUI.gameObject);
        }

        public void ResetHealth()
        {
            currentHealth = MaxHealth;
            UpdateUI();
        }

        public void TakeDamage(float amount)
        {
            if (amount <= 0f || IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth - amount, 0f, MaxHealth);
            UpdateUI();

            PlayHitFeedback();

            if (IsDead)
                HandleDeath();
        }


        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, MaxHealth);
            UpdateUI();
        }

        public void SetHealth(float newValue)
        {
            currentHealth = Mathf.Clamp(newValue, 0f, MaxHealth);
            UpdateUI();

            if (IsDead)
                HandleDeath();
        }

        public void SetStats(EnemyStats newStats, bool resetHealth = true)
        {
            stats = newStats;
            if (resetHealth) ResetHealth(); else UpdateUI();
        }

        private void UpdateUI()
        {
            if (!healthUI) return;
            float normalized = MaxHealth > 0f ? currentHealth / MaxHealth : 0f;
            healthUI.SetHealth(normalized);
        }

        private void HandleDeath()
        {
            // Trigger UnityEvent (if assigned)
            onDeath?.Invoke();

            stageManager.gameController.AddBossTrigger(stats.scoreReward);

            // Spawn explosion VFX via pool
            if (VFXPool.Instance != null && !string.IsNullOrEmpty(deathVFXKey))
            {
                VFXPool.Instance.Spawn(deathVFXKey, transform.position);
            }

            // Destroy health bar
            if (healthUI)
                Destroy(healthUI.gameObject);

            // Destroy enemy itself after short delay
            Destroy(gameObject);
        }

        private void PlayHitFeedback()
        {
            hitTween?.Kill();

            // Reset to original transform state
            transform.localScale = originalScale;
            transform.localRotation = originalRotation;

            // Random small rotation direction
            Vector3 randomRot = GetRandomHitRotation();

            hitTween = DOTween.Sequence()
                .Append(
                    transform.DOPunchScale(
                        hitPunchScale,
                        hitPunchDuration,
                        8,
                        0.9f
                    )
                )
                .Join(
                    transform.DOPunchRotation(
                        randomRot,
                        hitPunchDuration,
                        8,
                        0.9f
                    )
                );
        }


        private Vector3 GetRandomHitRotation()
        {
            int dir = Random.Range(0, 4);

            switch (dir)
            {
                case 0: return new Vector3(0f, 0f, hitRotateAngle);  // right
                case 1: return new Vector3(0f, 0f, -hitRotateAngle);  // left
                case 2: return new Vector3(hitRotateAngle, 0f, 0f); // up
                default: return new Vector3(-hitRotateAngle, 0f, 0f); // down
            }
        }

    }
}
