using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ForieroEditor.Extensions;
using ForieroEngine;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        string DefineSymbols(string currentDefines, string[] newDefines, string[] removeDefines)
        {
            List<string> currentSymbols = new List<string>(currentDefines.Split(';'));
            List<string> newSymbols = new List<string>(newDefines);
            List<string> removeSymbols = new List<string>(removeDefines);
            List<string> merged = currentSymbols.Union(newSymbols).Distinct().Except(removeSymbols).ToList();

            return string.Join(";", merged.ToArray());
        }

        public void SetPlatform(string guid)
        {
            foreach (var p in platformVersions.versions)
            {
                if (p.guid == guid) { SetPlatform(p); break; }
            }
        }

        public void SetPlatform(PlatformVersion platformVersion)
        {
            platformVersion.applied = true;
            var productName = platformVersion.productName;
            if(string.IsNullOrEmpty(productName)) productName = platformVersion.storeSetttings ? platformVersion.storeSetttings.appName : PlayerSettings.productName;
            PlayerSettings.productName = productName;
//            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[1] { platformVersion.icon });
            PlayerSettings.SetApplicationIdentifier(platformVersion.targetGroup, platformVersion.storeSetttings ? platformVersion.storeSetttings.GetBundleId() : "com.foriero.test");  
            PlayerSettings.SetScriptingBackend(platformVersion.targetGroup, platformVersion.scriptingImplementation);
            PlayerSettings.SetApiCompatibilityLevel(platformVersion.targetGroup, platformVersion.apiCompatibilityLevel);
            PlayerSettings.bundleVersion = platformVersion.version.ToString();

            if(platformVersion.inAppPurchases) platformVersion.targetGroup.AddSymbolDefine("FORIERO_INAPP");
            else platformVersion.targetGroup.RemoveSymbolDefine("FORIERO_INAPP");

            foreach (var rv in Enum.GetValues(typeof(ReleaseVersion)).Cast<ReleaseVersion>())
            {
                if (rv == ReleaseVersion.NONE) continue;
                if (rv == platformVersion.releaseVersion) platformVersion.targetGroup.AddSymbolDefine($"RELEASE_VERSION_{rv.ToString()}");
                else platformVersion.targetGroup.RemoveSymbolDefine($"RELEASE_VERSION_{rv.ToString()}");
            }
            
            foreach (var pp in Enum.GetValues(typeof(PublishingPlatform)).Cast<PublishingPlatform>())
            {
                if (pp == PublishingPlatform.NONE) continue;
                if (pp == platformVersion.publishingPlatform) platformVersion.targetGroup.AddSymbolDefine($"PUBLISHING_PLATFORM_{pp.ToString()}");
                else platformVersion.targetGroup.RemoveSymbolDefine($"PUBLISHING_PLATFORM_{pp.ToString()}");
            }
            
            foreach (var fv in Enum.GetValues(typeof(Foriero.ProjectVersion)).Cast<Foriero.ProjectVersion>())
            {
                if (fv == Foriero.ProjectVersion.NONE) continue;
                if (fv == platformVersion.projectVersion) platformVersion.targetGroup.AddSymbolDefine($"FORIERO_VERSION_{fv.ToString()}");
                else  platformVersion.targetGroup.RemoveSymbolDefine($"FORIERO_VERSION_{fv.ToString()}");
            }
            
            foreach (var d in platformVersion.includedDefineList) { platformVersion.targetGroup.AddSymbolDefine(d); }
            foreach(var d in platformVersion.excludedDefineList) { platformVersion.targetGroup.RemoveSymbolDefine(d); }

            if (platformVersion.sceneSettings) SceneSettings.ApplyToBuildSettings(platformVersion.sceneSettings);
           
            SetPlatformIOS(platformVersion);
            SetPlatformAndroid(platformVersion, platformVersion.storeSetttings);
            SetPlatformStandalone(platformVersion);
            SetPlatformWSA(platformVersion);

            if(platformVersion.storeSetttings) platformVersion.storeSetttings.Apply();
            if(platformVersion.spriteAtlasesSettings) platformVersion.spriteAtlasesSettings.Apply();
            
            foreach (var so in platformVersion.settingsList)
            {
                if (!so) continue;
                var m = so.GetType().GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if(m != null) m.Invoke(so, null);
            }
            
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}