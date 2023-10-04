/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System;
using ForieroEngine.MIDIUnified.Interfaces;

namespace ForieroEngine.Music.NotationSystem
{
    public static partial class NSPlayback
    {
        private static SystemEnum _system = SystemEnum.Undefined;
        private static NSSystemsSettings Settings => NSSystemsSettings.instance;

        public static SystemEnum System
        {
            get => _system;
            private set
            {
                _system = value;
                onSystemChanged?.Invoke(_system);
            }
        }

        public static Action<SystemEnum> onSystemChanged;
        
        public static ControllerAlignment ControllerAlignment { get; private set; } = ControllerAlignment.Bottom;

        public static void InitSystem(SystemEnum system, string customSettingsNameId = null)
        {
            NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Undefined;
            NSTickerPlayback.rollingMode = NSTickerPlayback.RollingMode.Undefined;
            NSTickerPlayback.tickerMode = NSTickerPlayback.TickerMode.Undefined;

            switch (system)
            {
                case SystemEnum.Default:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    Settings.defaultSystemSettings.Init();
                    break;
                case SystemEnum.PageLayoutTicker:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    NSTickerPlayback.tickerMode = NSTickerPlayback.TickerMode.PageLayout;
                    Settings.pageLayoutTickerSystemSettings.Init();
                    NSTickerPlayback.tickerMode = NSTickerPlayback.TickerMode.PageLayout;
                    System = system;
                    break;
                case SystemEnum.RollingTicker:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    NSTickerPlayback.tickerMode = NSTickerPlayback.TickerMode.Screen;
                    Settings.tickerSystemSettings.Init();
                    NSTickerPlayback.tickerMode = NSTickerPlayback.TickerMode.Screen;
                    System = system;
                    break;
                case SystemEnum.RollingLeftRight:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Right;
                    Settings.rollingLeftRightSystemSettings.Init();
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Right;
                    System = system;
                    break;
                case SystemEnum.RollingRightLeft:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Left;
                    Settings.rollingLeftRightSystemSettings.Init();
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Left;
                    System = system;
                    break;
                case SystemEnum.RollingTopBottom:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Bottom;
                    Settings.rollingTopBottomSystemSettings.Init();
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Bottom;
                    System = system;
                    break;
                case SystemEnum.RollingBottomTop:
                    ControllerAlignment = ControllerAlignment.Top;
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Top;
                    Settings.rollingTopBottomSystemSettings.Init();
                    NSRollingPlayback.rollingMode = NSRollingPlayback.RollingMode.Top;
                    System = system;
                    break;
                case SystemEnum.Custom:
                    ControllerAlignment = ControllerAlignment.Bottom;
                    InitCustom(customSettingsNameId);
                    System = system;
                    break;
            }
        }

        public static void InitCustom(string customSettingsNameId) { }
    }
}
