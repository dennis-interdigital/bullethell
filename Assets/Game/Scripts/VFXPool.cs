using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace bullethell
{
    public class VFXPool : MonoBehaviour
    {
        [System.Serializable]
        public class VFXEntry
        {
            public string key;
            public ParticleSystem prefab;
            public int initialSize = 8;
        }

        public static VFXPool Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private Transform vfxPoolParent;
        [SerializeField] private List<VFXEntry> vfxCatalog = new List<VFXEntry>();

        private readonly Dictionary<string, Queue<ParticleSystem>> _pools = new Dictionary<string, Queue<ParticleSystem>>();
        private readonly Dictionary<ParticleSystem, string> _instanceKey = new Dictionary<ParticleSystem, string>();

        private void Awake()
        {
            // --- Singleton ---
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Optional: uncomment if you want it to persist between scenes
            // DontDestroyOnLoad(gameObject);

            // Ensure pool parent exists
            if (vfxPoolParent == null)
            {
                GameObject parentObj = new GameObject("[VFX Pool]");
                vfxPoolParent = parentObj.transform;
            }

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var entry in vfxCatalog)
            {
                if (entry.prefab == null || string.IsNullOrEmpty(entry.key)) continue;

                Queue<ParticleSystem> queue = new Queue<ParticleSystem>();

                for (int i = 0; i < entry.initialSize; i++)
                {
                    ParticleSystem ps = Instantiate(entry.prefab, vfxPoolParent);
                    ps.gameObject.SetActive(false);
                    _instanceKey[ps] = entry.key;
                    queue.Enqueue(ps);
                }

                _pools[entry.key] = queue;
            }
        }

        /// <summary>
        /// Spawn a particle effect from the pool at a given world position.
        /// </summary>
        public ParticleSystem Spawn(string key, Vector3 position)
        {
            ParticleSystem ps = GetOrCreate(key);
            if (ps == null) return null;

            ps.transform.position = position;
            ps.transform.SetParent(null); // detach for proper play in world
            ps.gameObject.SetActive(true);
            ps.Clear(true);
            ps.Play(true);

            StartCoroutine(ReturnWhenDone(ps));
            return ps;
        }

        private ParticleSystem GetOrCreate(string key)
        {
            if (!_pools.TryGetValue(key, out Queue<ParticleSystem> queue) || queue.Count == 0)
            {
                var entry = vfxCatalog.Find(e => e.key == key);
                if (entry == null || entry.prefab == null)
                {
                    Debug.LogError($"VFXPool: No prefab found for key '{key}'.");
                    return null;
                }

                ParticleSystem newPS = Instantiate(entry.prefab, vfxPoolParent);
                newPS.gameObject.SetActive(false);
                _instanceKey[newPS] = entry.key;
                return newPS;
            }

            return queue.Dequeue();
        }

        private IEnumerator ReturnWhenDone(ParticleSystem ps)
        {
            yield return new WaitWhile(() => ps.IsAlive(true));
            Return(ps);
        }

        public void Return(ParticleSystem ps)
        {
            if (ps == null) return;

            ps.gameObject.SetActive(false);
            ps.transform.SetParent(vfxPoolParent);

            if (!_instanceKey.TryGetValue(ps, out string key)) return;

            if (!_pools.ContainsKey(key))
                _pools[key] = new Queue<ParticleSystem>();

            _pools[key].Enqueue(ps);
        }
    }
}
