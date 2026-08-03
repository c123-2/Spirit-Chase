using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// \u901a\u7528\u5bf9\u8c61\u6c60\uff0c\u7528\u4e8e\u9891\u7e41\u521b\u5efa\u9500\u6bc1\u7684\u5bf9\u8c61\uff08\u7c92\u5b50\u3001\u97f3\u6548\u3001Buff\u7279\u6548\u7b49\uff09\u3002
/// \u4efb\u52a1 1.4\uff1aObjectPool \u901a\u7528\u5bf9\u8c61\u6c60
/// </summary>
public class ObjectPool : Singleton<ObjectPool>
{
    [Serializable]
    public class PoolConfig
    {
        public string key;
        public GameObject prefab;
        public int initialSize = 5;
        public int maxSize = 50;
        public bool autoExpand = true;
    }

    [SerializeField] private List<PoolConfig> _poolConfigs = new List<PoolConfig>();

    private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
    private readonly Dictionary<string, PoolConfig> _configMap = new Dictionary<string, PoolConfig>();
    private Transform _poolRoot;

    protected override void Awake()
    {
        base.Awake();
        _poolRoot = transform;
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in _poolConfigs)
        {
            RegisterPool(config.key, config.prefab, config.initialSize, config.maxSize, config.autoExpand);
        }
    }

    /// <summary>\u6ce8\u518c\u4e00\u4e2a\u65b0\u7684\u5bf9\u8c61\u6c60</summary>
    public void RegisterPool(string key, GameObject prefab, int initialSize = 5, int maxSize = 50, bool autoExpand = true)
    {
        if (_configMap.ContainsKey(key))
        {
            Debug.LogWarning($"[ObjectPool] Pool '{key}' already exists.");
            return;
        }

        var config = new PoolConfig
        {
            key = key,
            prefab = prefab,
            initialSize = initialSize,
            maxSize = maxSize,
            autoExpand = autoExpand
        };
        _configMap[key] = config;

        var queue = new Queue<GameObject>();
        for (int i = 0; i < initialSize; i++)
        {
            var obj = CreateNew(key, prefab);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
        _pools[key] = queue;
    }

    /// <summary>\u4ece\u6c60\u4e2d\u83b7\u53d6\u5bf9\u8c61</summary>
    public GameObject Get(string key)
    {
        if (!_pools.TryGetValue(key, out var queue))
        {
            Debug.LogError($"[ObjectPool] Pool '{key}' not found!");
            return null;
        }

        if (queue.Count == 0)
        {
            var config = _configMap[key];
            if (config.autoExpand && GetActiveCount(key) < config.maxSize)
            {
                var newObj = CreateNew(key, config.prefab);
                newObj.SetActive(true);
                return newObj;
            }
            Debug.LogWarning($"[ObjectPool] Pool '{key}' exhausted, returning null.");
            return null;
        }

        var obj = queue.Dequeue();
        if (obj != null)
        {
            obj.SetActive(true);
        }
        return obj;
    }

    /// <summary>\u5c06\u5bf9\u8c61\u5f52\u8fd8\u6c60\u4e2d</summary>
    public void Return(string key, GameObject obj)
    {
        if (obj == null) return;

        if (!_pools.TryGetValue(key, out var queue))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(_poolRoot);
        obj.transform.localPosition = Vector3.zero;
        queue.Enqueue(obj);
    }

    /// <summary>\u5ef6\u8fdf\u5f52\u8fd8\u5bf9\u8c61</summary>
    public void ReturnDelayed(string key, GameObject obj, float delay)
    {
        if (obj == null) return;
        StartCoroutine(ReturnRoutine(key, obj, delay));
    }

    private System.Collections.IEnumerator ReturnRoutine(string key, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Return(key, obj);
    }

    private GameObject CreateNew(string key, GameObject prefab)
    {
        var obj = Instantiate(prefab, _poolRoot);
        obj.name = $"{key}_{GetActiveCount(key)}";
        return obj;
    }

    private int GetActiveCount(string key)
    {
        if (!_pools.TryGetValue(key, out var queue)) return 0;
        int count = 0;
        foreach (var obj in queue)
        {
            if (obj != null && obj.activeInHierarchy) count++;
        }
        return count;
    }

    /// <summary>\u6e05\u7a7a\u6307\u5b9a\u6c60</summary>
    public void ClearPool(string key)
    {
        if (_pools.TryGetValue(key, out var queue))
        {
            while (queue.Count > 0)
            {
                var obj = queue.Dequeue();
                if (obj != null) Destroy(obj);
            }
        }
    }

    /// <summary>\u6e05\u7a7a\u6240\u6709\u6c60</summary>
    public void ClearAll()
    {
        foreach (var kv in _pools)
        {
            while (kv.Value.Count > 0)
            {
                var obj = kv.Value.Dequeue();
                if (obj != null) Destroy(obj);
            }
        }
        _pools.Clear();
        _configMap.Clear();
    }
}
