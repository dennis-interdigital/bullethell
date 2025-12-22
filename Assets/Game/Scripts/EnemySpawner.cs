using UnityEngine;
using System.Collections.Generic;

namespace bullethell
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Enemy Spawn")]
        public List<GameObject> enemyPrefabs;
        public Transform enemyParent;
        public float spawnInterval = 1.5f;
        public float spawnOutsideOffset = 1.5f;

        [Header("Top Spawn Padding")]
        [Tooltip("Horizontal padding so enemies don't spawn on screen edge")]
        [SerializeField] private float topSpawnHorizontalPadding = 1.2f;

        [Header("Boss Spawn")]
        public GameObject bossPrefab;
        public Transform bossSpawnPoint;

        [Header("Limit")]
        public int maxEnemiesOnScreen = 10;
        private int currentEnemyCount;

        [Header("Random Shoot Patterns")]
        public bool allowDownward = true;
        public bool allowAimed = true;
        public bool allowRing = true;
        public bool allowSpiral = true;
        public bool allowFan = true;

        private Camera mainCam;
        private float timer;
        private bool isInit;
        private bool isSpawningEnemies = true;
        private bool bossAlive;

        public StageManager stageManager;

        // ─────────────────────────────
        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            mainCam = Camera.main;
            isInit = true;
        }

        public void DoUpdate(float dt)
        {
            if (!isInit || !isSpawningEnemies || bossAlive)
                return;

            if (currentEnemyCount >= maxEnemiesOnScreen)
                return;

            timer += dt;
            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }

        // ─────────────────────────────
        #region ENEMY SPAWN (TOP ONLY)

        void SpawnEnemy()
        {
            if (enemyPrefabs == null || enemyPrefabs.Count == 0)
                return;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            Vector3 worldPos = GetTopSpawnPosition();
            worldPos.z = 0f;

            GameObject enemy = Instantiate(prefab, worldPos, Quaternion.identity);

            // Parent AFTER instantiation
            if (enemyParent)
                enemy.transform.SetParent(enemyParent, true);

            // HARD LOCK Z
            ForceZZero(enemy.transform);

            currentEnemyCount++;

            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status)
            {
                status.Init(stageManager);
                status.onDeath.AddListener(OnEnemyDeath);
            }

            EnemyShoot shoot = enemy.GetComponent<EnemyShoot>();
            if (shoot)
            {
                shoot.Init(stageManager);
                shoot.pattern = GetRandomShootPattern();
            }

            EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
            if (movement)
            {
                movement.entryDirection = EnemyMovement.EntryDirection.FromTop;
            }
        }

        void OnEnemyDeath()
        {
            currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
        }

        #endregion

        // ─────────────────────────────
        #region BOSS LOGIC

        public void SpawnBoss()
        {
            if (bossAlive || bossPrefab == null)
                return;

            bossAlive = true;
            isSpawningEnemies = false;

            Vector3 worldPos = bossSpawnPoint
                ? bossSpawnPoint.position
                : Vector3.zero;

            worldPos.z = 0f;

            GameObject boss = Instantiate(bossPrefab, worldPos, Quaternion.identity);

            if (enemyParent)
                boss.transform.SetParent(enemyParent, true);

            // HARD LOCK Z
            ForceZZero(boss.transform);

            BossStatus bossStatus = boss.GetComponent<BossStatus>();
            if (bossStatus)
            {
                bossStatus.Init(stageManager);
                bossStatus.onDeath.AddListener(OnBossDeath);
            }

            BossShoot shoot = boss.GetComponent<BossShoot>();
            if (shoot)
            {
                shoot.Init(stageManager);
            }
        }

        void OnBossDeath()
        {
            bossAlive = false;
            isSpawningEnemies = true;
            timer = 0f;
        }

        #endregion

        // ─────────────────────────────
        #region HELPERS

        Vector3 GetTopSpawnPosition()
        {
            float camH = mainCam.orthographicSize;
            float camW = camH * mainCam.aspect;
            Vector3 camPos = mainCam.transform.position;

            float minX = camPos.x - camW + topSpawnHorizontalPadding;
            float maxX = camPos.x + camW - topSpawnHorizontalPadding;

            return new Vector3(
                Random.Range(minX, maxX),
                camPos.y + camH + spawnOutsideOffset,
                0f
            );
        }

        void ForceZZero(Transform t)
        {
            Vector3 wp = t.position;
            wp.z = 0f;
            t.position = wp;

            Vector3 lp = t.localPosition;
            lp.z = 0f;
            t.localPosition = lp;
        }

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

        #endregion
    }
}
