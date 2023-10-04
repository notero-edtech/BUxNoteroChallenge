using UnityEngine;
using System.Collections;

#if FORIERO_SOCIALS
using Prime31;
#endif

namespace ForieroEngine
{
	#if UNITY_WSA && !UNITY_EDITOR && FORIERO_SOCIALS
	public static partial class Social {
		static void InitFacebook(){

		}

		static void PostFacebookInternal (string text, string name = null, string caption = null, string link = null, Texture2D texture = null){

		}
	}
	#endif
}
