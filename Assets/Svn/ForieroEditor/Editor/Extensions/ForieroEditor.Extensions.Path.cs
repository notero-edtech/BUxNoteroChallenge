using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Extensions
{
    public static partial class ForieroEditorExtensions
    {
        public static bool IsFolder(this Object o) => AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(o));
        public static bool IsScript(this Object o) => AssetDatabase.GetAssetPath(o).EndsWith(".cs");
        private static readonly string GetCurrentDirectoryInternal = Directory.GetCurrentDirectory();
        public static string GetCurrentDirectory() => GetCurrentDirectoryInternal;
        public static string FixUnixPath(this string s) => string.IsNullOrEmpty(s) ? s : s.Replace(@"\", "/");
        public static string FixWinPath(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace("/", @"\");
            return s.Replace(@"\\", @"\");
        }
        
        public static string FixOSPath(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
            return s.Replace(@"\", "/");
#elif UNITY_EDITOR_WIN
			s = s.Replace("/", @"\");
			return s.Replace(@"\\", @"\"); 
#else
			return s;
#endif
        }

        public static string FixAssetsPath(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace(@"\", "/");
            return s.Replace("//", "/");
        }

        public static string GetAssetPathFromFullPath(this string s) => "Assets" + s.FixAssetsPath().Replace(Application.dataPath.FixAssetsPath(), "");
        public static string GetFullPathFromAssetPath(this string s) => Path.Combine(GetCurrentDirectory(), s).FixOSPath();
        public static string RemoveProjectPath(this string s) => s.FixOSPath().Replace(GetCurrentDirectory().FixOSPath(), "").RemoveLeadingSlash().RemoveLeadingBackwardSlash();
        public static string RemovePath(this string s, string path) => s.FixOSPath().Replace(path.FixOSPath(), "").RemoveLeadingSlash().RemoveLeadingBackwardSlash();
        public static bool IsInProjectFolder(this string s) => s.FixOSPath().Contains(GetCurrentDirectory().FixOSPath());
        public static bool IsInProjectAssetsFolder(this string s) => s.FixOSPath().Contains(Path.Combine(GetCurrentDirectory(), "Assets").FixOSPath());
        public static bool IsItAssetPath(this string s) => s.StartsWith("Assets/", System.StringComparison.InvariantCulture) || s.StartsWith(@"Assets\", System.StringComparison.InvariantCulture);

        public static string RemoveLeadingSlash(this string s)
        {
            if (s.StartsWith("/", System.StringComparison.InvariantCulture)) { s = s.Substring(1); }
            return s;
        }

        public static string RemoveLeadingBackwardSlash(this string s)
        {
            if (s.StartsWith("\\", System.StringComparison.InvariantCulture)) { s = s.Substring(1); }
            return s;
        }
        
        public static string RemoveEndingSlash(this string s)
        {
            if (s.EndsWith("/", System.StringComparison.InvariantCulture)) { s = s.Substring(0, s.Length - 2); }
            return s;
        }

        public static string RemoveEndingBackwardSlash(this string s)
        {
            if (s.EndsWith("\\", System.StringComparison.InvariantCulture)) { s = s.Substring(0, s.Length - 2); }
            return s;
        }

        public static string DoubleQuotes(this string s) => "\"" + s + "\"";
        public static string SingleQuotes(this string s) => "'" + s + "'";

        public static bool FileEquals(this string f1, string f2)
        {
            var f1Info = new FileInfo(f1);
            var f2Info = new FileInfo(f2);

            if (f1Info.Length != f2Info.Length) return false;

            using (FileStream s1 = new FileStream(f1, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream s2 = new FileStream(f2, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader b1 = new BinaryReader(s1))
            using (BinaryReader b2 = new BinaryReader(s2))
            {
                while (true)
                {
                    byte[] data1 = b1.ReadBytes(64 * 1024);
                    byte[] data2 = b2.ReadBytes(64 * 1024);
                    if (data1.Length != data2.Length) return false;
                    if (data1.Length == 0) return true;
                    if (!data1.SequenceEqual(data2)) return false;
                }
            }
        }

        public static void DeleteEmptyDirectories(this string path, bool alert = true, bool debug = false)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("WARNING Trying to delete empty directories for empty path!!!");
                return;
            }

            var topDirectoryInfo = new DirectoryInfo(path.FixOSPath());
            var directoryInfos = topDirectoryInfo.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (var directoryInfo in directoryInfos)
            {
                var delete = false;

                foreach (var directory in topDirectoryInfo.GetDirectories())
                {
                    DeleteEmptyDirectories(directory.FullName, alert, debug);
                }

                var cleanedDirectoryInfo = new DirectoryInfo(directoryInfo.FullName);

                if (cleanedDirectoryInfo.GetDirectories().Count() == 0 && cleanedDirectoryInfo.GetFiles().Count() == 0)
                {
                    if (alert) delete = EditorUtilities.DisplayDialog("Delete", "Delete directory ? : " + cleanedDirectoryInfo.FullName, "Yes", "No");
                    else delete = true;
                    
                    if (delete)
                    {

                        if (cleanedDirectoryInfo.FullName.IsInProjectAssetsFolder())
                        {
                            string meta = cleanedDirectoryInfo.FullName + ".meta";
                            if(File.Exists(meta)) File.Delete(meta);
                            else Debug.LogError("Trying to delete meta file that does not exits : " + meta);
                        }                            
                        cleanedDirectoryInfo.Delete();

                        if (debug) Debug.Log("Directory deleted : " + cleanedDirectoryInfo.FullName);                        
                    }
                }
            }
        }
    }
}
