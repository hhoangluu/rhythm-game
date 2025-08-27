using UnityEngine;
using System.Collections.Generic;

namespace Doulingo.Core
{
    /// <summary>
    /// Simple, generic object pooling system for MonoBehaviour objects.
    /// Rule: All objects spawned from the same pool must keep the same name.
    /// This allows Return() to work without needing the prefab reference.
    /// </summary>
    /// <typeparam name="T">Type of MonoBehaviour to pool</typeparam>
    public static class ObjectPool<T> where T : MonoBehaviour
    {
        // Pool storage
        private static readonly Dictionary<string, Queue<T>> pools = new Dictionary<string, Queue<T>>();
        private static readonly Dictionary<string, List<T>> allPooledObjects = new Dictionary<string, List<T>>();
        
        // Default settings
        private const int DEFAULT_POOL_SIZE = 5;

        /// <summary>
        /// Get an object from the pool (auto-configures if needed)
        /// </summary>
        /// <param name="prefab">The prefab to get from pool</param>
        /// <param name="position">Position to spawn at</param>
        /// <returns>Pooled object or null if pool is exhausted</returns>
        public static T Get(T prefab, Vector3 position)
        {
            string poolKey = GetPoolKey(prefab);
            
            // Auto-configure pool if not exists
            if (!pools.ContainsKey(poolKey))
            {
                ConfigurePool(prefab);
            }
            
            T pooledObject = null;
            
            // Try to get from pool
            if (pools[poolKey].Count > 0)
            {
                pooledObject = pools[poolKey].Dequeue();
            }
            else
            {
                // Create more if pool is empty
                CreatePooledObject(prefab, poolKey);
                pooledObject = pools[poolKey].Dequeue();
            }
            
            // Position and activate the object
            if (pooledObject != null)
            {
                pooledObject.transform.position = position;
                pooledObject.gameObject.SetActive(true);
            }
            
            return pooledObject;
        }

        /// <summary>
        /// Return an object to the pool using its name to identify the pool
        /// </summary>
        /// <param name="obj">Object to return to pool</param>
        public static void Return(T obj)
        {
            if (obj == null) return;
            
            string poolKey = obj.gameObject.name;
            
            if (!pools.ContainsKey(poolKey))
            {
                Debug.LogWarning($"[ObjectPool<{typeof(T).Name}>] Attempting to return object '{poolKey}' to non-existent pool");
                return;
            }
            
            // Reset object state
            obj.gameObject.SetActive(false);
            obj.transform.position = Vector3.zero;
            obj.transform.localScale = Vector3.one;
            
            // Return to pool
            pools[poolKey].Enqueue(obj);
        }

        /// <summary>
        /// Clear all pools
        /// </summary>
        public static void ClearAll()
        {
            foreach (var kvp in pools)
            {
                string poolKey = kvp.Key;
                
                // Destroy all pooled objects
                foreach (var obj in allPooledObjects[poolKey])
                {
                    if (obj != null)
                    {
                        Object.Destroy(obj.gameObject);
                    }
                }
                
                kvp.Value.Clear();
                allPooledObjects[poolKey].Clear();
            }
            
            pools.Clear();
            allPooledObjects.Clear();
            
            Debug.Log($"[ObjectPool<{typeof(T).Name}>] Cleared all pools");
        }

        // Private helper methods
        private static string GetPoolKey(T prefab)
        {
            if (prefab == null)
            {
                return "null_prefab";
            }
            
            // Use prefab name as pool key
            return prefab.name;
        }

        private static void ConfigurePool(T prefab)
        {
            string poolKey = GetPoolKey(prefab);
            
            pools[poolKey] = new Queue<T>();
            allPooledObjects[poolKey] = new List<T>();
            
            // Pre-populate the pool
            for (int i = 0; i < DEFAULT_POOL_SIZE; i++)
            {
                CreatePooledObject(prefab, poolKey);
            }
            
            Debug.Log($"[ObjectPool<{typeof(T).Name}>] Auto-configured pool '{poolKey}' with {DEFAULT_POOL_SIZE} objects");
        }

        private static void CreatePooledObject(T prefab, string poolKey)
        {
            T obj = Object.Instantiate(prefab);
            obj.gameObject.SetActive(false);
            
            // Enforce the rule: all objects in the same pool must have the same name
            obj.gameObject.name = poolKey;
            
            pools[poolKey].Enqueue(obj);
            allPooledObjects[poolKey].Add(obj);
        }
    }
}
