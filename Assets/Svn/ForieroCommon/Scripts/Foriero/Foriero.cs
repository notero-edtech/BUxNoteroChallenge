using UnityEngine;

/*
 * This script is dedicated to unify splash screens for all platforms
 * 
 * Resources/skin/ipad_foriero.skin 
 * Resources/skin/iphone_foriero.skin
 * Resources/skin/iphone_retina_foriero.skin 
 * Resources/skin/pc_foriero.skin
*/

namespace ForieroEngine
{
    public static partial class Foriero
    {
        public static bool debug
        {
            get => ForieroDebug.Debug;
            set => ForieroDebug.Debug = value;
        }

        public static bool debugLanguages
        {
            get => ForieroDebug.Languages;
            set => LangSettings.instance.SetDebug(value);
        }

        public static readonly string foriero = "http://www.foriero.com/";
        public static readonly string facebook = "https://www.facebook.com/Foriero";
        public static readonly string twitter = "http://twitter.com/#!/foriero";

        public static string MailTo(string subject, string body = "", string to = "writeus@foriero.com")
        {
            return System.Uri.EscapeUriString("mailto:" + to)
            + "?subject=" + System.Uri.EscapeDataString(subject)
            + "&body=" + System.Uri.EscapeDataString("\n\n" + body + "\n" + Application.platform.ToString() + "\n" + Application.unityVersion);
        }

        public static void WebMail(string subject, string body)
        {
            Application.OpenURL("http://www.foriero.com/pages/mail-page.php?subject=" + System.Uri.EscapeDataString(subject) + "&body=" + System.Uri.EscapeDataString(body));
        }

        public enum ProjectVersion
        {
            NONE,
            FREE,
            LITE,
            PRO
        }
                       
        public static ProjectVersion projectVersion
        {            
            get
            {
#if FORIERO_VERSION_FREE
                return ProjectVersion.FREE;
#elif FORIERO_VERSION_LITE
                return ProjectVersion.LITE;
#elif FORIERO_VERSION_PRO
                return ProjectVersion.PRO;
#else
                return ProjectVersion.NONE;
#endif
            }
        }

        public static string emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                            + "@"
                                            + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

        public static bool IsEmailValid(string email)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(email.Trim(), emailPattern);
        }

        public static Vector2 GetPixelsPerMeter(float distance = 10f, Camera camera = null)
        {
            Vector2 metersPerPixel = GetMetersPerPixel(distance, camera);
            return new Vector2(1f / metersPerPixel.x, 1f / metersPerPixel.y);
        }

        public static Vector2 GetMetersPerPixel(float distance = 10f, Camera camera = null)
        {
            float x = Vector3.Distance(
                          camera ? camera.ScreenToWorldPoint(new Vector3(0, 0, distance)) : Camera.main.ScreenToWorldPoint(new Vector3(0, 0, distance)),
                          camera ? camera.ScreenToWorldPoint(new Vector3(1, 0, distance)) : Camera.main.ScreenToWorldPoint(new Vector3(1, 0, distance)));
            float y = Vector3.Distance(
                          camera ? camera.ScreenToWorldPoint(new Vector3(0, 0, distance)) : Camera.main.ScreenToWorldPoint(new Vector3(0, 0, distance)),
                          camera ? camera.ScreenToWorldPoint(new Vector3(0, 1, distance)) : Camera.main.ScreenToWorldPoint(new Vector3(0, 1, distance)));
            return new Vector2(x, y);
        }

        public static string systemInfo
        {
            get
            {
                if (_systemInfo == "")
                {
                    _systemInfo = GetSystemInfo();
                }
                return _systemInfo;
            }
        }

        private static string _systemInfo = "";

        private static string GetSystemInfo()
        {
            return "deviceModel:                "
            + SystemInfo.deviceModel.ToString() + '\n' +
            "deviceName:                "
            + SystemInfo.deviceName.ToString() + '\n' +
            "deviceUniqueIdentifier:    "
            + SystemInfo.deviceUniqueIdentifier.ToString() + '\n' +
            "graphicsDeviceID:          "
            + SystemInfo.graphicsDeviceID.ToString() + '\n' +
            "graphicsDeviceName:"
            + SystemInfo.graphicsDeviceName.ToString() + '\n' +
            "graphicsDeviceVendor:      "
            + SystemInfo.graphicsDeviceVendor.ToString() + '\n' +
            "graphicsDeviceVendorID:    "
            + SystemInfo.graphicsDeviceVendorID.ToString() + '\n' +
            "graphicsDeviceVersion:     "
            + SystemInfo.graphicsDeviceVersion.ToString() + '\n' +
            "graphicsMemorySize:        "
            + SystemInfo.graphicsMemorySize.ToString() + '\n' +
            "graphicsShaderLevel:       "
            + SystemInfo.graphicsShaderLevel.ToString() + '\n' +
            "operatingSystem:           "
            + SystemInfo.operatingSystem.ToString() + '\n' +
            "processorCount:            "
            + SystemInfo.processorCount.ToString() + '\n' +
            "processorType:             "
            + SystemInfo.processorType.ToString() + '\n' +
            "supportedRenderTargetCount:"
            + SystemInfo.supportedRenderTargetCount.ToString() + '\n' +
            "supportsAccelerometer:     "
            + SystemInfo.supportsAccelerometer.ToString() + '\n' +
            "supportsGyroscope:         "
            + SystemInfo.supportsGyroscope.ToString() + '\n' +            
            "supportsLocationService:   "
            + SystemInfo.supportsLocationService.ToString() + '\n' +
            "supportsShadows:           "
            + SystemInfo.supportsShadows.ToString() + '\n' +
            "supportsVibration:         "
            + SystemInfo.supportsVibration.ToString() + '\n' +
            "systemMemorySize:          "
            + SystemInfo.systemMemorySize.ToString() + '\n' +
            "resolution:                "
            + Screen.width.ToString() + " " + Screen.height.ToString() + '\n' +
            "fullScreen:                "
            + Screen.fullScreen + '\n' +
            "osResolution:              "
            + Screen.currentResolution.width + " " + Screen.currentResolution.height;
        }
    }
}
