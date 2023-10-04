using Mirror;

namespace Notero.Unity.Networking.Mirror.Example
{
    public struct CountdownMessage : NetworkMessage
    {
        public double StartTime;
        public double StartNumber;
    }
}
