using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ForieroEditor.SceneManager
{
    public class SceneGraph : EditorWindow
    {
        static SceneGraph _window;        
        SceneGraphView _graphView;        
        SceneSettings _sceneSettings;
        ObjectField _sceneSettingsObjectField;

        public static SceneGraph window => _window ?? OpenSceneGraphWindow();

        [MenuItem("Foriero/Tools/Scene Graph")]
        public static SceneGraph OpenSceneGraphWindow()
        {
            _window = GetWindow<SceneGraph>();            
            _window.titleContent = new GUIContent("Scene Graph", "");
            return _window;
        }

        Rect lastPosition;

        private void OnGUI()
        {
            if(lastPosition != this.position)
            {
                lastPosition = this.position;
                _graphView?.UpdatePosition();
            }
        }

        public static void InitSceneGraphExternal(SceneSettings settings)
        {
            if (!_window) OpenSceneGraphWindow();
            window._sceneSettings = settings;
            window._sceneSettingsObjectField.value = settings as Object;            
        }

        public void InitSceneGraph(SceneSettings settings)
        {
            _sceneSettings = settings;
            _graphView.InitGraph(settings);
        }

        private void OnEnable()
        {
            this.SetAntiAliasing(4);
            ConstructGraphView();
            GenerateToolbar();            
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
            _sceneSettings = null;
        }

        private void OnDestroy()
        {
            _sceneSettings = null;
        }
               
        private void ConstructGraphView()
        {
            _graphView = new SceneGraphView(this) { name = "Scene Graph" };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }
                
        private void GenerateToolbar()
        {
            var toolbar = new Toolbar();
            
            _sceneSettingsObjectField = new ObjectField() { objectType = typeof(SceneSettings) };
            _sceneSettingsObjectField.RegisterValueChangedCallback(evt =>
            {
                _sceneSettings = evt.newValue as SceneSettings;
                InitSceneGraph(_sceneSettings);
            });
            _sceneSettingsObjectField.style.width = 300;            
            toolbar.Add(_sceneSettingsObjectField);

            var newButton = new Button(() => { }) { text = "+" };
            toolbar.Add(newButton);

            //var spacer0 = new ToolbarSpacer();
            //spacer0.flex = true;
            //toolbar.Add(spacer0);
            
            var clearGraphButton = new Button(() =>
            {
                if (!_sceneSettings) return;

                if (EditorUtilities.DisplayDialog("WARNING", "Do you really want to clear the scene graph?", "Yes", "No"))
                {
                    _sceneSettings.sceneItems = new List<SceneSettings.SceneItem>();
                    _graphView.ClearGraph();
                }
            })
            { text = "Clear" };            
            
            toolbar.Add(clearGraphButton);

            //var spacer1 = new ToolbarSpacer();
            //spacer1.flex = true;
            //toolbar.Add(spacer1);

            var asyncSceneButton = new Button(() => { _graphView?.ToggleSceneSettingsInspector(); }) { text = "Async Scene" };
            toolbar.Add(asyncSceneButton);

            var spacer2 = new ToolbarSpacer();
            spacer2.flex = true;
            toolbar.Add(spacer2);

            var serializeGraphButton = new Button(() =>
            {
                if (!_sceneSettings) return;
                _sceneSettings.Serialize();
                EditorUtility.SetDirty(_sceneSettings);
                window.ShowNotification(new GUIContent("Settings Serialized"), 2);
            })
            { text = "Serialize" };
            toolbar.Add(serializeGraphButton);

            var applyGraphButton = new Button(() =>
            {
                if (!_sceneSettings || _sceneSettings == SceneSettings.instance) return;
                if (EditorUtilities.DisplayDialog("WARNING", "Do you really want to apply it?", "Yes", "No"))
                {
                    SceneSettings.ApplyToBuildSettings(_sceneSettings);
                    window.ShowNotification(new GUIContent("Settings Applied"), 2);
                }
            })
            { text = "Apply" };
            toolbar.Add(applyGraphButton);

            rootVisualElement.Add(toolbar);
        }
    }
}
