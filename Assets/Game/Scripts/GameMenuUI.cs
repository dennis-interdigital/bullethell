using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace bullethell
{
    public class GameMenuUI : BaseUI
    {
        [Header("Visual")]
        public Image bossTriggerfill;
        public TextMeshProUGUI bossTriggerValueText;

        [Header("Smoothing")]
        public float fillLerpSpeed = 8f;

        private float targetFill = 0f;

        public override void Init(StageManager stageManager)
        {
            base.Init(stageManager);

            if (bossTriggerfill)
                bossTriggerfill.fillAmount = 0f;
            UpdateBossTriggerUI(0, stageManager.gameController.bossTriggerValue);
        }

        public override void DoUpdate(float dt)
        {
            // Smooth fill update
            if (bossTriggerfill)
            {
                bossTriggerfill.fillAmount = Mathf.Lerp(
                    bossTriggerfill.fillAmount,
                    targetFill,
                    dt * fillLerpSpeed
                );
            }
        }

        public void UpdateBossTriggerUI(float currVal, float bossTriggerVal)
        {
            // Clamp & protect divide-by-zero
            targetFill = bossTriggerVal <= 0f
                ? 0f
                : Mathf.Clamp01(currVal / bossTriggerVal);

            bossTriggerValueText.text =
                $"<color=white>{currVal}</color> / <color=red>{bossTriggerVal}</color>";

            bossTriggerValueText.transform.DOPunchScale(new Vector3(.5f,.5f,.5f), .5f, 5, .5f);
        }
    }
}
