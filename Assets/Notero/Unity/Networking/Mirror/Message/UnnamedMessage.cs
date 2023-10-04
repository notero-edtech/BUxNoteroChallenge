using Mirror;
using System.Collections.Generic;

namespace Notero.Unity.Networking.Mirror.Message
{
    public struct UnnamedMessage : TargetableMessage, NetworkMessage
    {
        public string Message;

        public IReadOnlyList<ulong> ReceivedIds => m_TargetIds;
        private IReadOnlyList<ulong> m_TargetIds;

        public UnnamedMessage(string message, IReadOnlyList<ulong> targetIds = null)
        {
            Message = message;
            m_TargetIds = targetIds;
        }

        public UnnamedMessage(string message, ulong targetId)
        {
            Message = message;
            m_TargetIds = new ulong[] { targetId };
        }
    }
}
