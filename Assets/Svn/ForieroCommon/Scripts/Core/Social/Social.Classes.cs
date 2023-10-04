using UnityEngine;
using System.Collections;

namespace ForieroEngine
{
	public static partial class Social
	{
		public enum SocialNetwork
		{
			Facebook,
			Twitter
		}

		[System.Serializable]
		public class SocialVars
		{
			public string projectName = "";
			public string projectCaption = "";
			public string projectMessage = "";
			public string projectURL = "http://www.foriero.com/";
		}

		[System.Serializable]
		public class FacebookVars
		{
			public string facebookURL = "https://www.facebook.com/Foriero";
			[Tooltip ("Recommended resolution is 350x230.")]
			public Texture2D facebookImage = null;

			public string facebookAppId = "";
			public string facebookAppSecret = "";
			public string facebookDisplayName = "";

			[System.Serializable]
			public class IOSVars
			{
				public bool useNativeDialogs = true;
				[HideInInspector]
				public bool canUserUseComposer = false;
				public bool ignoreCanUserUseComposer = true;
				public bool includeImage = true;
			}

			public IOSVars ios;

			[System.Serializable]
			public class OSXVars
			{
				public bool useNativeDialogs = true;
				[HideInInspector]
				public bool canUserUseComposer = false;
				public bool ignoreCanUserUseComposer = true;
				public bool includeImage = true;
			}

			public OSXVars osx;
		}

		[System.Serializable]
		public class TwitterVars
		{
			public string twitterURL = "https://twitter.com/foriero";
			[Tooltip ("Recommended resolution is 350x230.")]
			public Texture2D twitterImage = null;

			public string twitterAPIKey = "";
			public string twitterAPISecret = "";

			[System.Serializable]
			public class IOSVars
			{
				public bool useNativeDialogs = true;
				[HideInInspector]
				public bool canUserUseComposer = false;
				public bool ignoreCanUserUseComposer = true;
				public bool includeImage = true;
			}

			public IOSVars ios;

			[System.Serializable]
			public class OSXVars
			{
				public bool useNativeDialogs = true;
				[HideInInspector]
				public bool canUserUseComposer = false;
				public bool ignoreCanUserUseComposer = true;
				public bool includeImage = true;
			}

			public OSXVars osx;
		}
	}
}
