using Mirror;

namespace Notero.Unity.Networking.Mirror
{
    public class TelepathyTransportAdapter : IStandardTransport
    {
        public int ConnectionTimeout
        {
            get => m_Source.ReceiveTimeout;
            set => m_Source.ReceiveTimeout = value;
        }
        public ushort Port
        {
            get => m_Source.port;
            set => m_Source.port = value;
        }

        private TelepathyTransport m_Source;

        public TelepathyTransportAdapter(TelepathyTransport source)
        {
            m_Source = source;
        }
    }
}
