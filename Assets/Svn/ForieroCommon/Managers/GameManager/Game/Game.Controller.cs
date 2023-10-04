using UnityEngine.InputSystem;
#if UNITY_STANDALONE || UNITY_PS3 || UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.XInput;
#elif UNITY_SWITCH
using UnityEngine.InputSystem.Switch;
#endif


public static partial class Game
{

    public enum ControllerEnum
    {
        Touch,
        KeyboardMouse,
        XBox,
        PS,
        Stadia,
        Switch,
        Gamepad,
        Undefined = int.MaxValue
    }

    public static ControllerEnum GetControllerEnum(this InputDevice device)
    {
#if UNITY_STANDALONE || UNITY_PS3 || UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE        
        if (device is XInputController) return ControllerEnum.XBox;
        else if (device is DualShockGamepad) return ControllerEnum.PS;
#elif UNITY_SWITCH
        else if (device is SwitchProControllerHID) return ControllerEnum.Switch;
        else 
#endif
        if (device is Gamepad) return ControllerEnum.Gamepad;
        else return ControllerEnum.KeyboardMouse;
    }

    public enum SwitchControllerEnum
    {
        SwitchPro,
        Undefined = int.MaxValue
    }

    public static SwitchControllerEnum GetSwitchControllerEnum(this InputDevice device)
    {
#if UNITY_SWITCH
        if (device is SwitchProControllerHID) return SwitchControllerEnum.SwitchPro;
#endif
        return SwitchControllerEnum.Undefined;
    }

    public enum DualShockControllerEnum
    {
        PS3,
        PS4,
        PS5,
        Undefined = int.MaxValue
    }

    public static DualShockControllerEnum GetDualShockControllerEnum(this InputDevice device)
    {
#if UNITY_STANDALONE || UNITY_PS3 || UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE    
        if (device is DualShockGamepad)
        {
            if (device is DualShock3GamepadHID) return DualShockControllerEnum.PS3;
            else if (device is DualShock4GamepadHID) return DualShockControllerEnum.PS4;
            //else if (device is DualShock5GamepadHID) return DualShockControllerEnum.PS5;
            
            return DualShockControllerEnum.PS4;
        }
#endif
        return DualShockControllerEnum.Undefined;
    }

    public enum XBoxControllerEnum
    {
        XBoxOne,
        Undefined = int.MaxValue
    }

    public static XBoxControllerEnum GetXBoxControllerEnum(this InputDevice device)
    {
#if UNITY_STANDALONE || UNITY_PS3 || UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE   
        if (device is XInputController)
        {
#if UNITY_STANDALONE_OSX
            if (device is XboxGamepadMacOS) return XBoxControllerEnum.XBoxOne;
            else if (device is XboxOneGampadMacOSWireless) return XBoxControllerEnum.XBoxOne;
#endif
            return XBoxControllerEnum.XBoxOne;
        }
#endif
        return XBoxControllerEnum.Undefined;
    }
}