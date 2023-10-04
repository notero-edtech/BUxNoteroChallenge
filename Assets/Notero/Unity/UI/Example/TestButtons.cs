using UnityEngine;
using UnityEngine.Serialization;

namespace Notero.Unity.UI.Example
{
    public class TestButtons : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("m_Button")]
        private Button[] m_SingleVariantButtons;

        [SerializeField]
        [FormerlySerializedAs("m_TransformableButton")]
        private TransformableButton[] m_MultiVariantButtons;

        private void Awake()
        {
            foreach(var button in m_SingleVariantButtons)
            {
                button.onClick.AddListener(() => DebugLogButtonClicked(button));
            }

            foreach(var button in m_MultiVariantButtons)
            {
                button.onClick.AddListener(() => DebugLogButtonClicked(button));
            }
        }

        [ContextMenu("Enable Interaction")]
        private void EnableInteration()
        {
            foreach(var button in m_SingleVariantButtons)
            {
                button.interactable = true;
            }

            foreach(var button in m_MultiVariantButtons)
            {
                button.interactable = true;
            }
        }

        [ContextMenu("Disable Interaction")]
        private void DisableInteraction()
        {
            foreach(var button in m_SingleVariantButtons)
            {
                button.interactable = false;
            }

            foreach(var button in m_MultiVariantButtons)
            {
                button.interactable = false;
            }
        }

        [ContextMenu("Set Variant 0")]
        private void SetVariant0()
        {
            foreach(var button in m_MultiVariantButtons)
            {
                button.CurrentVariant = 0;
            }
        }

        [ContextMenu("Set Variant 1")]
        private void SetVariant1()
        {
            foreach(var button in m_MultiVariantButtons)
            {
                button.CurrentVariant = 1;
            }
        }

        private void DebugLogButtonClicked(Button button)
        {
            Debug.Log($"[TestButtons] Click button {button.name}");
        }
    }
}
