using kcp2k;

namespace Notero.Unity.Networking.Mirror
{
    public class KcpTransportAdapter : IStandardTransport
    {
        public int ConnectionTimeout
        {
            get => m_Source.Timeout;
            set => m_Source.Timeout = value;
        }
        public ushort Port
        {
            get => m_Source.Port;
            set => m_Source.Port = value;
        }

        private KcpTransport m_Source;

        public KcpTransportAdapter(KcpTransport source)
        {
            m_Source = source;
        }
    }
}
