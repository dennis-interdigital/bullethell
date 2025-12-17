using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("References")]
    public Transform target;                         // Enemy transform to follow
    public Vector3 offset = new Vector3(0f, 2f, 0f); // Offset above enemy
    public float followSmooth = 5f;                  // Follow smoothing

    [Header("Bar")]
    public Image healthFill;                         // Image with 'Filled' type
    public float fillSmooth = 10f;                   // Fill smoothing

    private Vector3 _targetPos;
    private float _targetFill = 1f;

    private void OnEnable()
    {
        this.gameObject.transform.position = _targetPos;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Smoothly follow the enemy
        _targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * followSmooth);

        // Smoothly update fill
        if (healthFill)
            healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, _targetFill, Time.deltaTime * fillSmooth);
    }

    /// <summary>Set health [0..1]</summary>
    public void SetHealth(float normalized)
    {
        _targetFill = Mathf.Clamp01(normalized);
    }
}
