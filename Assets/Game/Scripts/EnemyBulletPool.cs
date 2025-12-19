using System.Collections.Generic;
using UnityEngine;
namespace bullethell
{
    public class EnemyBulletPool : MonoBehaviour
    {
        [SerializeField] private EnemyBulletScript bulletPrefab;
        [SerializeField] private int initialSize = 20;

        private readonly Queue<EnemyBulletScript> _pool = new();


        public void Init()
        {
            for (int i = 0; i < initialSize; i++)
            {
                var b = Instantiate(bulletPrefab, transform);
                b.SetPool(this);
                b.gameObject.SetActive(false);
                _pool.Enqueue(b);
            }
        }


        public EnemyBulletScript Get()
        {
            if (_pool.Count == 0)
            {
                var b = Instantiate(bulletPrefab, transform);
                b.SetPool(this);
                b.gameObject.SetActive(false);
                return b;
            }
            return _pool.Dequeue();
        }

        public void Return(EnemyBulletScript b)
        {
            if (!b) return;
            b.gameObject.SetActive(false);
            b.transform.SetParent(transform);
            _pool.Enqueue(b);
        }
    }
}
