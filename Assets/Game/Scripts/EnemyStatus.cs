using UnityEngine;
using UnityEngine.Events;

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

    public EnemyStats Stats => stats;
    public float MaxHealth => (stats ? stats.maxHealth : 1f) * Mathf.Max(0.1f, healthMultiplier);
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    public void Init(Canvas canvas)
    {
        worldCanvas = canvas;
        if (healthUIPrefab && worldCanvas)
        {
            healthUI = Instantiate(healthUIPrefab, worldCanvas.transform);
            healthUI.target = healthUITargetTransform;
        }

        ResetHealth();
    }
    //private void Awake()
    //{
    //    // Ensure world-space canvas exists
    //    if (!worldCanvas)
    //    {
    //        var existing = FindObjectOfType<Canvas>();
    //        if (existing && existing.renderMode == RenderMode.WorldSpace)
    //            worldCanvas = existing;
    //        else
    //        {
    //            GameObject canvasGO = new GameObject("WorldCanvas", typeof(Canvas));
    //            worldCanvas = canvasGO.GetComponent<Canvas>();
    //            worldCanvas.renderMode = RenderMode.WorldSpace;
    //            worldCanvas.worldCamera = Camera.main;
    //            worldCanvas.transform.localScale = Vector3.one;
    //        }
    //    }

    //    // Instantiate health bar
        

    //}

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
}
