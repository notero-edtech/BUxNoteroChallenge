using System.Collections.Generic;
using UnityEngine;

namespace Notero.Utilities.Pooling
{
    /// <summary>
    /// manager managing pools
    /// new pool will be created for each prefab
    /// empty pools are preriodically removed
    /// </summary>
    public class PoolManager : MonoBehaviour
    {
        private const float m_CleanInterval = 1;
        private readonly Dictionary<PoolObject, Pool> m_PoolMap = new Dictionary<PoolObject, Pool>();
        private float m_NextClean;

        public static PoolManager Create()
        {
            GameObject gameObject = new GameObject(nameof(PoolManager));
            DontDestroyOnLoad(gameObject);

            return gameObject.AddComponent<PoolManager>();
        }

        public T Rent<T>(T prefab) where T : PoolObject
        {
            if(!m_PoolMap.TryGetValue(prefab, out Pool pool))
            {
                pool = new Pool(prefab);
                m_PoolMap.Add(prefab, pool);
            }

            return pool.Rent() as T;
        }

        private void Update()
        {
            if(Time.time > m_NextClean)
            {
                m_NextClean = Time.time + m_CleanInterval;

                PoolObject emptyKey = null;

                // remove expired free objects
                foreach(var pair in m_PoolMap)
                {
                    if(pair.Value.IsEmpty)
                    {
                        emptyKey = pair.Key;
                    }
                    else
                    {
                        pair.Value.RemoveExpired();
                    }
                }

                // remove empty pool
                if(emptyKey != null)
                {
                    m_PoolMap[emptyKey].MarkDestroyed();
                    m_PoolMap.Remove(emptyKey);
                }
            }
        }

        private void OnDestroy()
        {
            foreach(Pool pool in m_PoolMap.Values)
            {
                pool.MarkDestroyed();
            }
        }
    }
}