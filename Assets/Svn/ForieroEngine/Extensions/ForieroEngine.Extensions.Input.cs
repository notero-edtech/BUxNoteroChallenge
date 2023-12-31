﻿using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ForieroEngine.Extensions
{
    public static partial class ForieroEngineExtensions
    {
        #if ENABLE_INPUT_SYSTEM
        public static Key ToKey(this KeyCode k)
        {
            switch (k)
            {
                case KeyCode.None: return Key.None;
                case KeyCode.Backspace: return Key.Backspace;
                case KeyCode.Delete: return Key.Delete;
                case KeyCode.Tab: return Key.Tab;
                case KeyCode.Clear: return Key.None;
                case KeyCode.Return: return Key.Enter;
                case KeyCode.Pause: return Key.Pause;
                case KeyCode.Escape: return Key.Escape;
                case KeyCode.Space: return Key.Space;
                case KeyCode.Keypad0: return Key.Numpad0;
                case KeyCode.Keypad1: return Key.Numpad1;
                case KeyCode.Keypad2: return Key.Numpad2;
                case KeyCode.Keypad3: return Key.Numpad3;
                case KeyCode.Keypad4: return Key.Numpad4;
                case KeyCode.Keypad5: return Key.Numpad5;
                case KeyCode.Keypad6: return Key.Numpad6;
                case KeyCode.Keypad7: return Key.Numpad7;
                case KeyCode.Keypad8: return Key.Numpad8;
                case KeyCode.Keypad9: return Key.Numpad9;
                case KeyCode.KeypadPeriod: return Key.NumpadPeriod; 
                case KeyCode.KeypadDivide: return Key.NumpadDivide;
                case KeyCode.KeypadMultiply: return Key.NumpadMultiply;
                case KeyCode.KeypadMinus: return Key.NumpadMinus;
                case KeyCode.KeypadPlus: return Key.NumpadPlus;
                case KeyCode.KeypadEnter: return Key.NumpadEnter;
                case KeyCode.KeypadEquals: return Key.NumpadEquals;
                case KeyCode.UpArrow: return Key.UpArrow;
                case KeyCode.DownArrow: return Key.DownArrow;
                case KeyCode.RightArrow: return Key.RightArrow;
                case KeyCode.LeftArrow: return Key.LeftArrow;
                case KeyCode.Insert: return Key.Insert;
                case KeyCode.Home: return Key.Home;
                case KeyCode.End: return Key.End;
                case KeyCode.PageUp: return Key.PageUp;
                case KeyCode.PageDown: return Key.PageDown;
                case KeyCode.F1: return Key.F1;
                case KeyCode.F2: return Key.F2;
                case KeyCode.F3: return Key.F3;
                case KeyCode.F4: return Key.F4;
                case KeyCode.F5: return Key.F5;
                case KeyCode.F6: return Key.F6;
                case KeyCode.F7: return Key.F7;
                case KeyCode.F8: return Key.F8;
                case KeyCode.F9: return Key.F9;
                case KeyCode.F10: return Key.F10;
                case KeyCode.F11: return Key.F11;
                case KeyCode.F12: return Key.F12;
                case KeyCode.F13: return Key.None;
                case KeyCode.F14: return Key.None;
                case KeyCode.F15: return Key.None;
                case KeyCode.Alpha0: return Key.Digit0;
                case KeyCode.Alpha1: return Key.Digit1;
                case KeyCode.Alpha2: return Key.Digit2;
                case KeyCode.Alpha3: return Key.Digit3;
                case KeyCode.Alpha4: return Key.Digit4;
                case KeyCode.Alpha5: return Key.Digit5;
                case KeyCode.Alpha6: return Key.Digit6;
                case KeyCode.Alpha7: return Key.Digit7;
                case KeyCode.Alpha8: return Key.Digit8;
                case KeyCode.Alpha9: return Key.Digit9;
                case KeyCode.Exclaim: return Key.None;
                case KeyCode.DoubleQuote: return Key.None;
                case KeyCode.Hash: return Key.None;
                case KeyCode.Dollar: return Key.None;
                case KeyCode.Percent: return Key.None;
                case KeyCode.Ampersand: return Key.None;
                case KeyCode.Quote: return Key.Quote;
                case KeyCode.LeftParen: return Key.None;
                case KeyCode.RightParen: return Key.None;
                case KeyCode.Asterisk: return Key.None;
                case KeyCode.Plus: return Key.None;
                case KeyCode.Comma: return Key.Comma;
                case KeyCode.Minus: return Key.Minus;
                case KeyCode.Period: return Key.Period;
                case KeyCode.Slash: return Key.Slash;
                case KeyCode.Colon: return Key.None;
                case KeyCode.Semicolon: return Key.Semicolon;
                case KeyCode.Less: return Key.None;
                case KeyCode.Equals: return Key.Equals;
                case KeyCode.Greater: return Key.None;
                case KeyCode.Question: return Key.None;
                case KeyCode.At: return Key.None;
                case KeyCode.LeftBracket: return Key.LeftBracket;
                case KeyCode.Backslash: return Key.Backslash;
                case KeyCode.RightBracket: return Key.RightBracket;
                case KeyCode.Caret: return Key.None;
                case KeyCode.Underscore: return Key.None;
                case KeyCode.BackQuote: return Key.Backquote;
                case KeyCode.A: return Key.A;
                case KeyCode.B: return Key.B;
                case KeyCode.C: return Key.C;
                case KeyCode.D: return Key.D;
                case KeyCode.E: return Key.E;
                case KeyCode.F: return Key.F;
                case KeyCode.G: return Key.G;
                case KeyCode.H: return Key.H;
                case KeyCode.I: return Key.I;
                case KeyCode.J: return Key.J;
                case KeyCode.K: return Key.K;
                case KeyCode.L: return Key.L;
                case KeyCode.M: return Key.M;
                case KeyCode.N: return Key.N;
                case KeyCode.O: return Key.O;
                case KeyCode.P: return Key.P;
                case KeyCode.Q: return Key.Q;
                case KeyCode.R: return Key.R;
                case KeyCode.S: return Key.S;
                case KeyCode.T: return Key.T;
                case KeyCode.U: return Key.U;
                case KeyCode.V: return Key.V;
                case KeyCode.W: return Key.W;
                case KeyCode.X: return Key.X;
                case KeyCode.Y: return Key.Y;
                case KeyCode.Z: return Key.Z;
                case KeyCode.LeftCurlyBracket: return Key.None;
                case KeyCode.Pipe: return Key.None;
                case KeyCode.RightCurlyBracket: return Key.None;
                case KeyCode.Tilde: return Key.None;
                case KeyCode.Numlock: return Key.NumLock;
                case KeyCode.CapsLock: return Key.CapsLock;
                case KeyCode.ScrollLock: return Key.ScrollLock;
                case KeyCode.RightShift: return Key.RightShift;
                case KeyCode.LeftShift: return Key.LeftShift;
                case KeyCode.RightControl: return Key.RightCtrl;
                case KeyCode.LeftControl: return Key.LeftCtrl;
                case KeyCode.RightAlt: return Key.RightAlt;
                case KeyCode.LeftAlt: return Key.LeftAlt;
                #if UNITY_2021_1_OR_NEWER
                case KeyCode.LeftMeta: return Key.LeftMeta;
                #endif
                case KeyCode.LeftWindows: return Key.LeftWindows;
                #if UNITY_2021_1_OR_NEWER
                case KeyCode.RightMeta: return Key.RightMeta;
                #endif
                case KeyCode.RightWindows: return Key.RightWindows;
                case KeyCode.AltGr: return Key.AltGr;
                case KeyCode.Help: return Key.None;
                case KeyCode.Print: return Key.PrintScreen;
                case KeyCode.SysReq: return Key.None;
                case KeyCode.Break: return Key.None;
                case KeyCode.Menu: return Key.ContextMenu;
                case KeyCode.Mouse0: 
                case KeyCode.Mouse1:
                case KeyCode.Mouse2:
                case KeyCode.Mouse3:
                case KeyCode.Mouse4:
                case KeyCode.Mouse5:
                case KeyCode.Mouse6:
                case KeyCode.JoystickButton0:
                case KeyCode.JoystickButton1:
                case KeyCode.JoystickButton2:
                case KeyCode.JoystickButton3:
                case KeyCode.JoystickButton4:
                case KeyCode.JoystickButton5:
                case KeyCode.JoystickButton6:
                case KeyCode.JoystickButton7:
                case KeyCode.JoystickButton8:
                case KeyCode.JoystickButton9:
                case KeyCode.JoystickButton10:
                case KeyCode.JoystickButton11:
                case KeyCode.JoystickButton12:
                case KeyCode.JoystickButton13:
                case KeyCode.JoystickButton14:
                case KeyCode.JoystickButton15:
                case KeyCode.JoystickButton16:
                case KeyCode.JoystickButton17:
                case KeyCode.JoystickButton18:
                case KeyCode.JoystickButton19:
                case KeyCode.Joystick1Button0:
                case KeyCode.Joystick1Button1:
                case KeyCode.Joystick1Button2:
                case KeyCode.Joystick1Button3:
                case KeyCode.Joystick1Button4:
                case KeyCode.Joystick1Button5:
                case KeyCode.Joystick1Button6:
                case KeyCode.Joystick1Button7:
                case KeyCode.Joystick1Button8:
                case KeyCode.Joystick1Button9:
                case KeyCode.Joystick1Button10:
                case KeyCode.Joystick1Button11:
                case KeyCode.Joystick1Button12:
                case KeyCode.Joystick1Button13:
                case KeyCode.Joystick1Button14:
                case KeyCode.Joystick1Button15:
                case KeyCode.Joystick1Button16:
                case KeyCode.Joystick1Button17:
                case KeyCode.Joystick1Button18:
                case KeyCode.Joystick1Button19:
                case KeyCode.Joystick2Button0:
                case KeyCode.Joystick2Button1:
                case KeyCode.Joystick2Button2:
                case KeyCode.Joystick2Button3:
                case KeyCode.Joystick2Button4:
                case KeyCode.Joystick2Button5:
                case KeyCode.Joystick2Button6:
                case KeyCode.Joystick2Button7:
                case KeyCode.Joystick2Button8:
                case KeyCode.Joystick2Button9:
                case KeyCode.Joystick2Button10:
                case KeyCode.Joystick2Button11:
                case KeyCode.Joystick2Button12:
                case KeyCode.Joystick2Button13:
                case KeyCode.Joystick2Button14:
                case KeyCode.Joystick2Button15:
                case KeyCode.Joystick2Button16:
                case KeyCode.Joystick2Button17:
                case KeyCode.Joystick2Button18:
                case KeyCode.Joystick2Button19:
                case KeyCode.Joystick3Button0:
                case KeyCode.Joystick3Button1:
                case KeyCode.Joystick3Button2:
                case KeyCode.Joystick3Button3:
                case KeyCode.Joystick3Button4:
                case KeyCode.Joystick3Button5:
                case KeyCode.Joystick3Button6:
                case KeyCode.Joystick3Button7:
                case KeyCode.Joystick3Button8:
                case KeyCode.Joystick3Button9:
                case KeyCode.Joystick3Button10:
                case KeyCode.Joystick3Button11:
                case KeyCode.Joystick3Button12:
                case KeyCode.Joystick3Button13:
                case KeyCode.Joystick3Button14:
                case KeyCode.Joystick3Button15:
                case KeyCode.Joystick3Button16:
                case KeyCode.Joystick3Button17:
                case KeyCode.Joystick3Button18:
                case KeyCode.Joystick3Button19:
                case KeyCode.Joystick4Button0:
                case KeyCode.Joystick4Button1:
                case KeyCode.Joystick4Button2:
                case KeyCode.Joystick4Button3:
                case KeyCode.Joystick4Button4:
                case KeyCode.Joystick4Button5:
                case KeyCode.Joystick4Button6:
                case KeyCode.Joystick4Button7:
                case KeyCode.Joystick4Button8:
                case KeyCode.Joystick4Button9:
                case KeyCode.Joystick4Button10:
                case KeyCode.Joystick4Button11:
                case KeyCode.Joystick4Button12:
                case KeyCode.Joystick4Button13:
                case KeyCode.Joystick4Button14:
                case KeyCode.Joystick4Button15:
                case KeyCode.Joystick4Button16:
                case KeyCode.Joystick4Button17:
                case KeyCode.Joystick4Button18:
                case KeyCode.Joystick4Button19:
                case KeyCode.Joystick5Button0:
                case KeyCode.Joystick5Button1:
                case KeyCode.Joystick5Button2:
                case KeyCode.Joystick5Button3:
                case KeyCode.Joystick5Button4:
                case KeyCode.Joystick5Button5:
                case KeyCode.Joystick5Button6:
                case KeyCode.Joystick5Button7:
                case KeyCode.Joystick5Button8:
                case KeyCode.Joystick5Button9:
                case KeyCode.Joystick5Button10:
                case KeyCode.Joystick5Button11:
                case KeyCode.Joystick5Button12:
                case KeyCode.Joystick5Button13:
                case KeyCode.Joystick5Button14:
                case KeyCode.Joystick5Button15:
                case KeyCode.Joystick5Button16:
                case KeyCode.Joystick5Button17:
                case KeyCode.Joystick5Button18:
                case KeyCode.Joystick5Button19:
                case KeyCode.Joystick6Button0:
                case KeyCode.Joystick6Button1:
                case KeyCode.Joystick6Button2:
                case KeyCode.Joystick6Button3:
                case KeyCode.Joystick6Button4:
                case KeyCode.Joystick6Button5:
                case KeyCode.Joystick6Button6:
                case KeyCode.Joystick6Button7:
                case KeyCode.Joystick6Button8:
                case KeyCode.Joystick6Button9:
                case KeyCode.Joystick6Button10:
                case KeyCode.Joystick6Button11:
                case KeyCode.Joystick6Button12:
                case KeyCode.Joystick6Button13:
                case KeyCode.Joystick6Button14:
                case KeyCode.Joystick6Button15:
                case KeyCode.Joystick6Button16:
                case KeyCode.Joystick6Button17:
                case KeyCode.Joystick6Button18:
                case KeyCode.Joystick6Button19:
                case KeyCode.Joystick7Button0:
                case KeyCode.Joystick7Button1:
                case KeyCode.Joystick7Button2:
                case KeyCode.Joystick7Button3:
                case KeyCode.Joystick7Button4:
                case KeyCode.Joystick7Button5:
                case KeyCode.Joystick7Button6:
                case KeyCode.Joystick7Button7:
                case KeyCode.Joystick7Button8:
                case KeyCode.Joystick7Button9:
                case KeyCode.Joystick7Button10:
                case KeyCode.Joystick7Button11:
                case KeyCode.Joystick7Button12:
                case KeyCode.Joystick7Button13:
                case KeyCode.Joystick7Button14:
                case KeyCode.Joystick7Button15:
                case KeyCode.Joystick7Button16:
                case KeyCode.Joystick7Button17:
                case KeyCode.Joystick7Button18:
                case KeyCode.Joystick7Button19:
                case KeyCode.Joystick8Button0:
                case KeyCode.Joystick8Button1:
                case KeyCode.Joystick8Button2:
                case KeyCode.Joystick8Button3:
                case KeyCode.Joystick8Button4:
                case KeyCode.Joystick8Button5:
                case KeyCode.Joystick8Button6:
                case KeyCode.Joystick8Button7:
                case KeyCode.Joystick8Button8:
                case KeyCode.Joystick8Button9:
                case KeyCode.Joystick8Button10:
                case KeyCode.Joystick8Button11:
                case KeyCode.Joystick8Button12:
                case KeyCode.Joystick8Button13:
                case KeyCode.Joystick8Button14:
                case KeyCode.Joystick8Button15:
                case KeyCode.Joystick8Button16:
                case KeyCode.Joystick8Button17:
                case KeyCode.Joystick8Button18:
                case KeyCode.Joystick8Button19: return Key.None;
            }

            return Key.None;
        }
        #endif
    }
}