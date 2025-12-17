using Plane.Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Game Modules")]
    public GameController gameController;
    public RoadCreator roadCreator;
    public PlayerMovement playerMovement;
    public PlayerShoot playerShoot;

    void Start()
    {
        playerMovement.Init(this);
        roadCreator.Init(this);
        playerShoot.Init(this);
    }
    void FixedUpdate()
    {
        float dt = Time.deltaTime;
        playerMovement.DoUpdate(dt);
        playerShoot.DoUpdate(dt);
        roadCreator.DoUpdate();
    }
}
