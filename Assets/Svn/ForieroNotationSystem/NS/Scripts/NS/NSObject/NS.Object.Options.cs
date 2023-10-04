/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */

namespace ForieroEngine.Music.NotationSystem.Classes
{
    public partial class NSObject
    {
        public interface INSObjectOptions<TSelf>
        {
            void Reset();
            void CopyValuesFrom(TSelf t);
        }
    }
}
