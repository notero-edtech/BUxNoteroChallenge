using Notero.Utilities.Math;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Hendrix.Generic.UI.Elements
{
    public class SmoothProgressBar : Slider
    {
        [SerializeField]
        private bool m_IsSmooth = true;

        [SerializeField]
        private float m_Speed = 0.3f;

        private float m_TargetValue;
        private float m_Velocity;
        private float m_LowestAnchorMaxXValue;
        private const float m_Precision = 0.1f;

        public override float value
        {
            get => base.value;
            set
            {
                if(!Mathf.Approximately(value, m_TargetValue))
                {
                    m_TargetValue = value;

                    if(!m_IsSmooth)
                    {
                        base.value = value;
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            if(Application.isPlaying)
            {
                if(!FloatComparator.IsEquals(base.value, m_TargetValue, m_Precision))
                {
                    base.value = Mathf.SmoothDamp(base.value, m_TargetValue, ref m_Velocity, m_Speed);

                    if(FloatComparator.IsEquals(base.value, m_TargetValue, m_Precision))
                    {
                        base.value = m_TargetValue;
                    }
                }
            }

            if(base.value == 0)
            {
                fillRect.anchorMax = Vector2.up;
            }
            else if(fillRect.anchorMax.x < m_LowestAnchorMaxXValue)
            {
                fillRect.anchorMax = new Vector2(m_LowestAnchorMaxXValue, 1);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_LowestAnchorMaxXValue = CalculateLowestAnchorMaxXValue();
        }

        private float CalculateLowestAnchorMaxXValue()
        {
            Image fillImage = fillRect.GetComponent<Image>();
            float spriteWidth = fillImage.sprite.rect.width;
            float progressWidth = ((RectTransform)fillRect.parent).rect.width;

            return spriteWidth / progressWidth;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SmoothProgressBar))]
    public class SmoothProgressBarEditor : UnityEditor.UI.SliderEditor
    {
        private SerializedProperty m_IsSmoothField;
        private SerializedProperty m_SpeedField;

        protected override void OnEnable()
        {
            base.OnEnable();
            m_IsSmoothField = serializedObject.FindProperty("m_IsSmooth");
            m_SpeedField = serializedObject.FindProperty("m_Speed");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_IsSmoothField);
            EditorGUILayout.PropertyField(m_SpeedField);
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}