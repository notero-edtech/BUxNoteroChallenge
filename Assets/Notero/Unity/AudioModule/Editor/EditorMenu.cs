using UnityEditor;

namespace Notero.Unity.AudioModule.Editor
{
    public static class EditorMenu
    {
        [MenuItem("Notero/Audio Module/Reset Settings")]
        private static void DoSomething()
        {
            AudioPlayer.Instance.ResetSettings();
        }
    }
}
