#if NETFX_CORE
namespace UnifiedIO.WSAAsync.Internal
{
	public static class Path
	{
		public static Windows.Storage.StorageFolder BaseFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
	}
}
#endif