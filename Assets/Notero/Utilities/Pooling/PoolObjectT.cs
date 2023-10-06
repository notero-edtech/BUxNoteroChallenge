using UnityEngine;

namespace Notero.Utilities.Pooling
{
    public class PoolObject<T> : PoolObject where T : PoolObject
    {
        private PoolManager m_Manager;

        public T Rent()
        {
            if(m_IsPoolInstance)
            {
                throw new System.Exception("Not instantiating from prefab");
            }

            return GetManager().Rent(this) as T;
        }

        public T Rent(Vector3 position, Quaternion rotation, Transform parent)
        {
            T obj = Rent();
            obj.transform.SetParent(parent);
            obj.transform.SetPositionAndRotation(position, rotation);

            return obj;
        }

        public T Rent(Transform parent)
        {
            T obj = Rent();
            obj.transform.SetParent(parent);

            return obj;
        }

        private PoolManager GetManager()
        {
            if(m_Manager == null)
            {
                m_Manager = SharedContext.Instance.Get<PoolManager>();
                if(m_Manager == null)
                {
                    m_Manager = PoolManager.Create();
                    SharedContext.Instance.Add(m_Manager);
                }
            }

            return m_Manager;
        }
    }
}