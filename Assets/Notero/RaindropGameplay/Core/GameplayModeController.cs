using System;

namespace Notero.RaindropGameplay.Core
{
    /// <summary>
    /// Raindrop gameplay mode[normal, holdon]
    /// </summary>
    [Serializable]
    public enum GameplayMode
    {
        Normal,
        HoldOn,
        LibraryNormal,
        LibraryHoldOn
    }

    public class GameplayModeController
    {
        /// <summary>
        /// Current gameplay mode
        /// </summary>
        public GameplayMode Mode { get; private set; }

        /// <summary>
        /// Set current gameplay mode
        /// </summary>
        /// <param name="mode"></param>
        public void SelectMode(GameplayMode mode)
        {
            Mode = mode;
        }
    }
}