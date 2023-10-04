using UnityEngine;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using ForieroEditor.Extensions;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

public class MIDIUnifiedPostprocessBuildPlayer
{

    private static void RunProcess(string aCommand, string anArguments)
    {
        var p = new System.Diagnostics.Process();
        var buildOutput = "";
        try
        {
            p.StartInfo.FileName = aCommand;
            p.StartInfo.Arguments = anArguments;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            var output = p.StandardOutput.ReadToEnd();
            var error = p.StandardError.ReadToEnd();
            p.WaitForExit();
            p.Close();
            if (!string.IsNullOrEmpty(output)) { Debug.Log("OUTPUT : " + output); }
            if (!string.IsNullOrEmpty(error)) { Debug.Log("ERROR : " + error); }
        }
        catch (System.Exception e)
        {
            buildOutput += "\n\n" + e.Message;
            UnityEngine.Debug.LogWarning(buildOutput);
            return;
        }
        finally
        {
            p.Dispose();
            System.GC.Collect();
        }
    }

    private static bool IsVersion42OrLater()
    {
        var version = Application.unityVersion;
        var versionComponents = version.Split('.');

        var majorVersion = 0;
        var minorVersion = 0;

        try
        {
            if (versionComponents != null && versionComponents.Length > 0 && versionComponents[0] != null)
                majorVersion = Convert.ToInt32(versionComponents[0]);
            if (versionComponents != null && versionComponents.Length > 1 && versionComponents[1] != null)
                minorVersion = Convert.ToInt32(versionComponents[1]);
        }
        catch (System.Exception e) { Debug.LogError("Error parsing Unity version number: " + e); }

        return (majorVersion > 4 || (majorVersion == 4 && minorVersion >= 2));
    }

    [PostProcessBuild(20)] private static void OnPostprocessBuildPlayer(BuildTarget target, string buildPath)
    {        
        var plugins = Directory.GetFiles(Application.dataPath, "midiunified_plugins.txt", SearchOption.AllDirectories);
        if (plugins.Length == 0) return;
        var pluginsFolder = Path.GetDirectoryName(plugins[0]).FixOSPath();
        Debug.Log("MIDIUnified : " + pluginsFolder);

        #region LINUX

        if (target == BuildTarget.StandaloneLinux64)
        {
            return;

            //string src_path_x86 = Path.Combine(pluginsFolder, "Linux/x86");
            //string dst_path_x86 = Path.Combine(Path.GetDirectoryName(buildPath), "x86");
            //if (!Directory.Exists(dst_path_x86))
            //{
            //    Directory.CreateDirectory(dst_path_x86);
            //}

            //if (File.Exists(Path.Combine(src_path_x86, "libbass.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86, "libbass.so"), Path.Combine(dst_path_x86, "libbass.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86, "libbassmidi.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86, "libbassmidi.so"), Path.Combine(dst_path_x86, "libbassmidi.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86, "libbass_fx.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86, "libbass_fx.so"), Path.Combine(dst_path_x86, "libbass_fx.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86, "libbassmix.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86, "libbassmix.so"), Path.Combine(dst_path_x86, "libbassmix.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86, "libbassenc.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86, "libbassenc.so"), Path.Combine(dst_path_x86, "libbassenc.so"), true);
            //}

            //string src_path_x86_64 = Path.Combine(pluginsFolder, "Linux/x86_64");
            //string dst_path_x86_64 = Path.Combine(Path.GetDirectoryName(buildPath), "x86_64");
            //if (!Directory.Exists(dst_path_x86_64))
            //{
            //    Directory.CreateDirectory(dst_path_x86_64);
            //}

            //if (File.Exists(Path.Combine(src_path_x86_64, "libbass.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86_64, "libbass.so"), Path.Combine(dst_path_x86_64, "libbass.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86_64, "libbassmidi.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86_64, "libbassmidi.so"), Path.Combine(dst_path_x86_64, "libbassmidi.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86_64, "libbass_fx.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86_64, "libbass_fx.so"), Path.Combine(dst_path_x86_64, "libbass_fx.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86_64, "libbassmix.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86_64, "libbassmix.so"), Path.Combine(dst_path_x86_64, "libbassmix.so"), true);
            //}

            //if (File.Exists(Path.Combine(src_path_x86_64, "libbassenc.so")))
            //{
            //    File.Copy(Path.Combine(src_path_x86_64, "libbassenc.so"), Path.Combine(dst_path_x86_64, "libbassen.so"), true);
            //}
        }

        #endregion

        #region OSX

        if (target == BuildTarget.StandaloneOSX)
        {
            //CopyOSX("libbass.dylib", buildPath);
            //CopyOSX("libbassmidi.dylib", buildPath);
            //CopyOSX("libbass_fx.dylib", buildPath);
            //CopyOSX("libbassmix.dylib", buildPath);
            //CopyOSX("libbassenc.dylib", buildPath);
        }

        #endregion

        #region iOS

        if (target == BuildTarget.iOS)
        {
#if UNITY_IOS 
            string projectPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";

            PBXProject pbxProject = new PBXProject();
            pbxProject.ReadFromFile(projectPath);

            pbxProject.AddFrameworkToProject(pbxProject.GetUnityFrameworkTargetGuid(), "CoreMIDI.framework", true);

#if BASS24_DISABLED
            pbxProject.SetBuildProperty(pbxProject.GetUnityMainTargetGuid(), "ENABLE_BITCODE", "YES");
            pbxProject.SetBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), "ENABLE_BITCODE", "YES");
#else
            pbxProject.SetBuildProperty(pbxProject.GetUnityMainTargetGuid(), "ENABLE_BITCODE", "NO");
            pbxProject.SetBuildProperty(pbxProject.GetUnityFrameworkTargetGuid(), "ENABLE_BITCODE", "NO");                
#endif
            pbxProject.WriteToFile(projectPath);
#endif
        }

        #endregion

    }

    private static void CopyOSX(string pluginName, string buildPath)
    {
        var src = Directory.GetFiles(Application.dataPath, pluginName, SearchOption.AllDirectories)[0];
        Debug.Log(src);

        var dst = buildPath + "/Contents/Frameworks/MonoEmbedRuntime/osx/" + pluginName;
        var directory = Path.GetDirectoryName(dst);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        Debug.Log(dst);

        File.Copy(src, dst, true);
    }
}