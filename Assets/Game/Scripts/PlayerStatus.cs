using System.Collections;
using UnityEngine;
using UnityEngine.Events;
namespace bullethell
{
    public class PlayerStatus : MonoBehaviour
    {
        [Header("Health")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Visual")]
        [SerializeField] private PlayerHealthUI healthUI;   // assign your world-space health UI
        [SerializeField] private ParticleSystem destroyedVfx;
        [SerializeField] private GameObject playerDestroyedVisual;
        [SerializeField] private GameObject playerMainVisual;

        [Header("Events")]
        public UnityEvent onDeath;                          // optional: hook VFX/SFX/respawn

        public bool IsDead => currentHealth <= 0f;

        public StageManager stageManager;

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            healthUI.gameObject.SetActive(true);
            ResetHealth();
        }

        /// <summary>Sets health to max and refreshes UI.</summary>
        public void ResetHealth()
        {
            playerDestroyedVisual.SetActive(false);
            playerMainVisual.SetActive(true);
            healthUI.gameObject.SetActive(true);
            currentHealth = Mathf.Max(1f, maxHealth);
            UpdateUI();
        }

        /// <summary>Reduce health by amount (>=0). Triggers onDeath if reaches 0.</summary>
        public void TakeDamage(float amount)
        {
            if (amount <= 0f || IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
            UpdateUI();

            if (IsDead)
            {
                onDeath?.Invoke();
                OnDeath();
            }
        }

        /// <summary>Increase health by amount (>=0), clamped to max.</summary>
        public void Heal(float amount)
        {
            if (amount <= 0f || IsDead) return;

            currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);
            UpdateUI();
        }

        /// <summary>Set absolute health value (0..max).</summary>
        public void SetHealth(float newValue)
        {
            currentHealth = Mathf.Clamp(newValue, 0f, maxHealth);
            UpdateUI();

            if (IsDead)
            {
                onDeath?.Invoke();
            }
        }

        /// <summary>Optionally update max health at runtime.</summary>
        public void SetMaxHealth(float newMax, bool keepRatio = true)
        {
            newMax = Mathf.Max(1f, newMax);
            float ratio = Mathf.Clamp01(currentHealth / maxHealth);
            maxHealth = newMax;
            currentHealth = keepRatio ? ratio * maxHealth : Mathf.Min(currentHealth, maxHealth);
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (healthUI)
            {
                float normalized = maxHealth > 0f ? currentHealth / maxHealth : 0f;
                healthUI.SetHealth(normalized);
            }
        }

        void OnDeath()
        {
            playerDestroyedVisual.SetActive(true);
            playerMainVisual.SetActive(false);
            healthUI.gameObject.SetActive(false);
            StartCoroutine(onDeathCouroutine());
        }

        IEnumerator onDeathCouroutine()
        {
            yield return new WaitForSeconds(1f);
            destroyedVfx.Play();
            yield return new WaitForSeconds(1f);
            playerDestroyedVisual.SetActive(false);
        }
    }
}
