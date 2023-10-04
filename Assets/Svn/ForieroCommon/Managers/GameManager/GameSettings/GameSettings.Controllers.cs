using System;
using System.Runtime.CompilerServices;
using ForieroEngine.Settings;

public partial class GameSettings : Settings<GameSettings>, ISettingsProvider
{
    [Serializable]
    public class ControllersSettings : AbstractSettings<ControllersSettings>
    {
        public override void Init()
        {
            
        }

        public static Controller[] Controllers => instance.controllers.controllers;

        public static Action<int> OnControllerChanged;
        public static Action<int> OnAnalogStickDeadZoneChanged;
        public static Action<int> OnVibrationChanged;

        [Serializable]
        public class Controller
        {
            public Game.ControllerEnum controller = Game.ControllerEnum.KeyboardMouse;
            public float analogStickDeadZone = 1f;
            public float vibration = 1f;
        }

        public Controller[] controllers;
    }
}
