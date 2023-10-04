using UnityEngine;
using System.Collections;
using ForieroEngine;

#if FORIERO_SOCIALS
using Prime31;
#endif

using Social = ForieroEngine.Social;

namespace ForieroEngine
{
	#if UNITY_STANDALONE_OSX && !UNITY_EDITOR && FORIERO_SOCIALS
	public static partial class Social {
		static void InitFacebook ()
		{
			SharingMac.didShareItemsEvent += OSX_HandledidShareItemsEvent;
			SharingMac.didFailToShareItemsEvent += OSX_HandledidFailToShareItemsEvent;
		}

		static void PostFacebookInternal (string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
		{
			string imagePath = "";

			if (texture && settings.facebook.osx.includeImage) {
				if (Social.SaveTexture2DToPersistentPath (texture, "facebook_share.png")) {
					imagePath = texture == null ? "" : Social.GetFilePersistentPath ("facebook_share.png");
				}
			}

			SharingMac.shareItems (new string[] { text, imagePath, link }, MacSharingService.Facebook);
		}

		static void OSX_HandledidShareItemsEvent ()
		{
			Debug.Log ("Sharing Success");
		}

		static void OSX_HandledidFailToShareItemsEvent (string obj)
		{
			Debug.Log ("Sharing Failed");
		}
	}
	#endif
}
