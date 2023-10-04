using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEditor.Android;

public class MIDIUnifiedPostprocessBuildPlayerAndroid : IPostGenerateGradleAndroidProject
{
    public int callbackOrder => 0;
    public void OnPostGenerateGradleAndroidProject(string path)
    {
        var androidManifest = new AndroidManifest(GetManifestPath(path));
        androidManifest.AddMidiPermissionsAndFeatures();
        androidManifest.Save();
    }
    
    private string _manifestFilePath;

    private string GetManifestPath(string basePath)
    {
        if (string.IsNullOrEmpty(_manifestFilePath))
        {
            var pathBuilder = new StringBuilder(basePath);
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("src");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("main");
            pathBuilder.Append(Path.DirectorySeparatorChar).Append("AndroidManifest.xml");
            _manifestFilePath = pathBuilder.ToString();
        }
        return _manifestFilePath;
    }

    private class AndroidXmlDocument : XmlDocument
    {
        private string _mPath;
        protected XmlNamespaceManager nsMgr;
        public readonly string AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
        public AndroidXmlDocument(string path)
        {
            _mPath = path;
            using (var reader = new XmlTextReader(_mPath)) { reader.Read(); Load(reader); }
            nsMgr = new XmlNamespaceManager(NameTable);
            nsMgr.AddNamespace("android", AndroidXmlNamespace);
        }
        public string Save() => SaveAs(_mPath);
        public string SaveAs(string path)
        {
            using var writer = new XmlTextWriter(path, new UTF8Encoding(false));
            writer.Formatting = Formatting.Indented;
            Save(writer);
            return path;
        }
    }

    private class AndroidManifest : AndroidXmlDocument
    {
        private readonly XmlElement ApplicationElement;
        public AndroidManifest(string path) : base(path) => ApplicationElement = SelectSingleNode("/manifest/application") as XmlElement;
        
        private XmlAttribute CreateAndroidAttribute(string key, string value)
        {
            var attr = CreateAttribute("android", key, AndroidXmlNamespace);
            attr.Value = value;
            return attr;
        }

        private void AddPermission(string permission, List<(string name, string value)> otherAttributes = null)
        {
            var manifest = SelectSingleNode("/manifest");
            
            var p = $"android.permission.{permission}";
            
            XmlElement child = null;
            foreach (XmlElement e in manifest)
            {
                if(e.Name == "uses-permission" && e.HasAttribute("android:name"))
                    if (e.Attributes["android:name"].Value == p) { child = e; break; }
            }
            
            if (child == null)
            {
                child = CreateElement("uses-permission");
                manifest.AppendChild(child);
                var name = CreateAndroidAttribute("name", $"android.permission.{permission}");
                child.Attributes.Append(name);
            }

            otherAttributes?.ForEach(kv =>
            {
                var a = CreateAndroidAttribute(kv.name, kv.value);
                child.Attributes.Append(a);
            });
        }
        
        private void AddFeature(string feature, List<(string name, string value)> otherAttributes = null)
        {
            var manifest = SelectSingleNode("/manifest");
            
            XmlElement child = null;
            foreach (XmlElement e in manifest)
            {
                if(e.Name == "uses-feature" && e.HasAttribute("android:name"))
                    if (e.Attributes["android:name"].Value == feature) { child = e; break; }
            }
            
            if (child == null)
            {
                child = CreateElement("uses-feature");
                manifest.AppendChild(child);
                var nameAttribute = CreateAndroidAttribute("name", $"{feature}");
                child.Attributes.Append(nameAttribute);                
            }

            otherAttributes?.ForEach(kv =>
            {
                var a = CreateAndroidAttribute(kv.name, kv.value);
                child.Attributes.Append(a);
            });
        }

        internal XmlNode GetActivityWithLaunchIntent()
        {
            return SelectSingleNode("/manifest/application/activity[intent-filter/action/@android:name='android.intent.action.MAIN' and " +
                                    "intent-filter/category/@android:name='android.intent.category.LAUNCHER']", nsMgr);
        }

        internal void SetApplicationTheme(string appTheme) => ApplicationElement.Attributes.Append(CreateAndroidAttribute("theme", appTheme));
        internal void SetStartingActivityName(string activityName) => GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("name", activityName));
        internal void SetHardwareAcceleration() => GetActivityWithLaunchIntent().Attributes.Append(CreateAndroidAttribute("hardwareAccelerated", "true"));
        
        internal void AddMidiPermissionsAndFeatures()
        {
            var sdk = PlayerSettings.Android.targetSdkVersion;
            
            AddPermission("RECORD_AUDIO");
            
            var requiredFalse = new List<(string name, string value)>();
            requiredFalse.Add(new ("required", "false"));
            
            var requiredTrue = new List<(string name, string value)>();
            requiredTrue.Add(new ("required", "true"));
            
            AddFeature("android.software.midi", requiredFalse);
            AddFeature("android.hardware.bluetooth", requiredFalse);
            AddFeature("android.hardware.bluetooth_le", requiredFalse);
            
            if ((int)sdk > (int)AndroidSdkVersions.AndroidApiLevel30)
            {
                // *** API Target Level 12> *** //
                var maxSdkVersion = new List<(string name, string value)>();
                maxSdkVersion.Add(new ("maxSdkVersion", "30"));
                //AddPermission("BLUETOOTH", maxSdkVersion);
                //AddPermission("BLUETOOTH_ADMIN", maxSdkVersion);
                
                AddPermission("BLUETOOTH");
                AddPermission("BLUETOOTH_ADMIN");
                
                AddPermission("BLUETOOTH_CONNECT");
                
                var userPermissionFlags = new List<(string name, string value)>();
                userPermissionFlags.Add(new ("usesPermissionFlags", "neverForLocation"));
                AddPermission("BLUETOOTH_SCAN", userPermissionFlags);
            }
            else
            {
                AddPermission("BLUETOOTH");
                AddPermission("BLUETOOTH_ADMIN");
                AddPermission("ACCESS_COARSE_LOCATION");
                if ((int)sdk >= (int)AndroidSdkVersions.AndroidApiLevel28)
                {
                    AddPermission("ACCESS_BACKGROUND_LOCATION");
                }
            }
            
            AddPermission("BLUETOOTH_PRIVILEGED");
        }
    }
}