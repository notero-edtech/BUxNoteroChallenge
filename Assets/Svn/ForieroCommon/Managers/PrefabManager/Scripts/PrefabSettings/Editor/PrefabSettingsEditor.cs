using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;

[CustomEditor(typeof(PrefabSettings))]
public class PrefabSettingssEditor : Editor
{
    ReorderableList categoryList;
    ReorderableList prefabList;

    protected SerializedProperty m_Script;
    protected SerializedProperty categories;
    protected SerializedProperty prefabs;

    SerializedObject bankSO;

    protected virtual void OnEnable()
    {
        // soundSettings = target as SoundSettings;

        m_Script = serializedObject.FindProperty("m_Script");
        categories = serializedObject.FindProperty("categories");

        categoryList = new ReorderableList(serializedObject, categories, true, true, true, true);

        categoryList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Prefab Categories");
        };

        categoryList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = categoryList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float width = rect.width / 2f;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("id"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + width, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("bank"), GUIContent.none);
        };

        categoryList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
                prefabList = null;
            }
        };

        categoryList.onChangedCallback = (ReorderableList l) =>
        {

        };

        categoryList.onReorderCallback = (ReorderableList l) =>
        {

        };

        categoryList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("id").stringValue = "";
            l.serializedProperty.GetArrayElementAtIndex(l.count - 1).FindPropertyRelative("bank").objectReferenceValue = null;
        };

        categoryList.onSelectCallback = (ReorderableList l) =>
        {
            SetPrefabList();
        };
    }

    void SetPrefabList()
    {
        if (categoryList.index >= 0 && categoryList.index < categoryList.count)
        {
            switch ((PM.PrefabCategory.Tab)categoryList.serializedProperty.GetArrayElementAtIndex(categoryList.index).FindPropertyRelative("tab").enumValueIndex)
            {
                case PM.PrefabCategory.Tab.Self:
                    FillSelectedList(categoryList.serializedProperty.GetArrayElementAtIndex(categoryList.index).FindPropertyRelative("items"));
                    bankSO = null;
                    break;
                case PM.PrefabCategory.Tab.Bank:
                    var bank = categoryList.serializedProperty.GetArrayElementAtIndex(categoryList.index).FindPropertyRelative("bank").objectReferenceValue;
                    if (bank)
                    {
                        bankSO = new SerializedObject(bank);
                        FillSelectedList(bankSO.FindProperty("items"));
                    }
                    else
                    {
                        bankSO = null;
                        prefabList = null;
                    }
                    break;
            }
        }
        else
        {
            prefabList = null;
            bankSO = null;
        }
    }

    void FillSelectedList(SerializedProperty p)
    {
        prefabList = new ReorderableList(serializedObject, p, true, true, true, true);

        prefabList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "PREFABs");
        };

        prefabList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = prefabList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            float width = rect.width / 2f;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("id"), GUIContent.none);
            EditorGUI.PropertyField(new Rect(rect.x + width, rect.y, width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("prefab"), GUIContent.none);
        };

        prefabList.onRemoveCallback = (ReorderableList l) =>
        {
            if (l.index >= 0 && l.index < l.count)
            {
                l.serializedProperty.DeleteArrayElementAtIndex(l.index);
            }
        };

        prefabList.onChangedCallback = (ReorderableList l) =>
        {

        };

        prefabList.onReorderCallback = (ReorderableList l) =>
        {

        };

        prefabList.onAddCallback = (ReorderableList l) =>
        {
            l.serializedProperty.InsertArrayElementAtIndex(l.count);
        };

        prefabList.onSelectCallback = (ReorderableList l) =>
        {

        };
    }

    Color backgroundColor;

    public override void OnInspectorGUI()
    {
        backgroundColor = GUI.backgroundColor;
        EditorGUI.BeginChangeCheck();
        {
            categoryList.DoLayoutList();

            DrawCategoryTabs();

            if (prefabList != null)
            {
                prefabList.DoLayoutList();
            }
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            if (bankSO != null)
            {
                bankSO.ApplyModifiedProperties();
            }
        }
    }

    void DrawCategoryTabs()
    {
        if (categoryList.index >= 0 && categoryList.index < categoryList.count)
        {
            var tab = categoryList.serializedProperty.GetArrayElementAtIndex(categoryList.index).FindPropertyRelative("tab");

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = (tab.enumValueIndex == (int)PM.PrefabCategory.Tab.Self ? Color.green : backgroundColor);
            if (GUILayout.Button("Self"))
            {
                tab.enumValueIndex = (int)PM.PrefabCategory.Tab.Self;
                SetPrefabList();
            }
            GUI.backgroundColor = (tab.enumValueIndex == (int)PM.PrefabCategory.Tab.Bank ? Color.green : backgroundColor);
            if (GUILayout.Button("Bank"))
            {
                tab.enumValueIndex = (int)PM.PrefabCategory.Tab.Bank;
                SetPrefabList();
            }
            GUI.backgroundColor = backgroundColor;
            EditorGUILayout.EndHorizontal();
        }
    }
}
