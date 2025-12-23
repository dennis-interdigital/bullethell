using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bullethell
{
    public class PlayerManager : MonoBehaviour
    {
        public PlayerMovement playerMovement;
        public PlayerShoot playerShoot;
        public PlayerStatus playerStatus;

        [HideInInspector] public StageManager stageManager;
        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;   
            playerMovement.Init(stageManager);
            playerShoot.Init(stageManager);
            playerStatus.Init(stageManager);

            playerStatus.onDeath.AddListener(OnPlayerDead);
        }

        void OnPlayerDead()
        {
            playerMovement.StopMovement();
            playerShoot.StopShooting();
            this.transform.DOLocalMove(Vector3.zero, 1f);
        }

        public void ResetPlayerState()
        {
            playerStatus.ResetHealth();
            playerMovement.StartMovement();
            playerShoot.StartShooting();
        }

        public void ContinuePlayerState()
        {
            playerMovement.StartMovement();
            playerShoot.StartShooting();
        }
    }
}

