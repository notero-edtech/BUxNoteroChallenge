using System;
using ForieroEngine.Settings;

public partial class GameSettings : Settings<GameSettings>, ISettingsProvider
{
    [Serializable]
    public class PlayersSettings : AbstractSettings<PlayersSettings>
    {
        public override void Init()
        {
            
        }
        
        public int maxPlayers = 100;
        
    }
}
