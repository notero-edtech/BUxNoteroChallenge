using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ForieroEngine.Extensions;
using UnityEngine;

namespace ForieroEngine
{
    public class ForieroPlayerPrefs : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
            GameObject fpp = new GameObject("ForieroPlayerPrefs");
            DontDestroyOnLoad(fpp);
            fpp.AddComponent<ForieroPlayerPrefs>();
            if (ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (ForieroPlayerPrefs - MonoBehaviour - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

        void OnDestroy()
        {
            if (ForieroDebug.General) Debug.Log("ForieroPlayerPrefs : OnDestroy");
            PlayerPrefs.Terminate();
        }

        void OnApplicationQuit()
        {
            if (ForieroDebug.General) Debug.Log("ForieroPlayerPrefs : OnApplicationQuit");
            PlayerPrefs.Terminate();
        }
    }

    public static class PlayerPrefs
    {
        private static bool _debug = false;
        private const string File = "foriero.txt";
        private static string _fileFullPath = "";
        private static ES3File _esFile;

        private const string FileBackup = "foriero_backup.txt";
        private static string _fileBackupFullPath = "";

        private const string Pass = "159487";

        private static string _deviceUniqueIdentifier = "";
        
        private static ES3Settings _settings = null;

        private static Dictionary<string, string> _strings = new Dictionary<string, string>();
        private static Dictionary<string, float> _floats = new Dictionary<string, float>();
        private static Dictionary<string, int> _ints = new Dictionary<string, int>();
        private static Dictionary<string, bool> _bools = new Dictionary<string, bool>();

        private static volatile bool _save = false;
        private static bool _saveBackup = false;
        private static volatile bool _terminate = false;

        struct StringStruct
        {
            public string id;
            public string value;
        }

        struct FloatStruct
        {
            public string id;
            public float value;
        }

        struct IntStruct
        {
            public string id;
            public int value;
        }

        struct BoolStruct
        {
            public string id;
            public bool value;
        }

        private static ConcurrentQueue<StringStruct> _stringsThread = new ();
        private static ConcurrentQueue<FloatStruct> _floatsThread = new ();
        private static ConcurrentQueue<IntStruct> _intsThread = new ();
        private static ConcurrentQueue<BoolStruct> _boolsThread = new ();

        private static readonly int SavingPeriodMilliseconds = 100;
        private static readonly double BackupPeriodMilliseconds = 5000;
        private static double _elapsedBackupTime = 0;

        private static Thread _savingThread;
        private static bool _initialized = false;
        
        private static void InitSavingThread()
        {
            if (ForieroDebug.General) UnityEngine.Debug.Log("PlayerPrefs Thread : Initializing");
            _savingThread = new Thread(SilentSaving);
            _savingThread.Start();
        }

        private static void SilentSaving()
        {
            do
            {
                Thread.Sleep(SavingPeriodMilliseconds);
                _elapsedBackupTime += SavingPeriodMilliseconds;
                if (_elapsedBackupTime > BackupPeriodMilliseconds && _saveBackup)
                {
                    _saveBackup = false;
                    _elapsedBackupTime = 0;

                    if (_debug) UnityEngine.Debug.Log("PlayerPrefs Thread : Saving backup!");
                    if(System.IO.File.Exists(_fileFullPath)) System.IO.File.Copy(_fileFullPath, _fileBackupFullPath, true);
                }
                else
                {
                    SaveInternal();
                }
            } while (!_terminate);
        }

        public static void Save()
        {
            _save = true;
        }

        public static void Terminate()
        {
            if(_debug) UnityEngine.Debug.Log("PlayerPrefs Thread : Aborting");
            _terminate = true;
            if (_debug) UnityEngine.Debug.Log("PlayerPrefs Thread : Joining");
            if (_savingThread != null) _savingThread.Join();
            if (_debug) UnityEngine.Debug.Log("PlayerPrefs Thread : Finished");
        }

        private static void SaveInternal()
        {
            if (_save && _esFile != null)
            {
                _elapsedBackupTime = 0;
                if (_debug) UnityEngine.Debug.Log("PlayerPrefs Thread : Saving");

                while (_stringsThread.TryDequeue(out StringStruct stringStruct)){ _esFile.Save<string>(stringStruct.id, stringStruct.value); }
                while (_floatsThread.TryDequeue(out FloatStruct floatStruct)){ _esFile.Save<float>(floatStruct.id, floatStruct.value); }
                while (_intsThread.TryDequeue(out IntStruct intStruct)){ _esFile.Save<int>(intStruct.id, intStruct.value); }
                while (_boolsThread.TryDequeue(out BoolStruct boolStruct)){ _esFile.Save<bool>(boolStruct.id, boolStruct.value); }

                _esFile.Sync();

                _save = false;
                _saveBackup = true;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            if (_initialized) return;
            _initialized = true;
            _debug = ForieroDebug.General;
            
            System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;

            _autoSave = ForieroSettings.instance.autoSave;
            _threaded = ForieroSettings.instance.threaded;

            _fileFullPath = Path.Combine(Application.persistentDataPath, File).FixOSPath();
            _fileBackupFullPath = Path.Combine(Application.persistentDataPath, FileBackup).FixOSPath();


            if (_settings != null){ return; }

            ES3.Init();

            _deviceUniqueIdentifier = SystemInfo.deviceUniqueIdentifier;

            _settings = new ES3Settings(File)
            {
                encryptionPassword = Pass,
#if UNITY_EDITOR
                encryptionType = ES3.EncryptionType.None
#else
                encryptionType = ES3.EncryptionType.AES
#endif
            };

            _esFile = null;
                                 
            try { _esFile = new ES3File(File, _settings); }
            catch
            {
                _esFile = null;
                #if !UNITY_WEBGL
                if (UnifiedIO.File.Exists(File)) UnifiedIO.File.Delete(File);
                if (_debug) UnityEngine.Debug.LogError("ES3.LoadAll error!!!");
                #endif
            }

            if (_esFile == null && ES3.FileExists())
            {
                System.IO.File.Copy(_fileBackupFullPath, _fileFullPath, true);
            }

            try { _esFile = new ES3File(File, _settings); }
            catch
            {
                _esFile = null;
                #if !UNITY_WEBGL
                if (UnifiedIO.File.Exists(File)) UnifiedIO.File.Delete(File);
                if (_debug) UnityEngine.Debug.LogError("ES3.LoadAll error!!!");
                #endif
            }

            if(_esFile == null) _esFile = new ES3File(File, _settings);

            if (_esFile != null)
            {
                var keys = _esFile.GetKeys();
                foreach (string key in keys)
                {
                    //Type type = entry.Value.GetType();

                    var es3Type = _esFile.GetKeyType(key);
                    if (es3Type == null) continue;

                    if (es3Type == typeof(bool))
                    {
                        _bools.Add(key, _esFile.Load<bool>(key));
                        if (_debug) UnityEngine.Debug.Log("Loaded Strings Key : " + key + " " + _bools[key].ToString());
                    }
                    else if (es3Type == typeof(float))
                    {
                        _floats.Add(key, _esFile.Load<float>(key));
                        if (_debug) UnityEngine.Debug.Log("Loaded Floats Key : " + key + " " + _floats[key].ToString());
                    }
                    else if (es3Type == typeof(int))
                    {
                        _ints.Add(key, _esFile.Load<int>(key));
                        if (_debug) UnityEngine.Debug.Log("Loaded Ints Key : " + key + " " + _ints[key].ToString());
                    }
                    else if (es3Type == typeof(string))
                    {
                        _strings.Add(key, _esFile.Load<string>(key));
                        if (_debug) UnityEngine.Debug.Log("Loaded Strings Key : " + key + " " + _strings[key]);
                    }
                    else if (es3Type == typeof(Vector2))
                    {
                        //thisVar.Value = es2Data.Load<Vector2> (key);
                    }
                    else if (es3Type == typeof(Vector3))
                    {
                        //thisVar.Value = es2Data.Load<Vector3> (key);
                    }
                    else if (es3Type == typeof(Rect))
                    {
                        //thisVar.Value = es2Data.Load<Rect> (key);
                    }
                    else if (es3Type == typeof(Quaternion))
                    {
                        //thisVar.Value = es2Data.Load<Quaternion> (key);
                    }
                    else if (es3Type == typeof(Color))
                    {
                        //thisVar.Value = es2Data.Load<Color> (key);
                    }
                    else if (es3Type == typeof(Material))
                    {
                        //thisVar.Value = es2Data.Load<Material> (key);
                    }
                    else if (es3Type == typeof(Texture2D))
                    {
                        //thisVar.Value = es2Data.Load<Texture2D> (key);
                    }
                    else
                    {
                        //thisVar.Value = entry.Value as UnityEngine.Object;
                    }
                }
            }

            if (_threaded) { InitSavingThread(); }

            _save = true;

            if (ForieroDebug.CodePerformance) UnityEngine.Debug.Log("METHOD STOPWATCH (ForieroPlayerPrefs - BeforeSceneLoad): " + stopWatch?.Elapsed.ToString());
        }
               
        private static string GetId(string key) => _deviceUniqueIdentifier + "_" + key;
        
        private static bool _autoSave = false;
        private static bool _threaded = false;

        public static bool HasKey<T>(string key)
        {
            Init();
            var id = GetId(key);
            return Exists<T>(id);
        }

        private static void SetAutoSave()
        {
            if (_autoSave)
            {
                _save = true;
                if (!_threaded){ SaveInternal(); }
            }
        }

        public static void DeleteKey<T>(string key)
        {
            Init();

            string id = GetId(key);

            if (typeof(string) == typeof(T))
            {
                if (_strings.ContainsKey(id))
                {
                    _strings.Remove(id);
                    SetAutoSave();
                }
            }
            else if (typeof(float) == typeof(T))
            {
                if (_floats.ContainsKey(id))
                {
                    _floats.Remove(id);
                    SetAutoSave();
                }
            }
            else if (typeof(int) == typeof(T))
            {
                if (_ints.ContainsKey(id))
                {
                    _ints.Remove(id);
                    SetAutoSave();
                }
            }
            else if (typeof(bool) == typeof(T))
            {
                if (_bools.ContainsKey(id))
                {
                    _bools.Remove(id);
                    SetAutoSave();
                }
            }
        }

        public static void DeleteAll()
        {
            Init();

            _strings = new Dictionary<string, string>();
            _floats = new Dictionary<string, float>();
            _ints = new Dictionary<string, int>();
            _bools = new Dictionary<string, bool>();

            if (ES3.FileExists(File, _settings))
            {
                ES3.DeleteFile(File, _settings);
            }

            UnityEngine.PlayerPrefs.DeleteAll();

            SetAutoSave();
        }

        static void Save<T>(string id, T value) where T : IConvertible
        {
            Init();

            bool different = false;

            if (typeof(string) == typeof(T))
            {
                if (_strings.ContainsKey(id))
                {
                    var s = value as string;
                    if (_strings[id] != s)
                    {
                        _strings[id] = s;
                        different = true;
                    }
                }
                else
                {
                    _strings.Add(id, value as string);
                    different = true;
                }
            }
            else if (typeof(float) == typeof(T))
            {
                if (_floats.ContainsKey(id))
                {
                    var f = (float)Convert.ToDecimal(value);
                    if (Mathf.Abs(_floats[id] - f) > Mathf.Epsilon)
                    {
                        _floats[id] = f;
                        different = true;
                    }
                }
                else
                {
                    _floats.Add(id, (float)Convert.ToDecimal(value));
                    different = true;
                }
            }
            else if (typeof(int) == typeof(T))
            {
                if (_ints.ContainsKey(id))
                {
                    var i = (int)Convert.ToInt32(value);
                    if (_ints[id] != i)
                    {
                        _ints[id] = i;
                        different = true;
                    }
                }
                else
                {
                    _ints.Add(id, (int)Convert.ToInt32(value));
                    different = true;
                }
            }
            else if (typeof(bool) == typeof(T))
            {
                if (_bools.ContainsKey(id))
                {
                    var b = Convert.ToBoolean(value);
                    if (_bools[id] != b)
                    {
                        _bools[id] = b;
                        different = true;
                    }
                }
                else
                {
                    _bools.Add(id, Convert.ToBoolean(value));
                    different = true;
                }
            }

            if (different)
            {
                if (typeof(string) == typeof(T))
                {
                    _stringsThread.Enqueue(new StringStruct { id = id, value = value as string });
                    SetAutoSave();
                }
                else if (typeof(float) == typeof(T))
                {
                    _floatsThread.Enqueue(new FloatStruct { id = id, value = (float)Convert.ToDecimal(value) });
                    SetAutoSave();
                }
                else if (typeof(int) == typeof(T))
                {
                    _intsThread.Enqueue(new IntStruct { id = id, value = (int)Convert.ToInt32(value) });
                    SetAutoSave();
                }
                else if (typeof(bool) == typeof(T))
                {
                    _boolsThread.Enqueue(new BoolStruct { id = id, value = Convert.ToBoolean(value) });
                    SetAutoSave();
                }
            }
        }

        static bool Exists<T>(string id)
        {
            Init();
            if (typeof(string) == typeof(T)) { if (_strings.ContainsKey(id)) return true; }
            else if (typeof(float) == typeof(T)) { if (_floats.ContainsKey(id)) return true; }
            else if (typeof(int) == typeof(T)) { if (_ints.ContainsKey(id)) return true; }
            else if (typeof(bool) == typeof(T)) { if (_bools.ContainsKey(id)) return true; }
            return false;
        }

        public static bool GetBool(string key, bool defaultValue = false)
        {
            Init();
            var id = GetId(key);
            return _bools.ContainsKey(id) ? _bools[id] : defaultValue;
        }

        public static void SetBool(string key, bool value)
        {
            Init();
            var id = GetId(key);
            Save<bool>(id, value);
        }

        public static int GetInt(string key, int defaultValue = 0)
        {
            Init();
            var id = GetId(key);
            return _ints.ContainsKey(id) ? _ints[id] : defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            Init();
            var id = GetId(key);
            Save<int>(id, value);
        }

        public static string GetString(string key, string defaultValue = "")
        {
            Init();
            var id = GetId(key);
            return _strings.ContainsKey(id) ? _strings[id] : defaultValue;
        }

        public static void SetString(string key, string value)
        {
            Init();
            var id = GetId(key);
            Save<string>(id, value);
        }

        public static float GetFloat(string key, float defaultValue = 0f)
        {
            Init();
            var id = GetId(key);
            return _floats.ContainsKey(id) ? _floats[id] : defaultValue;
        }

        public static void SetFloat(string key, float value)
        {
            Init();
            var id = GetId(key);
            Save<float>(id, value);
        }
    }
}