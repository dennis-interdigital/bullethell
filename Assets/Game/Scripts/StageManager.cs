using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bullethell
{
    public class StageManager : MonoBehaviour
    {
        [Header("Game Modules")]
        public GameController gameController;
        public UIManager uiManager;
        public RoadCreator roadCreator;
        public PlayerManager playerManager;
        public EnemySpawner enemySpawner;
        public EnemyBulletPool enemyBulletPool;
        public Canvas worldCanvas;

        void Start()
        {           
            roadCreator.Init(this);
            gameController.Init(this);  
            uiManager.Init(this);
            uiManager.ShowUI(UIState.TitleMenu);
            playerManager.playerStatus.onDeath.AddListener(OnLoseGame);
        }
        void FixedUpdate()
        {
            float dt = Time.deltaTime;
            float tt = Time.time;
            playerManager.playerMovement.DoUpdate(dt);
            playerManager.playerShoot.DoUpdate(dt);
            enemySpawner.DoUpdate(dt);
            roadCreator.DoUpdate();
            uiManager.DoUpdate(dt);
        }

        public void StartGame()
        {
            playerManager.Init(this);
            enemyBulletPool.Init();
            enemySpawner.Init(this);
            uiManager.ShowUI(UIState.GameMenu);
        }

        public void RetryGame()
        {
            playerManager.ResetPlayerState();
            enemySpawner.StartSpawner();
            uiManager.ShowUI(UIState.GameMenu);
        }

        public void ContinueGame()
        {
            playerManager.ContinuePlayerState();
            enemySpawner.StartSpawner();
            uiManager.ShowUI(UIState.GameMenu);
        }

        void OnLoseGame()
        {
            enemyBulletPool.ClearAllBullets();
            enemySpawner.StopSpawner();
            enemySpawner.ResetEnemies();
            DOVirtual.DelayedCall(3f, () =>
            {
                uiManager.ShowUI(UIState.LoseMenu);
            });
        }

        public void OnWinBoss()
        {

            enemySpawner.StopSpawner();
            enemySpawner.ResetEnemies();
            DOVirtual.DelayedCall(3f, () =>
            {
                playerManager.playerMovement.StopMovement();
                playerManager.playerShoot.StopShooting();
                uiManager.ShowUI(UIState.WinMenu);
            });
        }
    }
}
