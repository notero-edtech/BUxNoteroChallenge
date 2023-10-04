using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ForieroEngine.Extensions;
using ForieroEngine.Settings;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace ForieroEditor.Settings
{
    public class SettingsManagerWindow : EditorWindow
    {
        static SettingsManagerWindow _window;              
        VisualElement root;        
        ScrollView settingsView;
        ScrollView inspectorView;
        InspectorElement inspector;

        // TOOLBAR //
        ToolbarButton captionButton;
        ToolbarButton applyButton;
        ToolbarSearchField searchField;

        SettingsAssets settings = new SettingsAssets();
        SettingsGuid inspectedSettingsGuid;

        [SerializeField]
        public string guid = "";

        private bool prefs = false;

        private IMGUIContainer playerPrefsIMGUI;
        private IMGUIContainer editorPRefsIMGUI;
        private IMGUIContainer persistentPathIMGUI;

        private int leftColumnWidth = 225;
        
        class SettingsAssets
        {
            public List<SettingsGuids> settingsGuids = new List<SettingsGuids>();

            public void RemoveSubButtons() { foreach (var settingsGuid in settingsGuids) settingsGuid.RemoveSubButtons(); }
            
            public void Sort()
            {
                settingsGuids.Sort((a, b) =>
                {
                    return a.settingsGuid.type.Name.CompareTo(b.settingsGuid.type.Name);
                });

                foreach (var guids in settingsGuids) guids.Sort();
            }
        }
                
        class SettingsGuids
        {
            public SettingsGuids(Type type)
            {
                this.settingsGuid = new SettingsGuid(type);
            }

            public SettingsGuid settingsGuid;
            public VisualElement container = new VisualElement();
            public VisualElement subContainer = new VisualElement();
            public List<SettingsGuid> settingsGuids = new List<SettingsGuid>();

            public void AddSubButtons() { container.Add(subContainer); }
            public void RemoveSubButtons() { if (container.Contains(subContainer)) container.Remove(subContainer); }
                        
            public void Sort()
            {
                settingsGuids.Sort((a, b) =>
                {
                    return a.type.Name.CompareTo(b.type.Name);
                });
            }
        }

        class SettingsGuid
        {
            public readonly string guid = "";
            public readonly Type type;
            public readonly bool main = true;
            public string GetAssetPath() => AssetDatabase.GUIDToAssetPath(guid);
            public string GetAssetPathFileNameWithoutExt() => Path.GetFileNameWithoutExtension(GetAssetPath());
            public string GetButtonName()
            {
                var result = Path.GetFileNameWithoutExtension(GetAssetPath());
                if (string.IsNullOrEmpty(result)) result = type.Name;
                //return result.Replace("Settings", " Settings");
                return result.Replace("Settings", "");
            }

            public Object GetObject()
            {
                var o = AssetDatabase.LoadAssetAtPath<Object>(GetAssetPath());
                if (!o)
                {
                    type.GetMethod("Select", BindingFlags.Public | BindingFlags.FlattenHierarchy| BindingFlags.Static)?.Invoke(null, null);
                    o = AssetDatabase.LoadAssetAtPath<Object>(GetAssetPath());
                }
                return o;
            }
            
            public SerializedObject GetSerializedObject() => new SerializedObject(GetObject());
            public SettingsGuids settingsGuids;            
            public SettingsGuid(Type type) { this.type = type; }
            public SettingsGuid(Type type, string guid, bool main) { this.type = type; this.guid = guid; this.main = main; }            
        }

        List<Type> types = new List<Type>();
                                
        [MenuItem("Foriero/Settings Manager", false, -1001)]
        static void Init()
        {
            _window = GetWindow<SettingsManagerWindow>();
            _window.titleContent = new GUIContent("Settings", "Settings");
        }

        void Reset()
        {
            root?.Clear();
            root = null;

            settingsView = null;

            inspectorView = null;
            inspector = null;
            inspectedSettingsGuid = null;

            captionButton = null;
            applyButton = null;
            searchField = null;
        }

        void Refresh()
        {
            Reset();

            settings = new SettingsAssets();

            types = TypeCache.GetTypesWithAttribute<SettingsManager>().ToList();
            //types = TypeCache.GetTypesDerivedFrom<SettingsManager>().Where(t => t is ISettingsProvider).ToList();

            foreach (var t in types)
            {
                var search = $"t:{t.FullName}";

                var guids = AssetDatabase.FindAssets(search);

                var settingsGuids = new SettingsGuids(t);

                foreach(var guid in guids)
                {
                    var p = AssetDatabase.GUIDToAssetPath(guid);
                    if (p.Contains($"Assets/Resources/Settings/{t.Name}.asset"))
                    {
                        var settingsGuid = new SettingsGuid(t, guid, true);
                        settingsGuid.settingsGuids = settingsGuids;
                        settingsGuids.settingsGuid = settingsGuid;
                    }
                    else
                    {
                        var settingsGuidChild = new SettingsGuid(t, guid, false);
                        settingsGuidChild.settingsGuids = settingsGuids;
                        settingsGuids.settingsGuids.Add(settingsGuidChild);
                    }
                }

                if (settingsGuids.settingsGuid == null)
                {
                    var guid = AssetDatabase.AssetPathToGUID($"Assets/Resources/Settings/{t.Name}.asset"); 
                    settingsGuids.settingsGuid = new SettingsGuid(t, guid, true);
                }
                                
                settings.settingsGuids.Add(settingsGuids);
            }

            settings.Sort();

            root = rootVisualElement;
            root.style.flexDirection = FlexDirection.Column;

            var toolbar = new Toolbar();
            {
                searchField = new ToolbarSearchField();
                searchField.style.minWidth = leftColumnWidth - 5;
                searchField.style.maxWidth = leftColumnWidth - 5;
                searchField.RegisterValueChangedCallback((s) =>
                {
                    foreach (var setting in settings.settingsGuids)
                    {
                        if (string.IsNullOrEmpty(s.newValue)) setting.container.SetEnabled(true);
                        else setting.container.SetEnabled(setting.settingsGuid.GetButtonName().Contains(s.newValue));
                    }
                });
                toolbar.Add(searchField);

                var refreshButton = new ToolbarButton( clickEvent: () => Refresh()) { text = "Refresh" };
                toolbar.Add(refreshButton);
                
                // var space0 = new ToolbarSpacer() { flex = true };
                // toolbar.Add(space0);
                var prefsButton = new Button(() =>
                {
                    PlayerPrefs.DeleteAll();
                    ForieroEngine.PlayerPrefs.DeleteAll();
                    if (EditorUtility.DisplayDialog(
                        "Clear Persistent Data Path",
                        $"Are you sure you wish to clear the persistent data path?\n This action cannot be reversed.\n{Application.persistentDataPath}",
                        "Clear", "Cancel")
                    ) {
                        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath);
                        foreach (FileInfo file in di.GetFiles()) file.Delete();
                        foreach (DirectoryInfo dir in di.GetDirectories()) dir.Delete(true);
                    }
                }) { text = "Clear Prefs" };
               
                toolbar.Add(prefsButton);

                var space1 = new ToolbarSpacer() { flex = true };
                toolbar.Add(space1);

                captionButton = new ToolbarButton(() => EditorGUIUtility.PingObject(inspectedSettingsGuid.GetObject()));
                toolbar.Add(captionButton);

                var space2 = new ToolbarSpacer() { flex = true };
                toolbar.Add(space2);

                applyButton = new ToolbarButton(
                    ()=> {                       
                        if(inspectedSettingsGuid != null)
                        {
                            if (!EditorUtility.DisplayDialog(
                                "Do you want to apply settings?",
                                $"{inspectedSettingsGuid.GetAssetPathFileNameWithoutExt()}->{inspectedSettingsGuid.settingsGuids.settingsGuid.GetAssetPathFileNameWithoutExt()}",
                                "Yes",
                                "No")
                            ) return;
                            
                            var so = inspectedSettingsGuid.GetObject();
                            var m = so.GetType().GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                            if(m != null) m.Invoke(so, null);
                        }
                    }) { text = "Apply" };
                applyButton.style.backgroundColor = Color.red.Brightness(0.3f);
                applyButton.SetEnabled(false);
                toolbar.Add(applyButton);
            }
            root.Add(toolbar);

            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.marginTop = 25;
            root.Add(container);
            container.StretchToParentSize();

            Button CreateButton(SettingsGuid settingsGuid)
            {
                void Bind(SettingsGuid item)
                {
                    this.Bind(item);
                    applyButton.SetEnabled(!item.main);                    
                    if (item.main)
                    {
                        settings?.RemoveSubButtons();
                        item.settingsGuids?.AddSubButtons();
                    }
                }

                var b = new Button(() => Bind(settingsGuid)) { text = settingsGuid.GetButtonName() };
                b.style.minHeight = 25;
                b.style.marginTop = b.style.marginBottom = b.style.marginLeft = b.style.marginRight = 0;                
                if (!settingsGuid.main) b.style.backgroundColor = Color.yellow.Brightness(0.3f).A(0.3f);
                return b;
            }

            settingsView = new ScrollView(ScrollViewMode.Vertical);
            {
                settingsView.style.maxWidth = leftColumnWidth;
                settingsView.style.minWidth = leftColumnWidth;
                                
                foreach(var settingsGuids in settings.settingsGuids)
                {
                    settingsGuids.container.style.flexGrow = 1f;
                    settingsGuids.container.Add(CreateButton(settingsGuids.settingsGuid));
                    foreach (var settingsGuid in settingsGuids.settingsGuids)
                    {
                        settingsGuids.subContainer.Add(CreateButton(settingsGuid));
                    }
                    settingsView.contentContainer.Add(settingsGuids.container);
                }               
            }
            container.Add(settingsView);
            
            inspectorView = new ScrollView(ScrollViewMode.Vertical);
            inspectorView.style.flexGrow = 1f;
            container.Add(inspectorView);

            foreach (var settingsGuids in settings.settingsGuids)
            {
                foreach (var settingsGuid in settingsGuids.settingsGuids)
                {
                    if (guid == settingsGuid.guid)
                    {
                        Bind(settingsGuid);
                        applyButton.SetEnabled(!settingsGuid.main);
                    }
                }
            }

            root.MarkDirtyRepaint();
        }

        private void OnEnable()
        {
            Refresh();      
        }

        void Bind(SettingsGuid settingsGuid)
        {
            //Debug.Log($"Settings | {settingsGuid.type.Name} : {settingsGuid.guid} : {settingsGuid.GetAssetPath()}");

            inspectedSettingsGuid = settingsGuid;

            guid = inspectedSettingsGuid.guid;
            
            var o = settingsGuid.GetObject();
            
            captionButton.text = o?.name;
            captionButton.MarkDirtyRepaint();

            if (inspector != null)
            {
                inspector.Unbind();
                inspector.Clear();
                inspectorView.Remove(inspector);
            }
                        
            inspector = new InspectorElement();
            inspectorView.contentContainer.Add(inspector);
           
            if (o) inspector.Bind(settingsGuid.GetSerializedObject());
                               
            root.MarkDirtyRepaint();
            Repaint();           
        }             
        
        private void OnDisable()
        {
            if (inspector.IsBound()) inspector.Unbind();
        }
    }
}
