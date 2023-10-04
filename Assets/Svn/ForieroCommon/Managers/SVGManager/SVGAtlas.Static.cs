using System;
#if !UNITY_WEBGL
using UnifiedIO;
#endif
using UnityEngine;
#if UNITY_2018_1_OR_NEWER && VECTOR_GRAPHICS
using Unity.VectorGraphics;
#endif

namespace ForieroEngine.SVG
{
	public partial class SVGAtlas : ScriptableObject
	{
		public static Action<string> OnWatchStartMessage;
		public static Action<string> OnWatchEndMessage;

		static void WatchStartMessage(string message)
		{
			OnWatchStartMessage?.Invoke(message);
		}

		static void WatchEndMessage(long ellapsedTime, string message)
		{
			message = "SVG (" + ellapsedTime.ToString("0000") + ") : " + message;
			if (Foriero.debug) Debug.Log(message);
			OnWatchEndMessage?.Invoke(message);
		}

		static Material vectorMaterial;
		static Material vectorGradientMaterial;

		public static void SaveTexture2D(string fileName, byte[] bytes)
		{
			#if !UNITY_WEBGL
			if (File.Exists(fileName)) File.Delete(fileName);
			File.WriteBytes(fileName, bytes);
			#endif
		}

		public static Texture2D SVGSpriteToTexture2D(Sprite sprite, int sampling = 8)
		{
#if UNITY_2018_1_OR_NEWER && VECTOR_GRAPHICS
			if (!vectorMaterial) vectorMaterial = new Material(Shader.Find("Unlit/Vector"));
			if (!vectorGradientMaterial) vectorGradientMaterial = new Material(Shader.Find("Unlit/VectorGradient"));
			var material = sprite.texture == null ? (vectorMaterial) : vectorGradientMaterial;

			var width = (int)(sprite.bounds.size.x * sprite.pixelsPerUnit);
			var height = (int)(sprite.bounds.size.y * sprite.pixelsPerUnit);
			return VectorUtils.RenderSpriteToTexture2D(sprite, width, height, material, sampling);
#else
            return null;
#endif
        }

		public Vector2 GetSpritePivot(Sprite sprite)
		{
			var pivotX = -sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f;
			var pivotY = -sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f;
			return new Vector2(pivotX, pivotY);
		}
	}
}
