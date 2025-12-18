using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public enum SpawnSide
    {
        Top,
        Left,
        Right
    }

    [Header("Spawn")]
    public GameObject enemyPrefab;
    public Transform enemyParent;
    public float spawnInterval = 1.5f;
    public float spawnOutsideOffset = 1.5f;

    [Header("Limit")]
    public int maxEnemiesOnScreen = 10;     // 🔑 NEW
    int currentEnemyCount = 0;

    [Header("Spawn Sides")]
    public bool spawnTop = true;
    public bool spawnLeft = true;
    public bool spawnRight = true;

    [Header("Random Shoot Patterns")]
    public bool allowDownward = true;
    public bool allowAimed = true;
    public bool allowRing = true;
    public bool allowSpiral = true;
    public bool allowFan = true;

    Camera mainCam;
    float timer;
    public StageManager stageManager;

    Vector2 enemyHalfSize;
    public bool isInit = false;

    public void Init(StageManager stageManager)
    {
        mainCam = Camera.main;
        this.stageManager = stageManager;
        CacheEnemyHalfSize();
        isInit = true;
    }

    public void DoUpdate(float dt)
    {
        if(isInit)
        {
            if (currentEnemyCount >= maxEnemiesOnScreen)
                return;

            timer += dt;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
    }

    // ─────────────────────────────
    void CacheEnemyHalfSize()
    {
        enemyHalfSize = Vector2.zero;

        if (!enemyPrefab) return;

        Renderer r = enemyPrefab.GetComponentInChildren<Renderer>();
        if (!r) return;

        Bounds b = r.bounds;
        enemyHalfSize = new Vector2(b.extents.x, b.extents.y);
    }

    void SpawnEnemy()
    {
        if (currentEnemyCount >= maxEnemiesOnScreen)
            return;

        SpawnSide side = GetRandomSide();
        Vector3 spawnLocation = GetSpawnPosition(side);

        Vector3 spawnPos = new Vector3(
            spawnLocation.x,
            spawnLocation.y,
            enemyParent.position.z
        );

        GameObject enemy = Instantiate(
            enemyPrefab,
            spawnPos,
            Quaternion.identity,
            enemyParent
        );

        currentEnemyCount++; // 🔑 increment on spawn

        EnemyStatus enemyStatus = enemy.GetComponent<EnemyStatus>();
        if (enemyStatus)
        {
            enemyStatus.Init(stageManager.worldCanvas);

            // 🔑 subscribe to death event
            enemyStatus.onDeath.AddListener(OnEnemyDeath);
        }

        EnemyBulletPool bulletPool = stageManager.enemyBulletPool;
        EnemyShoot enemyShoot = enemy.GetComponent<EnemyShoot>();
        enemyShoot.Init(bulletPool);

        EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
        if (movement)
        {
            movement.entryDirection = side switch
            {
                SpawnSide.Top => EnemyMovement.EntryDirection.FromTop,
                SpawnSide.Left => EnemyMovement.EntryDirection.FromLeft,
                SpawnSide.Right => EnemyMovement.EntryDirection.FromRight,
                _ => EnemyMovement.EntryDirection.FromTop
            };
        }

        EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
        if (shoot)
        {
            shoot.pattern = GetRandomShootPattern();
        }
    }

    // 🔑 CALLED WHEN ENEMY DIES
    void OnEnemyDeath()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }

    // ─────────────────────────────
    EnemyShoot.BulletPattern GetRandomShootPattern()
    {
        List<EnemyShoot.BulletPattern> patterns = new();

        if (allowDownward) patterns.Add(EnemyShoot.BulletPattern.Downward);
        if (allowAimed) patterns.Add(EnemyShoot.BulletPattern.Aimed);
        if (allowRing) patterns.Add(EnemyShoot.BulletPattern.Ring);
        if (allowSpiral) patterns.Add(EnemyShoot.BulletPattern.Spiral);
        if (allowFan) patterns.Add(EnemyShoot.BulletPattern.Fan);

        return patterns[Random.Range(0, patterns.Count)];
    }

    // ─────────────────────────────
    Vector3 GetSpawnPosition(SpawnSide side)
    {
        float camCenterY = mainCam.transform.position.y;
        float camTop = camCenterY + mainCam.orthographicSize;
        float camRight = mainCam.transform.position.x + mainCam.orthographicSize * mainCam.aspect;
        float camLeft = mainCam.transform.position.x - mainCam.orthographicSize * mainCam.aspect;

        float spawnMinY = camCenterY;
        float spawnMaxY = camTop;

        float minX = camLeft + enemyHalfSize.x;
        float maxX = camRight - enemyHalfSize.x;

        float minY = spawnMinY + enemyHalfSize.y;
        float maxY = spawnMaxY - enemyHalfSize.y;

        return side switch
        {
            SpawnSide.Top => new Vector3(
                Random.Range(minX, maxX),
                camTop + spawnOutsideOffset + enemyHalfSize.y,
                0f
            ),

            SpawnSide.Left => new Vector3(
                camLeft - spawnOutsideOffset - enemyHalfSize.x,
                Random.Range(minY, maxY),
                0f
            ),

            SpawnSide.Right => new Vector3(
                camRight + spawnOutsideOffset + enemyHalfSize.x,
                Random.Range(minY, maxY),
                0f
            ),

            _ => Vector3.zero
        };
    }

    SpawnSide GetRandomSide()
    {
        List<SpawnSide> sides = new();
        if (spawnTop) sides.Add(SpawnSide.Top);
        if (spawnLeft) sides.Add(SpawnSide.Left);
        if (spawnRight) sides.Add(SpawnSide.Right);

        return sides[Random.Range(0, sides.Count)];
    }
}
