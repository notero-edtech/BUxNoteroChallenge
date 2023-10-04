using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
	{
		[MenuItem("Foriero/Sprites/Swap/To @1x")] public static void SwapSprites1x() => Sprites.SwapTo("@1x");
		[MenuItem("Foriero/Sprites/Swap/To @2x")] public static void SwapSprites2x() => Sprites.SwapTo("@2x");
		[MenuItem("Foriero/Sprites/Swap/To @4x")] public static void SwapSprites4x() => Sprites.SwapTo("@4x");
		
		[MenuItem("Assets/Foriero/Atlas/Current Scene")]
		public static void AtlasCurrentScene()
		{
			//SpriteAtlas sa = Selection.activeObject as SpriteAtlas;

			var srs = GameObject.FindObjectsOfType<SpriteRenderer>();

			List<Sprite> sprites = new List<Sprite>();

			foreach (var sr in srs)
			{
				if (sr.sprite)
				{
					sprites.Add(sr.sprite);
				}
			}

			Selection.objects = sprites.ToArray();
		}

		[MenuItem("Assets/Foriero/Atlas/Current Scene", true)]
		private static bool AtlasCurrentSceneValidation()
		{
			return Selection.activeObject.GetType() == typeof(SpriteAtlas);
		}
	}
}