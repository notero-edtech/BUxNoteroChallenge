using Notero.RaindropGameplay.UI;
using Notero.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Notero.RaindropGameplay.Core
{
    [System.Serializable]
    public class PrefabInfo
    {
        public string Name;
        public GameObject GamePrefab;
    }

    public class MidiGameplayConnector : MonoSingleton<MidiGameplayConnector>
    {
        [SerializeField]
        private List<PrefabInfo> m_MidiGameInfo;

        [SerializeField]
        private List<PrefabInfo> m_BackgroundInfo;

        private GameObject m_GameplayObject;
        int m_CurrentGameIndex = 0;
        const string DefaultName = "Notero";

        int m_CurrentBGIndex = 0;
        const string DefaultBGName = "Notero";

        public IMidiGameController CurrentGameController { get; private set; }
        public IBackgroundFeedbackChangable BackgroundFeedbackChangable { get; private set; }

        public void InstantiateGameplay()
        {
            CurrentGameController = null;
            BackgroundFeedbackChangable = null;

            InstantiateGameController();

            //Validate game controller
            if(CurrentGameController == null)
            {
                Debug.LogError($"GameObject {m_GameplayObject.name} must contain interface IMidiGameController.");
                return;
            }

            //Get IBackgroundFeedbackChangable from game controller
            if(!m_GameplayObject.TryGetComponent(out IBackgroundFeedbackChangable bgController))
            {
                bgController = m_GameplayObject.GetComponentInChildren<IBackgroundFeedbackChangable>();
            }
            if(bgController == null) return;
            BackgroundFeedbackChangable = bgController;

            InstantiateBackgroundManager();

        }

        private void InstantiateBackgroundManager()
        {
            if(m_BackgroundInfo[m_CurrentBGIndex].GamePrefab == null) return;

            var backgroundGameObject = Instantiate(m_BackgroundInfo[m_CurrentBGIndex].GamePrefab);

            //if not in parent, check children.
            if(!backgroundGameObject.TryGetComponent(out BaseBackgroundFeedbackManager baseBackgroundFeedback))
            {
                baseBackgroundFeedback = backgroundGameObject.GetComponentInChildren<BaseBackgroundFeedbackManager>();

                if(baseBackgroundFeedback == null)
                {
                    Debug.LogError($"GameObject {backgroundGameObject.name} must contain an inheritance from BaseBackgroundFeedbackManager.");
                    return;
                }
            }

            BackgroundFeedbackChangable?.SetBackgroundFeedback(baseBackgroundFeedback);
        }

        private void InstantiateGameController()
        {
            m_GameplayObject = Instantiate(m_MidiGameInfo[m_CurrentGameIndex].GamePrefab);

            if(m_GameplayObject.TryGetComponent(out IMidiGameController game))
            {
                CurrentGameController = game;
            }
            else
            {
                CurrentGameController = m_GameplayObject.GetComponentInChildren<IMidiGameController>();
            }
        }

        public void DestroyGame()
        {
            Destroy(m_GameplayObject);
            m_GameplayObject = null;
        }

        public void SelectGameController(string name = DefaultName)
        {
            var index = m_MidiGameInfo.FindIndex(x => x.Name == name);

            if(index == -1)
            {
                Debug.Log($"Game title: {name} is not found");
                return;
            }

            SelectGameController(index);
        }

        public void SelectGameController(int index)
        {
            if(index >= m_MidiGameInfo.Count)
            {
                Debug.LogError($"Index out of bound.");
                return;
            }

            m_CurrentGameIndex = index;
        }

        public void SelectBackgroundManager(string name = DefaultBGName)
        {
            var index = m_BackgroundInfo.FindIndex(x => x.Name == name);

            if(index == -1)
            {
                Debug.Log($"Background title: {name} is not found");
                return;
            }

            SelectBackgroundManager(index);
        }

        public void SelectBackgroundManager(int index)
        {
            if(index >= m_BackgroundInfo.Count)
            {
                Debug.LogError($"Index out of bound.");
                return;
            }

            m_CurrentBGIndex = index;
        }
    }
}