using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace BulletHell
{
    public class PlayerHealthUI : MonoBehaviour
    {
        [Header("References")]
        public Transform mainObject;
        public Vector3 offset = new Vector3(0, 2f, 0);
        public float followSmooth = 5f;

        [Header("Health Bar")]
        public Image healthFill;

        [Header("Hit Animation")]
        public Vector3 punchScale = new Vector3(0.12f, 0.12f, 0f);
        public float punchDuration = 0.15f;

        public float shakeDuration = 0.15f;
        public Vector3 shakeStrength = new Vector3(0.08f, 0.08f, 0f);
        public int shakeVibrato = 10;

        private Vector3 targetPos;
        private float targetFill = 1f;
        private Tween hitTween;

        private void OnEnable()
        {
            if (mainObject)
            {
                targetPos = mainObject.position + offset;
                transform.position = targetPos;
            }
        }

        void LateUpdate()
        {
            if (!mainObject) return;

            // Follow player
            targetPos = mainObject.position + offset;
            transform.position = Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * followSmooth
            );

            // Smooth health fill
            if (healthFill)
            {
                healthFill.fillAmount = Mathf.Lerp(
                    healthFill.fillAmount,
                    targetFill,
                    Time.deltaTime * 10f
                );
            }
        }

        public void SetHealth(float normalizedHealth)
        {
            targetFill = Mathf.Clamp01(normalizedHealth);

            hitTween?.Kill();

            transform.localScale = Vector3.one;

            hitTween = DOTween.Sequence()
                .Append(
                    transform.DOPunchScale(
                        new Vector3(0.12f, 0.12f, 0f),
                        0.15f,
                        8,
                        0.9f
                    )
                )
                .Append(
                    transform.DOShakePosition(
                        0.15f,
                        new Vector3(0.08f, 0.08f, 0f),
                        10,
                        0.9f,
                        false,
                        true
                    )
                );
        }

    }
}
