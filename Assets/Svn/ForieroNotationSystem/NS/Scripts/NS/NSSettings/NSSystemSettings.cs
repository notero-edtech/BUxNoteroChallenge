/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using TMPro;

namespace ForieroEngine.Music.NotationSystem
{
    public enum TextMode { Text, TextMeshPRO }
    public enum CanvasRenderMode { Screen, World }
    public enum LogEnum { None, Log, LogWarning, LogError }

    [CreateAssetMenu(menuName = "NS/Settings/NS System Settings")]
    public class NSSystemSettings : ScriptableObject
    {
        [Header("Time")]
        public float timeLerpSpeed = 10f;

        [Header("Zoom")]
        public float zoom = 1f;
        public float zoomMin = 0.1f;
        public float zoomMax = 2f;

        [Header("Render Modes")]
        public CanvasRenderMode canvasRenderMode = CanvasRenderMode.Screen;
        public TextMode textMode = TextMode.Text;
       
        [Header("Font sizes")]
        [Tooltip("Font base size.")]
        public int smuflFontSize = 70;
        [Tooltip("Text font base size.")]
        public int textFontSize = 25;

        [Header("Fixed Camera")]
        [Tooltip("Allowing horizontal drag movement.")]
        public bool fixedCameraHDragMove = false;
        [Tooltip("Allowing vertical drag movement.")]
        public bool fixedCameraVDragMove = true;

        [Header("Movable Camera")]
        [Tooltip("Allowing horizontal drag movement.")]
        public bool movableCameraHDragMove = true;
        [Tooltip("Allowing vertical drag movement.")]
        public bool movableCameraVDragMove = true;
        [Tooltip("Allowing vertical drag movement.")]
        public float movableCameraLerpSpeed = 10;

        [Header("Colors")]
        public Color normalColor = Color.black;
        public Color selectedColor = Color.green;
        public Color hoverColor = Color.cyan;

        [Header("Playback")]
        public NSPlayback.PlaybackMode playbackMode = NSPlayback.PlaybackMode.Undefined;        
        public NSPlayback.UpdateMode updateMode = NSPlayback.UpdateMode.Update;

        [Header("Playback Rolling")]
        public NSPlayback.NSRollingPlayback.RollingMode rollingMode = NSPlayback.NSRollingPlayback.RollingMode.Undefined;
        public int barlinesPixelShiftX = 50;
        public int barlinesPixelShiftY = 0;

        [Header("Playback Ticker")]
        public NSPlayback.NSTickerPlayback.TickerMode tickerMode = NSPlayback.NSTickerPlayback.TickerMode.Undefined;

        [Header("Hit Zone")]
        [Tooltip("Check passed object.")]
        public bool checkPassed = false;

        public bool colorPassedObjects = false;
        public Color passedColor = Color.red;
        
        [Header("Logging")]
        public LogEnum addingObjectConstraints = LogEnum.LogError;        
    }
}
