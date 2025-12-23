using System.Collections;
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
            canvasGroup.alpha = 0f;
            if (target)
            {
                targetPos = target.position + offset;
                transform.position = targetPos;
            }

            StartCoroutine(ShowUI());
        }

        IEnumerator ShowUI()
        {
            yield return new WaitForSeconds(1f);
            canvasGroup.alpha = 1f;
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
