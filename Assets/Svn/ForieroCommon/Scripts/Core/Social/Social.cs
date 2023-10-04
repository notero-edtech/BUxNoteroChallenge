using UnityEngine;

namespace ForieroEngine
{
    public static partial class Social
    {
        static bool initialized = false;
#pragma warning disable 414
        static bool facebookInitialised = false;
        static bool facebookLogged = false;

        static bool twitterInitialised = false;
        static bool twitterLogged = false;
#pragma warning restore 414

        public static SocialSettings settings
        {
            get
            {
                return SocialSettings.instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void LoadSettings()
        {
            System.Diagnostics.Stopwatch stopWatch = ForieroDebug.CodePerformance ? System.Diagnostics.Stopwatch.StartNew() : null;
            if (initialized)
            {
                Debug.LogError("SOCIAL - SocialSettings has been already loaded!");
                return;
            }
            else
            {
                Debug.Log("SOCIAL - Loading");
                if (settings.initialize)
                {
                    InitFacebook();
                    InitTwitter();
                    initialized = true;
                }
            }
            if(ForieroDebug.CodePerformance) Debug.Log("METHOD STOPWATCH (SocialSettings - AfterSceneLoad): " + stopWatch?.Elapsed.ToString());
        }

#if (!UNITY_ANDROID && !UNITY_STANDALONE_OSX && !UNITY_IOS && !UNITY_WSA) || UNITY_EDITOR || !FORIERO_SOCIALS
        static void InitFacebook()
        {

        }

        static void PostFacebookInternal(string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
        {
            string url = string.IsNullOrEmpty(settings.facebook.facebookURL) ? Foriero.facebook : settings.facebook.facebookURL;
            Debug.Log("SOCIAL - Opening URL : " + url);
            Application.OpenURL(url);
        }

        static void InitTwitter()
        {

        }

        static void PostTwitterInternal(string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
        {
            string url = string.IsNullOrEmpty(settings.twitter.twitterURL) ? Foriero.twitter : settings.twitter.twitterURL;
            Debug.Log("SOCIAL - Opening URL : " + url);
            Application.OpenURL(url);
        }
#endif

        public static void PostFacebook()
        {
            if (settings != null && settings.initialize && initialized)
            {
                PostFacebookInternal(settings.social.projectMessage, settings.social.projectName, settings.social.projectCaption, settings.social.projectURL, settings.facebook.facebookImage);
            }
            else
            {
                Debug.Log("SOCIAL - PostFacebook - Settings are null or not initialized. Opening : " + Foriero.facebook);
                Application.OpenURL(Foriero.facebook);
            }
        }

        public static void PostTwitter()
        {
            if (settings != null && settings.initialize && initialized)
            {
                Debug.Log("Social - PostTwitter");
                PostTwitterInternal(settings.social.projectMessage, settings.social.projectName, settings.social.projectCaption, settings.social.projectURL, settings.twitter.twitterImage);
            }
            else
            {
                Debug.Log("SOCIAL - PostTwitter - Settings are null or not initialized. Opening : " + Foriero.twitter);
                Application.OpenURL(Foriero.twitter);
            }
        }

        static void PostFacebook(string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
        {
            PostFacebook(text, name, caption, link, texture);
        }

        static void PostTwitter(string text, string name = null, string caption = null, string link = null, Texture2D texture = null)
        {
            PostTwitterInternal(text, name, caption, link, texture);
        }

        public static bool SaveTexture2DToPersistentPath(Texture2D texture, string fileName = "image.png")
        {
            bool result = true;
            string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            try
            {
                System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
                Debug.Log("SOCIAl - File saved : " + path);
            }
            catch
            {
                result = false;
                Debug.LogError("SOCIAL - Texture not readable or other error : " + path);
            }

            return result;
        }

        public static string GetFilePersistentPath(string fileName = "image.png")
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, fileName);
            Debug.Log("SOCIAL - File persistent path : " + path);
            return path;
        }
    }
}
