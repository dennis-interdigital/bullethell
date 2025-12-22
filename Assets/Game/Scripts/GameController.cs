using UnityEngine;
namespace bullethell
{
    public class GameController : MonoBehaviour
    {
        [Header("Config")]
        public Transform m_SpeedParticle;
        public float bossTriggerValue = 150f;
        public float currBossTriggerValue = 0;
        public float m_GameSpeed = 100;

        private StageManager stageManager;
        private UIManager uiManager;

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            uiManager = stageManager.uiManager;
            currBossTriggerValue = 0;
        }

        public void HandleGameOver()
        {
            //m_GameSpeed = 0;
            //m_SpeedParticle.gameObject.SetActive(false);
            currBossTriggerValue = 0;
        }

        public void HandleWin()
        {
            currBossTriggerValue = 0;
        }

        public void AddBossTrigger(float value)
        {
            currBossTriggerValue += value;
            var gameUI = uiManager.currentActiveUI as GameMenuUI;
            gameUI.UpdateBossTriggerUI(currBossTriggerValue, bossTriggerValue);
            if(currBossTriggerValue >= bossTriggerValue)
            {
                ResetBossTrigger();
                stageManager.enemySpawner.SpawnBoss();
            }
            //update UI Value on GameMenuUI
        }

        public void ResetBossTrigger()
        {
            currBossTriggerValue = 0;
            var gameUI = uiManager.currentActiveUI as GameMenuUI;
            gameUI.UpdateBossTriggerUI(currBossTriggerValue, bossTriggerValue);
        }
    }
}