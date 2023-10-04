using System;
using ForieroEngine.Settings;

public partial class GameSettings : Settings<GameSettings>, ISettingsProvider
{
    [Serializable]
    public class SlotsSettings : AbstractSettings<SlotsSettings>
    {
        public override void Init()
        {
            
        }
        
        public int maxSlots = 5;
    }
}
