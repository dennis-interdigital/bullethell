using UnityEngine;
using DG.Tweening;

namespace bullethell
{
    public class GameController : MonoBehaviour
    {
        [Header("Config")]
        public Transform m_SpeedParticle;
        public float bossTriggerValue = 150f;
        public float currBossTriggerValue = 0f;

        [Header("Game Speed")]
        public float m_GameSpeed = 1f;              // current speed (applied)
        public float defaultGameSpeed = 1f;         // normal speed
        public float gameSpeedLerpSpeed = 4f;       // smoothing strength

        private float targetGameSpeed;

        private StageManager stageManager;
        private UIManager uiManager;

        [Header("Visual")]
        public Transform scoreTargetTransform;

        // ─────────────────────────────
        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            uiManager = stageManager.uiManager;

            currBossTriggerValue = 0f;
            targetGameSpeed = defaultGameSpeed;
            m_GameSpeed = defaultGameSpeed;
        }

        void Update()
        {
            if (!Mathf.Approximately(m_GameSpeed, targetGameSpeed))
            {
                m_GameSpeed = Mathf.Lerp(
                    m_GameSpeed,
                    targetGameSpeed, 3f
                );

            }
        }


        /// <summary>
        /// Smoothly lerp game speed to target
        /// </summary>
        public void SetGameSpeed(float targetSpeed)
        {
            targetGameSpeed = Mathf.Max(0f, targetSpeed);
        }

        /// <summary>
        /// Instantly set game speed (no lerp)
        /// </summary>
        public void SetGameSpeedImmediate(float speed)
        {
            targetGameSpeed = speed;
            m_GameSpeed = speed;
        }

        /// <summary>
        /// Reset to default speed smoothly
        /// </summary>
        public void ResetGameSpeed()
        {
            SetGameSpeed(defaultGameSpeed);
        }

        public void HandleGameOver()
        {
            SetGameSpeed(0f);
        }

        public void HandleWin()
        {
            SetGameSpeed(5f); 
        }

        public void AddBossTrigger(float value)
        {
            currBossTriggerValue += value;

            var gameUI = uiManager.currentActiveUI as GameMenuUI;
            gameUI.UpdateBossTriggerUI(currBossTriggerValue, bossTriggerValue);

            if (currBossTriggerValue >= bossTriggerValue)
            {
                ResetBossTrigger();

                // Dramatic slow-mo
                SetGameSpeed(10f);

                stageManager.enemySpawner.SpawnBoss();
            }
        }

        public void ResetBossTrigger()
        {
            currBossTriggerValue = 0f;

            var gameUI = uiManager.currentActiveUI as GameMenuUI;
            gameUI.UpdateBossTriggerUI(currBossTriggerValue, bossTriggerValue);
        }
    }
}
