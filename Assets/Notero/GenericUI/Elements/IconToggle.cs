#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Hendrix.Generic.UI.Elements
{
    public class IconToggle : Toggle
    {
        [SerializeField]
        private GameObject m_PositiveToggleGO;

        [SerializeField]
        private GameObject m_NegativeToggleGO;

        protected override void Awake()
        {
            base.Awake();
            transition = Transition.None;
            onValueChanged.AddListener(OnToggleValueChanged);
        }

        public virtual void OnToggleValueChanged(bool isOn)
        {
            if (m_PositiveToggleGO != null)
            {
                m_PositiveToggleGO.SetActive(isOn);
            }
            if (m_NegativeToggleGO != null)
            {
                m_NegativeToggleGO.SetActive(!isOn);
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(IconToggle))]
    public class IconToggleScriptEditor : UnityEditor.UI.ToggleEditor
    {
        private SerializedProperty m_PositiveToggleGO;
        private SerializedProperty m_NegativeToggleGO;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_PositiveToggleGO = serializedObject.FindProperty("m_PositiveToggleGO");
            m_NegativeToggleGO = serializedObject.FindProperty("m_NegativeToggleGO");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_PositiveToggleGO);
            EditorGUILayout.PropertyField(m_NegativeToggleGO);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}