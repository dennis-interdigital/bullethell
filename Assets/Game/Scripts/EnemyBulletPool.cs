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
            return GetFromPool(enemyBulletPrefab, enemyPool);
        }

        public EnemyBulletScript GetBossBullet()
        {
            return GetFromPool(bossBulletPrefab, bossPool);
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
            ReturnToPool(b, enemyPool);
        }

        public void ReturnBossBullet(EnemyBulletScript b)
        {
            ReturnToPool(b, bossPool);
        }

        void ReturnToPool(
            EnemyBulletScript b,
            Queue<EnemyBulletScript> pool)
        {
            if (!b) return;

            b.gameObject.SetActive(false);
            b.transform.SetParent(transform);
            pool.Enqueue(b);
        }
    }
}
