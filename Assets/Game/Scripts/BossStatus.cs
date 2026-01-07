using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BulletHell
{
    public class BossStatus : MonoBehaviour
    {
        [Header("Definition")]
        [SerializeField] private EnemyStats stats;
        [Min(0.1f)] public float healthMultiplier = 1f;

        [Header("Runtime")]
        [SerializeField] private float currentHealth;
        public List<EnemyShoot> bossEnemyShootModules = new List<EnemyShoot>();
        public List<EnemyStatus> bossEnemyHealthModules = new List<EnemyStatus>();
        public EnemyMovement bossMovement;

        [Header("UI Settings")]
        [SerializeField] private EnemyHealthUI healthUIPrefab;   // 🔑 ADDED
        [SerializeField] private Transform healthUITargetTransform;
        [SerializeField] private Canvas worldCanvas;
        private EnemyHealthUI healthUI;

        [Header("VFX Settings")]
        [SerializeField] private string deathVFXKey = "bossExplosion";

        [Header("Events")]
        public UnityEvent onDeath;

        [Header("Hit Feedback")]
        [SerializeField] private Vector3 hitPunchScale = new Vector3(0.12f, 0.12f, 0f);
        [SerializeField] private float hitPunchDuration = 0.12f;

        [Header("Hit Rotation Feedback")]
        [SerializeField] private float hitRotateAngle = 8f;

        private Tween hitTween;
        private Vector3 originalScale;
        private Quaternion originalRotation;

        private StageManager stageManager;

        // ─────────────────────────────
        public EnemyStats Stats => stats;
        public float MaxHealth => (stats ? stats.maxHealth : 1f) * Mathf.Max(0.1f, healthMultiplier);
        public float CurrentHealth => currentHealth;
        public bool IsDead => currentHealth <= 0f;

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
            healthMultiplier = currWave/2 * 1.1f;
            Debug.Log($"BOSS - Curr Wave:{currWave}, Multiplier:{healthMultiplier}, Max Health:{MaxHealth}");
        }

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            bossMovement =  this.GetComponent<EnemyMovement>();
            worldCanvas = stageManager.worldCanvas;

            SetBossHealthMultiplier();
            if (healthUIPrefab && worldCanvas)
            {
                healthUI = Instantiate(healthUIPrefab, worldCanvas.transform);
                healthUI.target = healthUITargetTransform;
            }
            InitBossEnemyModules();
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
            Debug.Log($"Boss Health: {MaxHealth}");
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

        private void HandleDeath()
        {
            onDeath?.Invoke();

            stageManager.gameController.AddBossTrigger(stats.scoreReward);

            if (VFXPool.Instance != null && !string.IsNullOrEmpty(deathVFXKey))
            {
                VFXPool.Instance.Spawn(deathVFXKey, transform.position);
            }

            if (healthUI)
                Destroy(healthUI.gameObject);

            StartCoroutine(DeathCouroutine());
        }

        IEnumerator DeathCouroutine()
        {
            bossMovement.StopMovement();
            if (VFXPool.Instance != null && !string.IsNullOrEmpty(deathVFXKey))
            {
                VFXPool.Instance.Spawn(deathVFXKey, transform.position);
            }
            yield return new WaitForSeconds(1.5f);
            Destroy(gameObject);
        }

        // ─────────────────────────────
        private void PlayHitFeedback()
        {
            hitTween?.Kill();

            // Reset baseline (important for non-1 scale prefabs)
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
                0 => new Vector3(0f, 0f, hitRotateAngle),   // right
                1 => new Vector3(0f, 0f, -hitRotateAngle),  // left
                2 => new Vector3(hitRotateAngle, 0f, 0f),   // up
                _ => new Vector3(-hitRotateAngle, 0f, 0f),  // down
            };
        }

        public void InitBossEnemyModules()
        {
            foreach (var enemy in bossEnemyShootModules)
            {
                enemy.Init(stageManager);
            }

            foreach (var enemy in bossEnemyHealthModules)
            {
                enemy.Init(stageManager);
            }
        }
    }
}
