using UnityEngine;
using UnityEngine.UI;

namespace bullethell
{
    [RequireComponent(typeof(CanvasGroup))]
    public class EnemyHealthUI : MonoBehaviour
    {
        [Header("References")]
        public Transform target;
        public Vector3 offset = new Vector3(0f, 2f, 0f);
        public float followSmooth = 5f;

        [Header("Bar")]
        public Image healthFill;
        public float fillSmooth = 10f;

        [Header("Appear Settings")]
        public float appearDistance = 0.2f; // how close before showing

        private Vector3 targetPos;
        private float targetFill = 1f;

        private CanvasGroup canvasGroup;
        private bool isVisible;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            isVisible = false;
            canvasGroup.alpha = 0f;

            if (target)
            {
                // SNAP to target immediately (no lerp yet)
                targetPos = target.position + offset;
                transform.position = targetPos;
            }
        }

        void LateUpdate()
        {
            if (!target) return;

            targetPos = target.position + offset;

            // Follow
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSmooth
            );

            // Appear only when close enough (prevents screen jump)
            if (!isVisible)
            {
                float dist = Vector3.Distance(transform.position, targetPos);
                if (dist <= appearDistance)
                {
                    isVisible = true;
                    canvasGroup.alpha = 1f;
                }
            }

            // Smooth fill
            if (healthFill)
            {
                healthFill.fillAmount = Mathf.Lerp(
                    healthFill.fillAmount,
                    targetFill,
                    Time.deltaTime * fillSmooth
                );
            }
        }

        /// <summary>Set health [0..1]</summary>
        public void SetHealth(float normalized)
        {
            targetFill = Mathf.Clamp01(normalized);
        }
    }
}
