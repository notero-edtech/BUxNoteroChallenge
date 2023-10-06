using System.Collections.Generic;
using UnityEngine;

namespace Notero.Utilities.Pooling
{
    public class Pool
    {
        public bool IsEmpty => FreeCount + RentedCount == 0;

        public int RentedCount { get; private set; }

        public int FreeCount => m_FreeObjects.Count;

        /// <summary>
        /// recent objects are stay at the back of the list
        /// older objects staying at the front are candidate for removal
        /// </summary>
        private readonly LinkedList<PoolObject> m_FreeObjects = new LinkedList<PoolObject>();

        private PoolObject m_Prefab;
        private bool m_IsDestroyed;

        public Pool(PoolObject prefab)
        {
            m_Prefab = prefab;
        }

        /// <summary>
        /// rent an object out of this pool
        /// </summary>
        public PoolObject Rent()
        {
            PoolObject result = Pop();

            if(result == null)
            {
                result = Object.Instantiate(m_Prefab);
                result.SetPool(this);
            }

            RentedCount++;
            result.MarkRented();
            return result;
        }

        /// <summary>
        /// remove and destroy expired free objects
        /// </summary>
        public void RemoveExpired()
        {
            // check from the front of the list (older object)
            int maxIteration = m_FreeObjects.Count;
            for(int i = 0; i < maxIteration && m_FreeObjects.Count > 0; i++)
            {
                PoolObject obj = m_FreeObjects.First.Value;
                if(obj.IsDestroyed || obj.IsExpired)
                {
                    m_FreeObjects.RemoveFirst();
                    if(!obj.IsDestroyed)
                    {
                        Object.Destroy(obj.gameObject);
                    }
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// mark this pool as destroyed
        /// object returned after this will be destroyed
        /// </summary>
        public void MarkDestroyed()
        {
            m_IsDestroyed = true;
        }

        /// <summary>
        /// return object to the pool
        /// </summary>
        internal void Return(PoolObject obj)
        {
            RentedCount--;

            if(m_IsDestroyed && !obj.IsDestroyed)
            {
                Debug.Log($"{obj.GetType().Name} returned to a destroyed pool, the object will be destroyed");
                Object.Destroy(obj);
            }
            else if(!obj.IsDestroyed)
            {
                // push to the back of the list
                m_FreeObjects.AddLast(obj);
            }
        }

        private PoolObject Pop()
        {
            // pop out of the back of the list
            PoolObject result = null;
            if(m_FreeObjects.Count > 0)
            {
                result = m_FreeObjects.Last.Value;
                m_FreeObjects.RemoveLast();
            }
            return result;
        }
    }
}