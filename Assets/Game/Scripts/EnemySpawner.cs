using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private float topSpawnHorizontalPadding = 1.2f;

        [Header("Boss Spawn")]
        public GameObject bossPrefab;
        public Transform bossSpawnPoint;
        public GameObject currBossObject;
        public ParticleSystem ShowBossVFX;

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

        // 🔑 SPAWNER STATE
        private bool spawnerEnabled = true;
        private bool bossAlive;

        public StageManager stageManager;

        // 🔑 TRACK SPAWNED ENEMIES
        [SerializeField] private List<GameObject> aliveEnemies = new();

        // ─────────────────────────────
        public void Init(StageManager stageManager)
        {
            this.stageManager = stageManager;
            mainCam = Camera.main;
            isInit = true;
            timer = 0f;
        }

        public void DoUpdate(float dt)
        {
            if (!isInit || !spawnerEnabled || bossAlive)
                return;

            if (currentEnemyCount >= maxEnemiesOnScreen)
                return;

            timer += dt;
            if (timer >= spawnInterval)
            {
                if (!spawnerEnabled) return;
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

            if (enemyParent)
                enemy.transform.SetParent(enemyParent, true);

            ForceZZero(enemy.transform);

            currentEnemyCount++;
            aliveEnemies.Add(enemy);

            EnemyStatus status = enemy.GetComponent<EnemyStatus>();
            if (status)
            {
                status.Init(stageManager);
                status.onDeath.AddListener(() => OnEnemyDeath(enemy));
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

        void OnEnemyDeath(GameObject enemy)
        {
            currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
            aliveEnemies.Remove(enemy);
        }

        #endregion

        // ─────────────────────────────
        #region BOSS LOGIC

        public void SpawnBoss()
        {
            if (bossAlive || bossPrefab == null)
                return;

            StopSpawner();
            bossAlive = true;
            BossScoreDecreaseVisual();
            StartCoroutine(ShowBossSequence());
        }

        IEnumerator ShowBossSequence()
        {
            yield return new WaitForSeconds(0.2f);
            ShowBossVFX.Play();

            yield return new WaitForSeconds(1.5f);

            Vector3 worldPos = bossSpawnPoint
                ? bossSpawnPoint.position
                : Vector3.zero;

            worldPos.z = 0f;

            currBossObject = Instantiate(bossPrefab, worldPos, Quaternion.identity);

            if (enemyParent)
                currBossObject.transform.SetParent(enemyParent, true);

            ForceZZero(currBossObject.transform);

            BossStatus bossStatus = currBossObject.GetComponent<BossStatus>();
            if (bossStatus)
            {
                bossStatus.Init(stageManager);
                bossStatus.onDeath.AddListener(OnBossDeath);
            }

            BossShoot shoot = currBossObject.GetComponent<BossShoot>();
            if (shoot)
            {
                shoot.Init(stageManager);
            }

        }

        void OnBossDeath()
        {
            bossAlive = false;
            stageManager.OnWinBoss();
        }

        #endregion

        // ─────────────────────────────
        #region SPAWNER CONTROL

        public void StartSpawner()
        {
            spawnerEnabled = true;
            timer = 0f; // prevent instant spawn burst
        }

        public void StopSpawner()
        {
            spawnerEnabled = false;
        }

        public bool IsSpawnerRunning()
        {
            return spawnerEnabled;
        }

        #endregion

        // ─────────────────────────────
        #region RESET LOGIC

        public void ResetEnemies()
        {
            // Destroy all alive enemies
            for (int i = aliveEnemies.Count - 1; i >= 0; i--)
            {
                if (aliveEnemies[i])
                    Destroy(aliveEnemies[i]);
            }

            aliveEnemies.Clear();
            currentEnemyCount = 0;
            timer = 0f;

            // Destroy boss if exists
            if (currBossObject)
            {
                Destroy(currBossObject);
                currBossObject = null;
            }

            bossAlive = false;
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

        void BossScoreDecreaseVisual()
        {
            Vector3 startPos = stageManager.gameController.scoreTargetTransform.position;
            Transform endPos = bossSpawnPoint;

            int spawnCount = 5;
            float interval = 0.2f;

            Sequence seq = DOTween.Sequence();

            for (int i = 0; i < spawnCount; i++)
            {
                seq.AppendCallback(() =>
                {
                    GameObject vfxObj = VFXPool.Instance.Spawn(
                        "score",
                        stageManager.gameController.scoreTargetTransform.position
                    );

                    var fly = vfxObj.GetComponent<ScoreVFX>();
                    if (fly != null)
                    {
                        fly.InitNoScore(
                            stageManager,
                            startPos,
                            endPos
                        );
                    }
                });

                seq.AppendInterval(interval);
            }
        }

    }
}
