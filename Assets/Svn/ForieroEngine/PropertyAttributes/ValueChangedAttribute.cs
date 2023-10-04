using System.Linq;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ValueChangedAttribute : PropertyAttribute
{
    public string methodName;
    public ValueChangedAttribute(string methodNameNoArguments) { methodName = methodNameNoArguments; }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ValueChangedAttribute))]
public class ValueChangedAttributePropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property);
        if(EditorGUI.EndChangeCheck())
        {
            ValueChangedAttribute at = attribute as ValueChangedAttribute;
            MethodInfo method = property.serializedObject.targetObject.GetType().GetMethods().Where(m => m.Name == at.methodName).First();

            if (method != null && method.GetParameters().Count() == 0)// Only instantiate methods with 0 parameters
                method.Invoke(property.serializedObject.targetObject, null);
        }
    }
}
#endif