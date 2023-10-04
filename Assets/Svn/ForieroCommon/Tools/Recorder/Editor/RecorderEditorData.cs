using System;
using System.Collections.Generic;
using System.IO;
using ForieroEditor.CommandLine;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEditor.Recorder;
using UnityEngine;
using static UnityEditor.Recorder.ImageRecorderSettings;

public class RecorderEditorData : ScriptableObject
{
    static readonly string dataPath = "Assets/Editor/Recorder/RecorderEditorData.asset";
    static RecorderEditorData _data = null;

    public static RecorderEditorData data
    {
        get
        {
            if (_data == null) InitData();
            return _data;
        }
    }

    static void InitData()
    {
        _data = AssetDatabase.LoadAssetAtPath<RecorderEditorData>(dataPath);

        var p = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Editor/Recorder/Settings");
        if (!Directory.Exists(p)) Directory.CreateDirectory(p);

        if (_data == null)
        {
            _data = CreateInstance<RecorderEditorData>();
            AssetDatabase.CreateAsset(data, dataPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            _data = AssetDatabase.LoadAssetAtPath<RecorderEditorData>(dataPath);
        }
    }

    public enum RecordPathOptions
    {
        Cloud,
        Project,
        Absolute
    }

    public enum RecordingOptions
    {
        Movie,
        Image,
        Audio,
        Undefined = int.MaxValue
    }

    public enum Wildcards
    {
        Recorder,
        Time,
        Take,
        Date,
        Project,
        Product,
        Scene,
        Resolution,
        Frame,
        Extension
    }

    [Serializable]
    public class RecordGroup
    {
        public bool included = true;
        public string name = "";
        public RecordPathOptions recordPathOptions = RecordPathOptions.Cloud;
        public string path = "";
        public RecorderControllerSettings recorderControllerSettings;
        public RecordingOptions recordingOptions = RecordingOptions.Movie;
        public MovieRecorderSettings movieRecorderSettings;
        public ImageRecorderSettings imageRecorderSettings;
        public RecorderAudioSettings audioRecorderSettings;
        public List<RecordItem> items = new List<RecordItem>();

        public void ClearRecordFlags()
        {
            foreach (var i in items) i.record = false;
        }

        public void ClearRecordingFlags()
        {
            foreach (var i in items) i.recording = false;
        }

        public void FlagIncluded()
        {
            foreach (var i in items) if (i.included) i.record = true;
        }

        public bool HasRecordFlag()
        {
            foreach (var i in items) if (i.record) return true;

            return false;
        }

        public bool HasRecordingFlag()
        {
            foreach (var i in items) if (i.recording) return true;

            return false;
        }

        public RecordItem GetCurrent()
        {
            foreach (var i in items) if (i.record) return i;

            return null;
        }

        public string GetRootPath()
        {
            switch (recordPathOptions)
            {
                case RecordPathOptions.Cloud: return cloudPath.FixOSPath();
                case RecordPathOptions.Project: return Directory.GetCurrentDirectory().FixOSPath();
                case RecordPathOptions.Absolute: return path.FixOSPath();
            }

            return "";
        }

        public string GetPath()
        {
            string r = path;

            switch (recordPathOptions)
            {
                case RecordPathOptions.Cloud:
                case RecordPathOptions.Project:
                    if (r.StartsWith("/", StringComparison.Ordinal) || r.StartsWith("\\", StringComparison.Ordinal))
                    {
                        r = r.Substring(1);
                    }
                    return Path.Combine(GetRootPath(), r).FixOSPath();
                case RecordPathOptions.Absolute: return path.FixOSPath();

            }

            return "";
        }
    }

    [Serializable]
    public class RecordItem
    {
        public bool included = true;
        public string guid = Guid.NewGuid().ToString();
        public string name = "";
        public SceneAsset scene;
        public MovieRecorderSettings movieRecorderSettings;
        public ImageRecorderSettings imageRecorderSettings;
        public RecorderAudioSettings audioRecorderSettings;

        public bool record = false;
        public bool mov = true;

        [NonSerialized] public bool recording = false;

        public string GetTempImagePath() => Path.Combine(Directory.GetCurrentDirectory(), "Temp/Recorder/" + guid).FixOSPath();

        public string GetPath()
        {
            foreach (var g in data.groups)
            {
                if (g.items.Contains(this)) return Path.Combine(g.GetPath(), name).FixOSPath();
            }

            return "";
        }

        public RecordingOptions GetRecordingOption()
        {
            foreach (var g in data.groups)
            {
                if (g.items.Contains(this)) return g.recordingOptions;
            }

            return RecordingOptions.Undefined;
        }

        public RecorderControllerSettings GetRecorderControllerSettings()
        {
            foreach (var g in data.groups)
            {
                if (g.items.Contains(this)) return g.recorderControllerSettings;
            }
            return null;
        }

        public MovieRecorderSettings GetMovieRecorderSettings()
        {
            if (this.movieRecorderSettings != null) return this.movieRecorderSettings;

            foreach (var g in data.groups)
            {
                if (g.items.Contains(this)) return g.movieRecorderSettings;
            }

            return null;
        }

        public ImageRecorderSettings GetImageRecorderSettings()
        {
            if (this.imageRecorderSettings != null) return this.imageRecorderSettings;

            foreach (var g in data.groups)
            {
                if (g.items.Contains(this)) return g.imageRecorderSettings;
            }

            return null;
        }

        public RecorderAudioSettings GetAudioRecorderSettings()
        {
            if (this.audioRecorderSettings != null) return this.audioRecorderSettings;

            foreach (var g in data.groups)
            {
                if (g.items.Contains(this)) return g.audioRecorderSettings;
            }

            return null;
        }
    }

    static string _cloudPath = null;

    public static string cloudPath
    {
        get
        {
            if (string.IsNullOrEmpty(_cloudPath)) _cloudPath = EditorPrefs.GetString("RECORDER_CLOUD_PATH", "");
            return _cloudPath;
        }
        set
        {
            if (_cloudPath != value) EditorPrefs.SetString("RECORDER_CLOUD_PATH", value);
            _cloudPath = value;
        }
    }

    [SerializeField]
    public List<RecordGroup> groups = new List<RecordGroup>();

    public RecordItem GetCurrent()
    {
        foreach (var g in groups)
        {
            foreach (var i in g.items)
            {
                if (i.record) return i;
            }
        }

        return null;
    }

    public void ProcessImage2MOV()
    {
        if (EditorApplication.isPlaying || EditorApplication.isPaused) return;
        foreach (var g in groups)
        {
            if (g.recordingOptions != RecordingOptions.Image) continue;
            foreach (var i in g.items)
            {
                if (Directory.Exists(i.GetTempImagePath()))
                {
                    var s = i.GetImageRecorderSettings();
                    var t = FFMpeg.Image2MOV.png;
                    switch (s.OutputFormat)
                    {
                        case ImageRecorderOutputFormat.PNG:
                            t = FFMpeg.Image2MOV.png;
                            break;
                        case ImageRecorderOutputFormat.JPEG:
                            t = FFMpeg.Image2MOV.jpg;
                            break;
                        case ImageRecorderOutputFormat.EXR:
                            break;
                    }
                    var p = i.GetTempImagePath();
                    var f = Path.Combine(p, "done.txt");
                    if (Directory.Exists(p) && !File.Exists(f))
                    {
                        File.WriteAllText(f, "");
                        FFMpeg.ConvertImageSequence2MOV(g.recorderControllerSettings.FrameRate, t, p, "%04d", i.GetPath() + ".mov");
                    }
                }
            }
        }
    }

    public bool HasRecordFlag()
    {
        foreach (var g in groups) if (g.HasRecordFlag()) return true;

        return false;
    }

    public bool HasRecordingFlag()
    {
        foreach (var g in groups) if (g.HasRecordingFlag()) return true;

        return false;
    }

    public void ClearRecordFlag()
    {
        foreach (var g in groups) g.ClearRecordFlags();
    }

    public void ClearRecordingFlag()
    {
        foreach (var g in groups) g.ClearRecordingFlags();
    }

    public void UnflagAll()
    {
        foreach (var g in groups)
        {
            foreach (var i in g.items)
            {
                i.record = false;
            }
        }
    }

    public void Save()
    {
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }
}