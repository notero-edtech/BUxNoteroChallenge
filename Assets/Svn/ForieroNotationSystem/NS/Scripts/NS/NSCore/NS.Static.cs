/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using System.Collections.Generic;
using ForieroEngine.Music.NotationSystem.Classes;

namespace ForieroEngine.Music.NotationSystem
{
    public abstract partial class NS
    {
        private static NS _instance;

        public static NS instance
        {
            private set => _instance = value;
            get => _instance;
        }

        private static Sprite _wholeNote;
        public static Sprite wholeNote
        {
            get
            {
                if (!_wholeNote) _wholeNote = Resources.Load<Sprite>("smufl/whole");
                return _wholeNote;
            }
        }

        private static Sprite _halfNote;
        public static Sprite halfNote
        {
            get
            {
                if (!_halfNote) _halfNote = Resources.Load<Sprite>("smufl/half");
                return _halfNote;
            }
        }

        private static Sprite _quarterNote;
        public static Sprite quarterNote
        {
            get
            {
                if (!_quarterNote) _quarterNote = Resources.Load<Sprite>("smufl/quarter");
                return _quarterNote;
            }
        }
    }
}
