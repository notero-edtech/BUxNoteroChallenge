using Notero.Unity.UI.VirtualPiano;

namespace Hendrix.Gameplay.Core
{
    public static class HandIdentifier
    {
        public static Handside GetHandsideByTrackIndex(int trackIndex)
        {
            return trackIndex == (int)Handside.Right ? Handside.Right : Handside.Left;
        }
    }
}
