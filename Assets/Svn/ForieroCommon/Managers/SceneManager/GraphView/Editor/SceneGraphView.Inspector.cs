using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static SceneSettings;

namespace ForieroEditor.SceneManager
{
    public class InspectorElement : GraphElement { };

    public partial class SceneGraphView : GraphView
    {
        public SceneItem selectedItem = null;
        InspectorElement inspector = null;
        InspectorElement inspectorSettings = null;

        private void AddSceneInspector()
        {                        
            inspector = new InspectorElement();        
            inspector.style.backgroundColor = Color.black.A(0.3f);
            IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { DrawSelectedScene(); });            
            inspector.Add(inspectorIMGUI);
            inspectorIMGUI.StretchToParentSize();            
            Add(inspector);

            UpdateInspectorPosition();
        }

        void UpdateInspectorPosition() => inspector?.SetPosition(new Rect(_window.position.width - 410, _window.position.height - 310, 400, 300));

        public void ToggleSceneSettingsInspector()
        {
            inspectorSettings.visible = !inspectorSettings.visible;
        }

        private void AddSceneSettingsInspector()
        {
            inspectorSettings = new InspectorElement();
            inspectorSettings.style.backgroundColor = Color.black.A(0.3f);
            IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { DrawSettings(); });
            inspectorSettings.Add(inspectorIMGUI);
            inspectorIMGUI.StretchToParentSize();
            Add(inspectorSettings);
            inspectorSettings.visible = false;

            UpdateSettingsInspectorPosition();
        }

        void UpdateSettingsInspectorPosition() => inspectorSettings?.SetPosition(new Rect(330, 25, 400, 240));
              
        void DrawSettings()
        {
            if (_settings.IsNull()) return;
            
            GUILayout.Box("Loading Async Scene", GUILayout.ExpandWidth(true));

            _settings.loadingAsyncScene.asFirstScene = EditorGUILayout.Toggle("As first scene", _settings.loadingAsyncScene.asFirstScene);
            //EditorGUILayout.PropertyField(spLoadingAsyncScene.FindPropertyRelative("asFirstScene"));
            _settings.loadingAsyncScene.sceneAsset = EditorGUILayout.ObjectField("Scene", _settings.loadingAsyncScene.sceneAsset, typeof(SceneAsset), false) as SceneAsset;
            //EditorGUILayout.PropertyField(spLoadingAsyncScene.FindPropertyRelative("sceneAsset"));

            DrawTransition(_settings.loadingAsyncScene.transition);
        }

        void DrawSelectedScene()
        {
            if (selectedItem == null) return;
            EditorGUI.BeginDisabledGroup(true); EditorGUILayout.TextField("Node GUID", selectedItem.internalGUID); EditorGUI.EndDisabledGroup();
            selectedItem.sceneProxyName = EditorGUILayout.TextField("Proxy Name", selectedItem.sceneProxyName);
            //EditorGUILayout.PropertyField(selectedScene.FindPropertyRelative("sceneProxyName"));
            selectedItem.sceneAsset = EditorGUILayout.ObjectField("Scene", selectedItem.sceneAsset, typeof(SceneAsset), false) as SceneAsset;
            //EditorGUILayout.PropertyField(selectedScene.FindPropertyRelative("sceneAsset"));
            selectedItem.sceneAssetPath = EditorGUILayout.TextField("Asset Path", selectedItem.sceneAssetPath);
            //EditorGUILayout.PropertyField(selectedScene.FindPropertyRelative("sceneAssetPath"));

            selectedItem.asynchronously = EditorGUILayout.Toggle("Async", selectedItem.asynchronously);
            //EditorGUILayout.PropertyField(selectedScene.FindPropertyRelative("asynchronously"));
            selectedItem.asyncLoadingScene = EditorGUILayout.Toggle("Async with Loading Scene", selectedItem.asyncLoadingScene);
            //EditorGUILayout.PropertyField(selectedScene.FindPropertyRelative("asyncLoadingScene"));

            DrawTransition(selectedItem.transition);

        }

        void DrawTransition(SceneSettings.SceneTransition transition)
        {
            transition.fadeIn = EditorGUILayout.Toggle("Fade In", transition.fadeIn);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeIn"));
            transition.fadeInTime = EditorGUILayout.FloatField("Fade In Time", transition.fadeInTime);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeInTime"));
            transition.fadeInColor = EditorGUILayout.ColorField("Fade In Color", transition.fadeInColor);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeInColor"));
            transition.fadeInSound = EditorGUILayout.TextField("Fade In Sound", transition.fadeInSound);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeInSound"));

            transition.fadeOut = EditorGUILayout.Toggle("Fade Out", transition.fadeOut);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeOut"));
            transition.fadeOutTime = EditorGUILayout.FloatField("Fade Out Time", transition.fadeOutTime);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeOutTime"));
            transition.fadeOutColor = EditorGUILayout.ColorField("Fade Out Color", transition.fadeOutColor);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeOutColor"));
            transition.fadeOutSound = EditorGUILayout.TextField("Fade Out Sound", transition.fadeOutSound);
            //EditorGUILayout.PropertyField(sp.FindPropertyRelative("fadeOutSound"));
        }       
    }
}
