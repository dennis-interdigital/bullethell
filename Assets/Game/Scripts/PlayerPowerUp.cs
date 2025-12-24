using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BulletHell
{
    public class PlayerPowerUp : MonoBehaviour
    {
        StageManager stageManager;
        PlayerManager playerManager;
        PlayerShoot playerShoot;

        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            playerManager = stageManager.playerManager;
            playerShoot = playerManager.playerShoot;
        }

        public void ChangePlayerPower()
        {
            PlayerShoot.ShootType type = PlayerShoot.ShootType.Triple;
            playerShoot.SetShootType(type);
        }
    }

}
