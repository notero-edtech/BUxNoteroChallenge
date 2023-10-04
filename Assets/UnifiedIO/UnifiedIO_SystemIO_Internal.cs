#if !UNITY_WEBPLAYER && !UNITY_WEBGL
namespace UnifiedIO.SystemIO.Internal
{
	public static class Path
	{
		public static string BaseFolder = UnityEngine.Application.persistentDataPath;

		public static string GetFullPath(string path)
		{
			if (path.StartsWith("/") || path.StartsWith("\\")) {
				path = path.Substring(1);
			}
			return System.IO.Path.Combine(BaseFolder, path);
		}
	}
}
#endif