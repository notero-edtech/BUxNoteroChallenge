using Social = ForieroEngine.Social;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class SocialSettings : Settings<SocialSettings>, ISettingsProvider
{
#if UNITY_EDITOR
	[MenuItem("Foriero/Settings/Social", false, -1000)] public static void SocialSettingsMenu() => Select();
#endif

	public bool initialize = true;
	public Social.SocialVars social;
	public Social.FacebookVars facebook;
	public Social.TwitterVars twitter;
}
