using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace BulletHell
{
    public class EnemyStatus : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField] private EnemyStats stats;
        [Min(0.1f)] public float healthMultiplier = 1f;

        [Header("Runtime")]
        [SerializeField] private float currentHealth;

        [Header("UI Settings")]
        [SerializeField] private EnemyHealthUI healthUIPrefab;
        [SerializeField] private Transform healthUITargetTransform;
        [SerializeField] private Canvas worldCanvas;
        private EnemyHealthUI healthUI;

        [Header("Death VFX")]
        [SerializeField] private string deathVFXKey = "explosion";

        [Header("Score VFX")]
        [SerializeField] private string scoreVFXKey = "score";
        [SerializeField] private int scoreValue = 10;

        [Header("Events")]
        public UnityEvent onDeath;

        [Header("Hit Feedback")]
        [SerializeField] private Vector3 hitPunchScale = new Vector3(0.12f, 0.12f, 0f);
        [SerializeField] private float hitPunchDuration = 0.12f;
        [SerializeField] private float hitRotateAngle = 8f;

        private Tween hitTween;
        private Vector3 originalScale;
        private Quaternion originalRotation;

        public EnemyStats Stats => stats;
        public float MaxHealth => (stats ? stats.maxHealth : 1f) * Mathf.Max(0.1f, healthMultiplier);
        public float CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0f;

        private StageManager stageManager;

        // ─────────────────────────────
        private void Awake()
        {
            originalScale = transform.localScale;
            originalRotation = transform.localRotation;
        }
        public void SetBossHealthMultiplier()
        {
            var currWave = stageManager.gameController.currentWave;
            if (currWave <= 1)
            {
                healthMultiplier = 1;
                return;
            }
            healthMultiplier = currWave / 2 * 1.1f;
            Debug.Log($"Normal Enemy - Curr Wave:{currWave}, Multiplier:{healthMultiplier}, Max Health:{MaxHealth}");
        }

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            worldCanvas = stageManager.worldCanvas;

            SetBossHealthMultiplier();
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

        // ─────────────────────────────
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
            if (resetHealth) ResetHealth();
            else UpdateUI();
        }

        // ─────────────────────────────
        private void UpdateUI()
        {
            if (!healthUI) return;

            float normalized = MaxHealth > 0f
                ? currentHealth / MaxHealth
                : 0f;

            healthUI.SetHealth(normalized);
        }

        // ─────────────────────────────
        public void HandleDeath()
        {
            // Notify spawner / listeners
            onDeath?.Invoke();


            // Explosion VFX
            if (VFXPool.Instance != null && !string.IsNullOrEmpty(deathVFXKey))
            {
                VFXPool.Instance.Spawn(deathVFXKey, transform.position);
            }

            // Score fly VFX
            if (VFXPool.Instance != null &&
                !string.IsNullOrEmpty(scoreVFXKey) &&
                stageManager.gameController.scoreTargetTransform != null)
            {
                GameObject vfxObj = VFXPool.Instance.Spawn(scoreVFXKey, transform.position);
                var fly = vfxObj.GetComponent<ScoreVFX>();
                if (fly != null)
                {
                    fly.Init(
                        stageManager,
                        transform.position,
                        stageManager.gameController.scoreTargetTransform,
                        (int)stats.scoreReward
                    );
                }
            }

            // Cleanup UI
            if (healthUI)
                Destroy(healthUI.gameObject);

            Destroy(gameObject);
        }

        // ─────────────────────────────
        private void PlayHitFeedback()
        {
            hitTween?.Kill();

            transform.localScale = originalScale;
            transform.localRotation = originalRotation;

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

            return dir switch
            {
                0 => new Vector3(0f, 0f, hitRotateAngle),
                1 => new Vector3(0f, 0f, -hitRotateAngle),
                2 => new Vector3(hitRotateAngle, 0f, 0f),
                _ => new Vector3(-hitRotateAngle, 0f, 0f)
            };
        }
    }
}
