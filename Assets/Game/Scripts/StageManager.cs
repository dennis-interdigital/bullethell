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
        public PlayerMovement playerMovement;
        public PlayerShoot playerShoot;
        public EnemySpawner enemySpawner;
        public EnemyBulletPool enemyBulletPool;
        public Canvas worldCanvas;

        void Start()
        {
            playerMovement.Init(this);
            roadCreator.Init(this);
            gameController.Init(this);  
            uiManager.Init(this);
            uiManager.ShowUI(UIState.TitleMenu);
        }
        void FixedUpdate()
        {
            float dt = Time.deltaTime;
            float tt = Time.time;
            playerMovement.DoUpdate(dt);
            playerShoot.DoUpdate(dt);
            enemySpawner.DoUpdate(dt);
            roadCreator.DoUpdate();
            uiManager.DoUpdate(dt);
        }

        public void StartGame()
        {
            playerShoot.Init(this);
            enemyBulletPool.Init();
            enemySpawner.Init(this);
            uiManager.ShowUI(UIState.GameMenu);
        }
    }
}
