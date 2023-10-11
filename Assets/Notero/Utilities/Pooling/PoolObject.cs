using UnityEngine;

namespace Notero.Utilities.Pooling
{
    public class PoolObject : MonoBehaviour
    {
        public bool IsDestroyed { get; private set; }

        internal bool IsExpired => !m_IsRented && Time.time > m_ExpireTime;

        protected bool m_IsRented { get; private set; }

        /// <summary>
        /// indicate that this is a pool instantiated object (not original prefab)
        /// </summary>
        protected bool m_IsPoolInstance => m_Pool != null;

        private const string m_RentedNameFormat = "{0}-rented";
        private const string m_FreeNameFormat = "{0}-free";

        [SerializeField, Tooltip("Free object will be destroyed after this duration passed")]
        private float m_ExpireDuration = 10;

        private float m_ExpireTime;
        private Pool m_Pool;
        private string m_OriginalName;

        /// <summary>
        /// return object to its pool
        /// </summary>
        public void Return()
        {
            Debug.Assert(m_IsRented, "returning a returned pool object");
            Debug.Assert(m_IsPoolInstance, "returning an object without pool");

            if(!m_IsRented || !m_IsPoolInstance)
            {
                return;
            }

            m_IsRented = false;
            m_ExpireTime = Time.time + m_ExpireDuration;
            m_Pool.Return(this);

            if(!IsDestroyed)
            {
                name = string.Format(m_FreeNameFormat, m_OriginalName);
                OnReturned();
            }
        }

        internal void SetPool(Pool pool)
        {
            m_Pool = pool;
            m_OriginalName = name;
        }

        /// <summary>
        /// mark the object as rented
        /// </summary>
        internal void MarkRented()
        {
            Debug.Assert(!m_IsRented, "renting a rented pool object should never happen");
            Debug.Assert(!IsDestroyed, "destroyed pool object should never rented out");

            m_IsRented = true;
            name = string.Format(m_RentedNameFormat, m_OriginalName);

            OnRented();
        }

        /// <summary>
        /// called when the object is rented out
        /// </summary>
        protected virtual void OnRented()
        {
            gameObject.SetActive(true);
        }

        protected virtual void Update()
        {
            Debug.Assert(m_IsRented || !m_IsPoolInstance, "returned pool object should stay inactive");
        }

        /// <summary>
        /// called when the object is returned to its pool
        /// won't be called if the object is destroyed
        /// </summary>
        protected virtual void OnReturned()
        {
            gameObject.SetActive(false);

            // avoid getting destroyed together with parent object
            transform.SetParent(null);
        }

        /// <summary>
        /// if the object is destroyed but still on rent,
        /// return it so the pool have a correct count of pool objects
        /// </summary>
        protected virtual void OnDestroy()
        {
            IsDestroyed = true;

            if(m_IsRented)
            {
                Debug.Log($"[{nameof(PoolObject)}] {GetType().Name} destroyed before return");
                Return();
            }
        }
    }
}