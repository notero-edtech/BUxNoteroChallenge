using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
	{
		[MenuItem ("Foriero/Links/Portals/Amazon/Developer Portal", false, LinksInternal.menuOrderIndex)] public static void OpenAmazonDeveloperPortal () => Application.OpenURL ("https://developer.amazon.com/home.html");
		[MenuItem ("Foriero/Links/Portals/Apple/Certificates Portal", false, LinksInternal.menuOrderIndex)] public static void OpenAppleCertificatesPortal () => Application.OpenURL ("https://developer.apple.com/account/overview.action");
		[MenuItem ("Foriero/Links/Portals/Apple/iOS Developer Portal", false, LinksInternal.menuOrderIndex)]	public static void OpeniOSDeveloperPortal () => Application.OpenURL ("https://developer.apple.com/ios/");		
		[MenuItem ("Foriero/Links/Portals/Apple/OSX Developer Portal", false, LinksInternal.menuOrderIndex)]	public static void OpenOSXDeveloperPortal () => Application.OpenURL ("https://developer.apple.com/osx/");
		[MenuItem ("Foriero/Links/Portals/Google Play/Developer Portal", false, LinksInternal.menuOrderIndex)] public static void OpenGooglePlayDeveloperPortal () => Application.OpenURL ("https://play.google.com/apps/publish/");
		[MenuItem ("Foriero/Links/Portals/Microsoft/Developer Portal", false, LinksInternal.menuOrderIndex)] public static void OpenMicrosoftDeveloperPortal () => Application.OpenURL ("https://dev.windows.com/");
		[MenuItem ("Foriero/Links/Portals/Samsung/Developer Portal", false, LinksInternal.menuOrderIndex)] public static void OpenSamsungDeveloperPortal () =>  Application.OpenURL ("http://seller.samsungapps.com/");
		[MenuItem ("Foriero/Links/Portals/Ubuntu/Developer Portal", false, LinksInternal.menuOrderIndex)] public static void OpenUbuntuDeveloperPortal () => Application.OpenURL ("https://myapps.developer.ubuntu.com/");
		[MenuItem ("Foriero/Links/Portals/Facebook/Developer Portal", false, LinksInternal.menuOrderIndex)] public static void OpenFacebookDeveloperPortal () => Application.OpenURL ("https://developers.facebook.com/");
		[MenuItem ("Foriero/Links/Others/SVN Command Line", false, LinksInternal.menuOrderIndex)] public static void OpenSVNCommandLineLink () => Application.OpenURL ("https://subversion.apache.org/packages.html");
		[MenuItem ("Foriero/Links/Others/ImageMagick", false, LinksInternal.menuOrderIndex)]	public static void OpenImageMagickLink () => Application.OpenURL ("http://www.imagemagick.org/");
		[MenuItem ("Foriero/Links/Others/MacPorts", false, LinksInternal.menuOrderIndex)] public static void OpenMacPortsLink () => Application.OpenURL ("https://www.macports.org/");		
		[MenuItem ("Foriero/Links/Spine/Editor", false, LinksInternal.menuOrderIndex)] public static void OpenSpineEditorLink () => Application.OpenURL ("https://www.esotericsoftware.com/");
		[MenuItem ("Foriero/Links/Spine/Runtime", false, LinksInternal.menuOrderIndex)] public static void OpenSpineRuntimesLink () => Application.OpenURL ("https://github.com/EsotericSoftware/spine-runtimes");

		static class LinksInternal
        {
			public const int menuOrderIndex = -800;
        }
	}
}