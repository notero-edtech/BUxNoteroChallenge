using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Tools.MultiProjectBuilder
{
    public partial class MultiProjectBuilder : ScriptableObject
    {
        void SetPlatformIOS(PlatformVersion platformVersion)
        {
            if (platformVersion.targetGroup != BuildTargetGroup.iOS)
                return;

            PlayerSettings.iOS.buildNumber = platformVersion.build.ToString();

            PlayerSettings.SetIconsForTargetGroup(platformVersion.targetGroup, platformVersion.ios.GetIcons(IconKind.Application), IconKind.Application);
            PlayerSettings.SetIconsForTargetGroup(platformVersion.targetGroup, platformVersion.ios.GetIcons(IconKind.Notification), IconKind.Notification);
            PlayerSettings.SetIconsForTargetGroup(platformVersion.targetGroup, platformVersion.ios.GetIcons(IconKind.Settings), IconKind.Settings);
            PlayerSettings.SetIconsForTargetGroup(platformVersion.targetGroup, platformVersion.ios.GetIcons(IconKind.Spotlight), IconKind.Spotlight);
            PlayerSettings.SetIconsForTargetGroup(platformVersion.targetGroup, platformVersion.ios.GetIcons(IconKind.Store), IconKind.Store);
        }

    }
}