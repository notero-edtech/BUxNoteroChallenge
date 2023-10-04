using System.Collections.Generic;

namespace Notero.Unity.Networking.Mirror.Message
{
    public interface TargetableMessage
    {
        public IReadOnlyList<ulong> ReceivedIds
        {
            get;
        }
    }
}
