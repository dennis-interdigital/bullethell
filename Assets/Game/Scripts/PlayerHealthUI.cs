using DG.Tweening;
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
    private Vector3 punchScale = new Vector3(1f, 1f, 1f);
    private Tween hitTween;

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


    public void SetHealth(float normalizedHealth)
    {
        targetFill = Mathf.Clamp01(normalizedHealth);

        // Kill previous animation
        hitTween?.Kill();

        hitTween = DOTween.Sequence()
            .Append(
                transform.DOPunchScale(
                    punchScale,   // small punch
                    0.15f,
                    10,
                    0.9f
                )
            )
            .Append(
                transform.DOShakePosition(
                    0.2f,                // shake duration
                    new Vector3(3f, 3f, 3f),
                    10,
                    10f,
                    false,
                    true                  // local shake
                )
            );
    }
}
