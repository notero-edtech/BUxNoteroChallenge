using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if FORIERO_SOCIALS
using Prime31;
#endif

namespace ForieroEngine
{
	#if UNITY_ANDROID && !UNITY_EDITOR && FORIERO_SOCIALS
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
				Debug.Log ("Facebook : Session Opened");
				_hasPublishActions = FacebookAndroid.getSessionPermissions ().Contains ("publish_actions");
				if(!_hasPublishActions) FacebookAndroid.loginWithPublishPermissions (new string[] { "publish_actions" });
			};

			FacebookAndroid.init ();

			//FacebookAndroid.setSessionLoginBehavior( FacebookSessionLoginBehavior.SUPPRESS_SSO );
	
		}

		static void PostFacebookInternal (string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
		{
			Dictionary<string,object> facebookParams = new Dictionary<string,object> {
			{ "link", link },
			{ "name",  name },
			{ "caption", caption },
			{ "description", text }
			};
			FacebookAndroid.showFacebookShareDialog (facebookParams);
		}
	}
	#endif
}
