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
		static void InitTwitter(){

		}

		static void PostTwitterInternal (string text, string name = null, string caption = null, string link = null, Texture2D texture = null){
			Debug.Log ("Sharing Twitter : ShareItems");

			string imagePath = "";

			if (texture && settings.twitter.osx.includeImage) {
				if (Social.SaveTexture2DToPersistentPath (texture, "twitter_share.png")) {
					imagePath = texture == null ? "" : Social.GetFilePersistentPath ("twitter_share.png");
				}
			}

			SharingMac.shareItems (new string[] { text, imagePath, link }, MacSharingService.Twitter);
		}
	}
	#endif
}
