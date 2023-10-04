using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;
using static SceneSettings;

namespace ForieroEditor.SceneManager
{
    public class ReorderableListElement : GraphElement { };

    public partial class SceneGraphView : GraphView
    {
        ReorderableList reorderableList = null;
        ReorderableListElement list = null;

        private void CreateList()
        {
            reorderableList = GetReorderableNodeList();
            list = new ReorderableListElement();            
            IMGUIContainer listIMGUI = new IMGUIContainer(() => { reorderableList?.DoLayoutList(); });
            list.Add(listIMGUI);
            list.StretchToParentSize();            
            Add(list);
            
            UpdateListPosition();
        }

        void UpdateListPosition() => list?.SetPosition(new Rect(_window.position.width - 410, 25, 400, 300));

        ReorderableList GetReorderableNodeList()
        {
            ReorderableList rl = null;

            if (_settings.IsNull() || _settings.sceneItems.IsNull()) return null;

            rl = new ReorderableList(_settings.sceneItems, typeof(SceneItem), true, true, true, true)
            {
                drawHeaderCallback = (rect) =>
                {
                    EditorGUI.LabelField(rect, "ProxyName, Scene Asset, Scene Path");
                },

                onAddCallback = (list) =>
                {
                    var sceneItem = new SceneItem();
                    _settings.sceneItems.Add(sceneItem);

                    sceneItem.sceneAsset = null;
                    sceneItem.internalGUID = GUID.Generate().ToString();
                    sceneItem.sceneProxyName = "";
                    sceneItem.sceneName = "";
                    sceneItem.asynchronously = true;

                    var transition = sceneItem.transition;

                    transition.fadeInTime = 0.3f;
                    transition.fadeInColor = Color.black;
                    transition.fadeOutTime = 0.3f;
                    transition.fadeOutColor = Color.black;

                    //may be init//                
                },

                onRemoveCallback = (list) =>
                {
                    var node = rl.list[rl.index] as SceneItem;
                    rl.list.RemoveAt(rl.index);

                    //may be init// 
                },

                onSelectCallback = (list) =>
                {
                    if (rl.index >= 0 && rl.index < list.count)
                    {
                        selectedItem = list.list[rl.index] as SceneItem;
                    }
                    else
                    {
                        selectedItem = null;
                    }
                },

                onReorderCallback = (list) =>
                {
                    EditorUtility.SetDirty(_settings);
                },

                drawElementCallback = (rect, index, isActive, isFocused) =>
                {
                    var color = GUI.backgroundColor;
                    var sceneItem = (rl.list[index] as SceneItem);
                    if (!sceneItem.sceneAsset) GUI.backgroundColor = Color.red;

                    EditorGUI.BeginChangeCheck();
                    {
                        rect.y += 2;
                        rect.height = EditorGUIUtility.singleLineHeight;

                        float tw = 0;

                        float width = rect.width / 5f;

                        sceneItem.sceneProxyName = EditorGUI.TextField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), GUIContent.none, sceneItem.sceneProxyName);
                        tw += width;

                        sceneItem.sceneAsset = EditorGUI.ObjectField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), sceneItem.sceneAsset, typeof(SceneAsset), false) as SceneAsset;
                        tw += width;

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.TextField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), GUIContent.none, sceneItem.sceneName);
                        tw += width;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.TextField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), GUIContent.none, sceneItem.sceneAssetPath);
                        tw += width;
                        EditorGUI.EndDisabledGroup();

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUI.TextField(new Rect(rect.x + tw, rect.y, width, EditorGUIUtility.singleLineHeight), GUIContent.none, sceneItem.sceneAssetGUID);
                        tw += width;
                        EditorGUI.EndDisabledGroup();

                        GUI.backgroundColor = color;
                    }
                    if (EditorGUI.EndChangeCheck() || GUI.changed)
                    {
                        sceneItem.Serialize();
                        EditorUtility.SetDirty(_settings);

                        //may be repait//
                    }
                }
            };

            return rl;
        }
    }
}
