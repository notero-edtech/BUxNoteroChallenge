using System;
using System.Collections;
using System.Collections.Generic;
using DaVikingCode.AssetPacker;
using DaVikingCode.RectanglePacking;
using ForieroEngine.Threading.Unity;
#if !UNITY_WEBGL
using UnifiedIO;
#endif
using UnityEngine;

namespace ForieroEngine.SVG
{
	public partial class SVGAtlas : ScriptableObject
	{
		string cachePath { get { return "SVG/" + id + "/"; } }
		string cacheVersionPath { get { return cachePath + version.ToString() + "/"; } }
		string cacheVersionSpritesPath { get { return cacheVersionPath + "Sprites/"; } }

		public int GetSpriteIndex(Sprite sprite)
		{
			return sprites.IndexOf(sprite);
		}

		public Sprite GetSprite(string id)
		{
			Sprite sprite = null;

			packedSprites.TryGetValue(id, out sprite);

			return sprite;
		}

		public Sprite[] GetSprites(string prefix)
		{
			List<string> spriteNames = new List<string>();
			foreach (var asset in packedSprites)
			{
				if (asset.Key.StartsWith(prefix)) spriteNames.Add(asset.Key);
			}

			spriteNames.Sort(StringComparer.Ordinal);

			List<Sprite> sprites = new List<Sprite>();
			Sprite sprite;
			for (int i = 0; i < spriteNames.Count; ++i)
			{
				packedSprites.TryGetValue(spriteNames[i], out sprite);
				sprites.Add(sprite);
			}

			return sprites.ToArray();
		}

		public void Pack(bool load, Action finished)
		{
			MainThreadDispatcher.Instance.StartCoroutine(PackEnumerator(load, finished));
		}

		IEnumerator LoadSpritesEnumerator(TextureAssets textureAssets, Texture2D packedTexture)
		{
			WatchStart("Creating sprites");
			foreach (TextureAsset textureAsset in textureAssets.assets)
			{
				packedSprites.Add(textureAsset.name, Sprite.Create(packedTexture, new Rect(textureAsset.x, textureAsset.y, textureAsset.width, textureAsset.height), textureAsset.pivot, textureAsset.pixelsToUnit, 0, SpriteMeshType.FullRect));
			}
			WatchStop("Sprites created for texture");
			yield break;
		}

		IEnumerator LoadEnumerator(Action finished)
		{
			#if !UNITY_WEBGL
			if (packedSprites != null && packedSprites.Count > 0)
			{
				finished?.Invoke();
				yield break;
			}

			int numFiles = Directory.GetFiles(cacheVersionPath).Length;

			for (int i = 0; i < numFiles / 2; ++i)
			{
				string png = cacheVersionPath + "data" + i + ".png";

				WatchStart("Creating texture : " + png);
				Texture2D packedTexture = new Texture2D(2, 2);
				packedTexture.LoadImage(File.ReadBytes(png));
				WatchStop("Texture loaded from persistent path : " + png);
				yield return null;

				string json = cacheVersionPath + "data" + i + ".json";
				WatchStart("Creating texture asset : " + json);
				TextureAssets textureAssets = JsonUtility.FromJson<TextureAssets>("");
				textureAssets = JsonUtility.FromJson<TextureAssets>(File.ReadText(json));
				WatchStop("TextureAssets loaded from persisten path : " + json);

				yield return LoadSpritesEnumerator(textureAssets, packedTexture);

				// message here //

				yield return null;
			}
			#else
			yield return null;
			#endif
			finished?.Invoke();
		}

		public void Release()
		{
			if (packedSprites != null)
			{
				foreach (var asset in packedSprites)
				{
					Destroy(asset.Value.texture);
				}

				packedSprites.Clear();
			}
		}

