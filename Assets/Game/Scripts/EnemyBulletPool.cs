using System.Collections.Generic;
using UnityEngine;

namespace bullethell
{
    public class EnemyBulletPool : MonoBehaviour
    {
        [Header("Enemy Bullet")]
        [SerializeField] private EnemyBulletScript enemyBulletPrefab;
        [SerializeField] private int enemyInitialSize = 20;

        [Header("Boss Bullet")]
        [SerializeField] private EnemyBulletScript bossBulletPrefab;
        [SerializeField] private int bossInitialSize = 30;

        private readonly Queue<EnemyBulletScript> enemyPool = new();
        private readonly Queue<EnemyBulletScript> bossPool = new();

        // 🔑 TRACK ACTIVE BULLETS
        private readonly HashSet<EnemyBulletScript> activeEnemyBullets = new();
        private readonly HashSet<EnemyBulletScript> activeBossBullets = new();

        // ─────────────────────────────
        public void Init()
        {
            WarmPool(enemyBulletPrefab, enemyInitialSize, enemyPool);
            WarmPool(bossBulletPrefab, bossInitialSize, bossPool);
        }

        void WarmPool(
            EnemyBulletScript prefab,
            int count,
            Queue<EnemyBulletScript> pool)
        {
            if (!prefab) return;

            for (int i = 0; i < count; i++)
            {
                var b = Instantiate(prefab, transform);
                b.SetPool(this);
                b.gameObject.SetActive(false);
                pool.Enqueue(b);
            }
        }

        // ─────────────────────────────
        public EnemyBulletScript GetEnemyBullet()
        {
            EnemyBulletScript b = GetFromPool(enemyBulletPrefab, enemyPool);
            activeEnemyBullets.Add(b);
            return b;
        }

        public EnemyBulletScript GetBossBullet()
        {
            EnemyBulletScript b = GetFromPool(bossBulletPrefab, bossPool);
            activeBossBullets.Add(b);
            return b;
        }

        EnemyBulletScript GetFromPool(
            EnemyBulletScript prefab,
            Queue<EnemyBulletScript> pool)
        {
            if (pool.Count > 0)
                return pool.Dequeue();

            var b = Instantiate(prefab, transform);
            b.SetPool(this);
            b.gameObject.SetActive(false);
            return b;
        }

        // ─────────────────────────────
        public void ReturnEnemyBullet(EnemyBulletScript b)
        {
            if (!b) return;
            activeEnemyBullets.Remove(b);
            ReturnToPool(b, enemyPool);
        }

        public void ReturnBossBullet(EnemyBulletScript b)
        {
            if (!b) return;
            activeBossBullets.Remove(b);
            ReturnToPool(b, bossPool);
        }

        void ReturnToPool(
            EnemyBulletScript b,
            Queue<EnemyBulletScript> pool)
        {
            b.gameObject.SetActive(false);
            b.transform.SetParent(transform);
            pool.Enqueue(b);
        }

        public void ClearAllBullets()
        {
            // Enemy bullets
            foreach (var b in activeEnemyBullets)
            {
                if (b)
                {
                    b.gameObject.SetActive(false);
                    b.transform.SetParent(transform);
                    enemyPool.Enqueue(b);
                }
            }
            activeEnemyBullets.Clear();

            // Boss bullets
            foreach (var b in activeBossBullets)
            {
                if (b)
                {
                    b.gameObject.SetActive(false);
                    b.transform.SetParent(transform);
                    bossPool.Enqueue(b);
                }
            }
            activeBossBullets.Clear();
        }
    }
}
