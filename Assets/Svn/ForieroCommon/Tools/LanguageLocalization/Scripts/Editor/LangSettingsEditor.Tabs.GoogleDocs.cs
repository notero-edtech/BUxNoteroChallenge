using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;

public partial class LangSettingsEditor : Editor
{
    void InitGoogleDocsList()
    {
        googleDocsList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "GoogleDocs (File Name, Prepend File Name, .., Public Key)");
        };

        googleDocsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = googleDocsList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            float tw = 0; float w = 200;
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, w, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("fileName"), GUIContent.none);

            tw += w;w = 17;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("prependFileName"), GUIContent.none);

            tw += w;w = 50f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "Google"))
            {
                Application.OpenURL(o.googleDocs[index].googleURL);
            }

            tw += w; w = 70f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y,  w, EditorGUIUtility.singleLineHeight), "Download"))
            {
                if (index >= 0 && index < o.googleDocs.Count)
                {
                    LangSettings.GoogleDoc googleDoc = o.googleDocs[index];
                    //EditorCoroutineStart.StartCoroutine(DownloadOds(googleDoc));
                    DownloadXlsx(googleDoc);
                }
            }

            tw += w; w = 40f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "XLSX")){
                if (index >= 0 && index < o.googleDocs.Count)
                {
                    LangSettings.GoogleDoc googleDoc = o.googleDocs[index];
                    EditorUtility.OpenWithDefaultApp(googleDoc.odsAssetsPath.GetFullPathFromAssetPath());
                }
            }

            tw += w; w = 80f;
            if (GUI.Button(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), "XLSX->CSV"))
            {
                if (index >= 0 && index < o.googleDocs.Count)
                {
                    LangSettings.GoogleDoc googleDoc = o.googleDocs[index];
                    XLSXToDictionary(googleDoc);
                    AssetDatabase.Refresh();
                }
            }

            tw += w; w = rect.width - tw;
            EditorGUI.PropertyField(new Rect(rect.x + tw, rect.y, w, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("publicKey"), GUIContent.none);
        };
    }
}
