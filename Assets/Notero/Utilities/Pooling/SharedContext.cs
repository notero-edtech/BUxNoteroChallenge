using System;
using System.Collections.Generic;
using UnityEngine;

namespace Notero.Utilities.Pooling
{
    /// <summary>
    /// holding references to shared objects
    /// </summary>
    public class SharedContext : MonoSingleton<SharedContext>
    {
        private readonly Dictionary<Type, object> m_Objects = new Dictionary<Type, object>();

        public SharedContext() { }

        public void Add<T>(T obj) where T : class
        {
            Type key = typeof(T);

            if(!m_Objects.ContainsKey(key))
            {
                m_Objects.Add(key, obj);
            }
            else
            {
                Debug.LogError($"Object of same key detected: {key.AssemblyQualifiedName}");
            }
        }

        public void Remove<T>(T obj) where T : class
        {
            Remove<T>();
        }

        public void Remove<T>() where T : class
        {
            m_Objects.Remove(typeof(T));
        }

        public T Get<T>() where T : class
        {
            m_Objects.TryGetValue(typeof(T), out object result);

            return result as T;
        }

        public bool TryGet<T>(out T resultT) where T : class
        {
            bool result = m_Objects.TryGetValue(typeof(T), out object resultObj);
            resultT = resultObj as T;

            return result;
        }
    }
}