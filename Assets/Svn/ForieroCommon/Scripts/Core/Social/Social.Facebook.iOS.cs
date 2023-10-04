using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if FORIERO_SOCIALS
using Prime31;
#endif

namespace ForieroEngine
{
	#if UNITY_IOS && !UNITY_EDITOR && FORIERO_SOCIALS
	public static partial class Social {
		static bool _hasPublishActions = false;

		static void InitFacebook(){
			// dump custom data to log after a request completes

			FacebookManager.loginFailedEvent += (error) => {
				Utils.logObject (error);	
			};

			FacebookManager.graphRequestCompletedEvent += result => {
				Debug.Log ("Facebook : Graph Request Completed");
				Prime31.Utils.logObject (result);
			};

			// when the session opens or a reauth occurs we check the permissions to see if we can publish
			FacebookManager.sessionOpenedEvent += () => {
				Debug.Log ("Facebook : Session Opened");
				_hasPublishActions = FacebookBinding.getSessionPermissions ().Contains ("publish_actions");
				if(!_hasPublishActions) FacebookBinding.loginWithPublishPermissions (new string[] { "publish_actions" });
			};

			FacebookBinding.init ();
			
			settings.facebook.ios.canUserUseComposer = settings.facebook.ios.ignoreCanUserUseComposer ? true : FacebookBinding.canUserUseFacebookComposer ();
		}

		static void PostFacebookInternal (string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
		{		
			string imagePath = "";

			if (texture && settings.facebook.ios.includeImage) {
				if (Social.SaveTexture2DToPersistentPath (texture, "facebook_share.png")) {
					imagePath = texture == null ? "" : Social.GetFilePersistentPath ("facebook_share.png");
				}
			}

			if (settings.facebook.ios.useNativeDialogs && settings.facebook.ios.canUserUseComposer) {
				FacebookBinding.showFacebookComposer (text, imagePath, link);
			} else {
				Dictionary<string,object> facebookParams = new Dictionary<string,object> {
					{ "link", link },
					{ "name", name },
					{ "caption", caption },
					{ "description", text }
				};
				FacebookBinding.showFacebookShareDialog (facebookParams);
			}	
		}
	}
	#endif
}
