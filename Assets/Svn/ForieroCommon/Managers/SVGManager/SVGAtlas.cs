using System.Collections.Generic;
using UnityEngine;

namespace ForieroEngine.SVG
{
	public partial class SVGAtlas : ScriptableObject
	{
		public enum TextureSize
		{
			s1024 = 1024,
			s2048 = 2048,
			s4096 = 4096,
			s8192 = 8192
		}

		public string id = "";
		public TextureSize size = TextureSize.s2048;
		int textureSize { get { return (int)size; } }
		public int version = 0;
		public bool cache = true;
		public bool deletePreviousCacheVersion = true;
		public bool saveSprites = false;

		public bool keepInMemory = false;
		public bool loadOnOnset = false;

		public List<Sprite> sprites = new List<Sprite>();

		[HideInInspector]
		public Dictionary<string, Sprite> packedSprites = new Dictionary<string, Sprite>();
		public Material packedMaterial;

		System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

		void WatchStart(string message)
		{
			stopwatch.Reset();
			stopwatch.Start();
			WatchStartMessage(message);
		}

		void WatchStop(string message)
		{
			stopwatch.Stop();
			WatchEndMessage(stopwatch.ElapsedMilliseconds, message);
		}
	}
}
