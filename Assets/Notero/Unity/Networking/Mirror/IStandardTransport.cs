namespace Notero.Unity.Networking.Mirror
{
    public interface IStandardTransport
    {
        /// <summary>
        /// An amount of time in milliseconds to cancel the connection after Connect () has been called
        /// </summary>
        public abstract int ConnectionTimeout
        {
            get; set;
        }

        public ushort Port
        {
            get; set;
        }
    }
}
