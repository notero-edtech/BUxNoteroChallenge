using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class AdsProxySettings : Settings<AdsProxySettings>, ISettingsProvider
{
#if UNITY_EDITOR
	[MenuItem("Foriero/Settings/Ads", false, -1000)] public static void AdsSettingsMenu() => Select();	
#endif
		
	public bool initialize = false;

	public string iosId = "";
	public string androidId = "";

	public string wsaPcTabletAppId = "";
	public string wsaPcTabletAdId = "";

	public string wsaMobileAppId = "";
	public string wsaMobileAdId = "";
}
