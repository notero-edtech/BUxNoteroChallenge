/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections;
using System.Collections.Generic;

namespace ForieroEngine.Music.Training
{
    public static partial class TL
    {
        static Settings _settings = null;

        public static Settings settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }
                return _settings;

            }
            set
            {
                _settings = value;
            }
        }
    }
}
