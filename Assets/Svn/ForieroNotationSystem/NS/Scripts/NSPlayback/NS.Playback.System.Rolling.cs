/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        public static class NSRollingPlayback
        {
            public static void Reset() { _rollingMode = RollingMode.Undefined; _pixelsPerSecond = 200; }
            public static void ResetMeasures() { _pixelsPerSecond = 200; }

            #region RollingMode

            public enum RollingMode { Left, Right, Top, Bottom, Undefined = int.MaxValue }
            private static RollingMode _rollingMode = RollingMode.Undefined;

            public static RollingMode rollingMode
            {
                get => _rollingMode;
                set { _rollingMode = value; OnRollingModeChanged?.Invoke(_rollingMode); }
            }

            public static Action<RollingMode> OnRollingModeChanged;

            #endregion

            private static float _pixelsPerSecond = 200;
            public static float pixelsPerSecond
            {
                get => _pixelsPerSecond * (NSSettingsStatic.canvasRenderMode == CanvasRenderMode.Screen ? _zoom : 1);
                set => _pixelsPerSecond = value;
            }

            public static float pixelPosition
            {
                get => Time.time * pixelsPerSecond;
                set => Time.UpdateTime((value / pixelsPerSecond).Clamp(0, Time.TotalTime));
            }            
        }
    }
}
