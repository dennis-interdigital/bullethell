using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    public Transform mainObject;                     // Player target
    public Vector3 offset = new Vector3(0, 2f, 0); // Offset above player
    public float followSmooth = 5f;              // Smooth follow speed

    [Header("Health Bar")]
    public Image healthFill;                     // Image with fillAmount
    private Vector3 targetPos;
    private float targetFill = 1f;

    private void OnEnable()
    {
        this.gameObject.transform.position = targetPos;
    }

    void LateUpdate()
    {
        if (!mainObject) return;

        // Smoothly follow player position
        targetPos = mainObject.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSmooth);

        // Smoothly update fill
        if (healthFill)
            healthFill.fillAmount = Mathf.Lerp(healthFill.fillAmount, targetFill, Time.deltaTime * 10f);
    }

    /// <summary>
    /// Sets the health bar fill (value between 0 and 1)
    /// </summary>
    public void SetHealth(float normalizedHealth)
    {
        targetFill = Mathf.Clamp01(normalizedHealth);
    }
}
