using UnityEngine;

namespace Notero.Unity.Networking.Mirror
{
    public class NetworkManagerContainer
    {
        public const string NetworkManagerName = "Mirror.NetworkManager";

        public MirrorNetworkManager NetworkManager
        {
            get
            {
                Init();
                return m_NetworkManager;
            }
        }
        private MirrorNetworkManager m_NetworkManager;

        public void Init()
        {
            if(m_NetworkManager == null)
            {
                m_NetworkManager = Object.FindObjectOfType<MirrorNetworkManager>();

                if(m_NetworkManager != null)
                    return;

                var prefab = Resources.Load(NetworkManagerName);
                var gameObject = Object.Instantiate(prefab) as GameObject;
                gameObject.name = NetworkManagerName;
                Object.DontDestroyOnLoad(gameObject);
                m_NetworkManager = gameObject.GetComponent<MirrorNetworkManager>();
            }
        }

        public void Destroy()
        {
            Object.Destroy(m_NetworkManager.gameObject);
        }
    }
}
