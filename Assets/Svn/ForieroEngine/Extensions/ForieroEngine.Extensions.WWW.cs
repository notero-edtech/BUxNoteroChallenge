using UnityEngine.Networking;

namespace ForieroEngine.Extensions
{
	public static partial class ForieroEngineExtensions
	{
#if UNITY_2021_1_OR_NEWER
		public static bool HasError(this UnityWebRequest.Result r) => r
			is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError;
		
		public static bool HasError(this UnityWebRequest r) => r.result
			is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError or UnityWebRequest.Result.DataProcessingError;
#else
		public static bool HasError(this UnityWebRequest r) => r.isHttpError || r.isNetworkError;
#endif
	}
}
