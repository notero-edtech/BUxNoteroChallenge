using UnityEngine;

namespace Notero.Utilities.Pooling.Samples
{
    public class ObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private SamplePoolObject m_Prefab = default;

        [SerializeField]
        private Vector3 m_SpawnPosition = default;

        [SerializeField]
        private float m_RandomRadius = default;

        public void Spawn()
        {
            Vector3 position = m_SpawnPosition + Random.insideUnitSphere * m_RandomRadius;
            m_Prefab.Rent(position, Random.rotation, null);
        }
    }
}