using kcp2k;
using Mirror;
using System;

namespace Notero.Unity.Networking.Mirror
{
    public static class StandardTransportFactory
    {
        public static IStandardTransport Create(object original)
        {
            IStandardTransport product = null;
            if(original is KcpTransport)
            {
                product = new KcpTransportAdapter(original as KcpTransport);
            }
            else if(original is TelepathyTransport)
            {
                product = new TelepathyTransportAdapter(original as TelepathyTransport);
            }
            else
            {
                throw new NotImplementedException($"Create IStandardTransport from type({original.GetType()}) not implemented!");
            }

            return product;
        }
    }
}
