using System.Collections.Generic;
using UnityEngine;
namespace BulletHell
{
    public class BulletPool : MonoBehaviour
    {
        [SerializeField] private BulletScript bulletPrefab;
        [SerializeField] private int initialSize = 20;

        private readonly Queue<BulletScript> _pool = new Queue<BulletScript>();

        void Awake()
        {
            Prewarm();
        }

        private void Prewarm()
        {
            for (int i = 0; i < initialSize; i++)
            {
                BulletScript b = Instantiate(bulletPrefab, transform);
                b.SetPool(this);
                b.gameObject.SetActive(false);
                _pool.Enqueue(b);
            }
        }

        public BulletScript Get()
        {
            if (_pool.Count == 0)
            {
                // Expand
                BulletScript b = Instantiate(bulletPrefab, transform);
                b.SetPool(this);
                b.gameObject.SetActive(false);
                return b;
            }
            return _pool.Dequeue();
        }

        public void Return(BulletScript bullet)
        {
            bullet.gameObject.SetActive(false);
            _pool.Enqueue(bullet);
        }
    }
}
