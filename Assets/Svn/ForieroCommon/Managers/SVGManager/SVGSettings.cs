using UnityEngine;
using System.Collections.Generic;
using ForieroEngine.SVG;
using ForieroEngine.Settings;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SettingsManager]
public class SVGSettings : Settings<SVGSettings>, ISettingsProvider
{
#if UNITY_EDITOR
	[MenuItem("Foriero/Settings/SVG", false, -1000)] public static void SVGSettingsMenu() => Select();	
#endif
	
	public List<SVGAtlas> atlases = new List<SVGAtlas>();

	public static Sprite FindSprite(Sprite sprite, string atlasName = "")
	{
		foreach (var atlas in instance.atlases)
		{
			int index = atlas.GetSpriteIndex(sprite);
			if (index >= 0 && index < atlas.sprites.Count)
			{
				return atlas.GetSprite(index.ToString("000") + "_" + sprite.name);
			}
		}

		return null;
	}

	public static Material FindSpriteMaterial(Sprite sprite, string atlasName = "")
	{
		foreach (var atlas in instance.atlases)
		{
			int index = atlas.GetSpriteIndex(sprite);
			if (index >= 0 && index < atlas.sprites.Count)
			{
				atlas.packedMaterial = atlas.packedMaterial == null ? new Material(Shader.Find("Sprites/Default")) : atlas.packedMaterial;
				return atlas.packedMaterial;
			}
		}

		return null;
	}
}