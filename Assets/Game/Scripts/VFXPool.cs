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
            public GameObject prefab;
            public int initialSize = 8;
            public bool autoReturnParticle = true; // auto return if has ParticleSystem
        }

        public static VFXPool Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private Transform vfxPoolParent;
        [SerializeField] private List<VFXEntry> vfxCatalog = new();

        private readonly Dictionary<string, Queue<GameObject>> pools = new();
        private readonly Dictionary<GameObject, string> instanceKey = new();

        // ─────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            if (vfxPoolParent == null)
            {
                GameObject parentObj = new GameObject("[VFX Pool]");
                vfxPoolParent = parentObj.transform;
            }

            InitializePools();
        }

        // ─────────────────────────────
        private void InitializePools()
        {
            foreach (var entry in vfxCatalog)
            {
                if (entry.prefab == null || string.IsNullOrEmpty(entry.key))
                    continue;

                Queue<GameObject> queue = new();

                for (int i = 0; i < entry.initialSize; i++)
                {
                    GameObject obj = Instantiate(entry.prefab, vfxPoolParent);
                    obj.SetActive(false);
                    instanceKey[obj] = entry.key;
                    queue.Enqueue(obj);
                }

                pools[entry.key] = queue;
            }
        }

        // ─────────────────────────────
        /// <summary>
        /// Spawn a pooled VFX GameObject at world position
        /// </summary>
        public GameObject Spawn(string key, Vector3 position)
        {
            GameObject obj = GetOrCreate(key);
            if (!obj) return null;

            obj.transform.SetParent(null);
            obj.transform.position = position;
            obj.SetActive(true);

            // Auto-play particles if present
            var ps = obj.GetComponentInChildren<ParticleSystem>();
            if (ps)
            {
                ps.Clear(true);
                ps.Play(true);

                var entry = GetEntry(key);
                if (entry != null && entry.autoReturnParticle)
                    StartCoroutine(ReturnWhenParticleDone(obj, ps));
            }

            return obj;
        }

        // ─────────────────────────────
        private GameObject GetOrCreate(string key)
        {
            if (!pools.TryGetValue(key, out var queue) || queue.Count == 0)
            {
                var entry = GetEntry(key);
                if (entry == null || entry.prefab == null)
                {
                    Debug.LogError($"VFXPool: No prefab found for key '{key}'.");
                    return null;
                }

                GameObject obj = Instantiate(entry.prefab, vfxPoolParent);
                obj.SetActive(false);
                instanceKey[obj] = key;
                return obj;
            }

            return queue.Dequeue();
        }

        private VFXEntry GetEntry(string key)
        {
            return vfxCatalog.Find(e => e.key == key);
        }

        // ─────────────────────────────
        IEnumerator ReturnWhenParticleDone(GameObject obj, ParticleSystem ps)
        {
            yield return new WaitWhile(() => ps != null && ps.IsAlive(true));
            Return(obj);
        }

        // ─────────────────────────────
        public void Return(GameObject obj)
        {
            if (!obj) return;

            obj.SetActive(false);
            obj.transform.SetParent(vfxPoolParent);

            if (!instanceKey.TryGetValue(obj, out string key))
                return;

            if (!pools.ContainsKey(key))
                pools[key] = new Queue<GameObject>();

            pools[key].Enqueue(obj);
        }
    }
}