		IEnumerator PackEnumerator(bool load, Action finished)
		{
			#if !UNITY_WEBGL
			if (packedSprites != null && packedSprites.Count > 0)
			{
				finished?.Invoke();
				yield break;
			}

			if (!cache && Directory.Exists(cachePath)) Directory.Delete(cachePath);

			if (Directory.Exists(cacheVersionPath))
			{
				if (load)
				{
					yield return LoadEnumerator(finished);
				}

				finished?.Invoke();
				yield break;
			};

			if (deletePreviousCacheVersion && Directory.Exists(cachePath)) Directory.Delete(cachePath);

			Directory.Create(cachePath);
			Directory.Create(cacheVersionPath);
			Directory.Create(cacheVersionSpritesPath);

			List<Texture2D> textures = new List<Texture2D>();
			List<string> images = new List<string>();
			List<Vector2> pivots = new List<Vector2>();
			List<float> pixelsToUnits = new List<float>();

			int index = 0;
			WatchStart("Generating svg sprite textures");
			foreach (var sprite in sprites)
			{
				string spriteName = index.ToString("000") + "_" + sprite.name;
				var texture = SVGSpriteToTexture2D(sprite);

				textures.Add(texture);

				images.Add(spriteName);
				pivots.Add(GetSpritePivot(sprite));
				pixelsToUnits.Add(sprite.pixelsPerUnit);

				if (saveSprites)
				{
					SaveTexture2D(cacheVersionSpritesPath + spriteName + ".png", texture.EncodeToPNG());
				}
				index++;

			}
			WatchStop("Svg sprites converted to Png sprites");
			yield return null;

			List<Rect> rectangles = new List<Rect>();
			for (int i = 0; i < textures.Count; i++)
			{
				if (textures[i].width > textureSize || textures[i].height > textureSize)
				{
					throw new Exception("A texture size is bigger than the sprite sheet size!");
				}
				else
				{
					rectangles.Add(new Rect(0, 0, textures[i].width, textures[i].height));
				}
			}

			const int padding = 1;


			int numSpriteSheet = 0;
			while (rectangles.Count > 0)
			{
				var packedTexture = new Texture2D(textureSize, textureSize, TextureFormat.ARGB32, false);
				Color32[] fillColor = packedTexture.GetPixels32();
				for (int i = 0; i < fillColor.Length; ++i)
				{
					fillColor[i] = Color.clear;
				}

				RectanglePacker packer = new RectanglePacker(packedTexture.width, packedTexture.height, padding);

				for (int i = 0; i < rectangles.Count; i++)
				{
					packer.insertRectangle((int)rectangles[i].width, (int)rectangles[i].height, i);
				}

				packer.packRectangles();

				if (packer.rectangleCount > 0)
				{
					WatchStart("Packing textures");
					packedTexture.SetPixels32(fillColor);
					IntegerRectangle rect = new IntegerRectangle();
					List<TextureAsset> textureAssets = new List<TextureAsset>();

					List<Rect> garbageRect = new List<Rect>();
					List<Texture2D> garabeTextures = new List<Texture2D>();
					List<string> garbageImages = new List<string>();
					List<Vector2> garbagePivots = new List<Vector2>();
					List<float> garbagePixelsToUnits = new List<float>();

					for (int j = 0; j < packer.rectangleCount; j++)
					{
						rect = packer.getRectangle(j, rect);

						index = packer.getRectangleId(j);

						packedTexture.SetPixels32(rect.x, rect.y, rect.width, rect.height, textures[index].GetPixels32());

						TextureAsset textureAsset = new TextureAsset();
						textureAsset.x = rect.x;
						textureAsset.y = rect.y;
						textureAsset.width = rect.width;
						textureAsset.height = rect.height;
						textureAsset.name = images[index];
						textureAsset.pivot = pivots[index];
						textureAsset.pixelsToUnit = pixelsToUnits[index];

						textureAssets.Add(textureAsset);

						garbageRect.Add(rectangles[index]);
						garabeTextures.Add(textures[index]);
						garbageImages.Add(images[index]);
						garbagePivots.Add(pivots[index]);
						garbagePixelsToUnits.Add(pixelsToUnits[index]);
					}

					foreach (Rect garbage in garbageRect) rectangles.Remove(garbage);

					foreach (Texture2D garbage in garabeTextures) textures.Remove(garbage);

					foreach (string garbage in garbageImages) images.Remove(garbage);

					packedTexture.Apply();

					string pngFile = cacheVersionPath + "data" + numSpriteSheet + ".png";
					File.WriteBytes(pngFile, packedTexture.EncodeToPNG());

					string jsonFile = cacheVersionPath + "data" + numSpriteSheet + ".json";
					File.WriteText(jsonFile, JsonUtility.ToJson(new TextureAssets(textureAssets.ToArray())));
					++numSpriteSheet;

					WatchStop("Page packed and saved : " + pngFile);
					yield return null;

					if (load)
					{
						TextureAssets ta = new TextureAssets(textureAssets.ToArray());
						yield return LoadSpritesEnumerator(ta, packedTexture);
					}
					else
					{
						Destroy(packedTexture);
					}
				}
			}
			#else
			yield return null;
			#endif
			finished?.Invoke();
		}
	}
}
