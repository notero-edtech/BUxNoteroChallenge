using UnityEngine;
using System.Collections;

#if FORIERO_SOCIALS
using Prime31;
#endif

namespace ForieroEngine
{
	#if UNITY_IOS && !UNITY_EDITOR && FORIERO_SOCIALS
	public static partial class Social {
		static void InitTwitter(){
			TwitterManager.loginSucceededEvent += (s) => {
				twitterInitialised = true;
				Debug.Log(s);
			};

			TwitterBinding.init (settings.twitter.twitterAPIKey, settings.twitter.twitterAPISecret);
			settings.twitter.ios.canUserUseComposer = settings.twitter.ios.ignoreCanUserUseComposer ? true : TwitterBinding.canUserTweet ();
		}

		static void PostTwitterInternal (string text, string name = null, string caption = null, string link = null, Texture2D texture = null){
			string imagePath = "";

			if (texture && settings.twitter.ios.includeImage) {
				if (Social.SaveTexture2DToPersistentPath (texture, "twitter_share.png")) {
					imagePath = texture == null ? "" : Social.GetFilePersistentPath ("twitter_share.png");
				}
			}

			if (settings.twitter.ios.useNativeDialogs && settings.twitter.ios.canUserUseComposer) {
				Debug.Log ("Sharing Twitter : TweetComposer");
				TwitterBinding.showTweetComposer (text, imagePath, link);
			} else if (TwitterBinding.isLoggedIn ()) {
				Debug.Log ("Sharing Twitter : PostStatusUpdate");
				TwitterBinding.postStatusUpdate (text + " " + link, imagePath);
			} else {
				Debug.Log ("Sharing Twitter : ShareItems");
				SharingBinding.setPopoverPosition (Input.touches [0].position.x / 2f, (Screen.height - Input.touches [0].position.y) / 2f);
				if (string.IsNullOrEmpty (imagePath)) {
					SharingBinding.shareItems (new string[] { link, text });
				} else {
					SharingBinding.shareItems (new string[] { link, imagePath, text });
				}
			}
		}
	}
	#endif
}
