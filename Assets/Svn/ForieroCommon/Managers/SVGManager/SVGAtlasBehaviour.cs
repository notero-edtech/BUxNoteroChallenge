using UnityEngine;
#if UNITY_2018_1_OR_NEWER && VECTOR_GRAPHICS
using Unity.VectorGraphics;
#endif

[RequireComponent(typeof(SpriteRenderer))]
public class SVGAtlasBehaviour : MonoBehaviour
{
	public Material material;

	SpriteRenderer spriteRenderer;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		var sprite = spriteRenderer.sprite;


		var packedSprite = SVGSettings.FindSprite(sprite);

		if (packedSprite)
		{
			material = material == null ? SVGSettings.FindSpriteMaterial(sprite) : material;

			spriteRenderer.sprite = packedSprite;
			spriteRenderer.material = material;
		}
		else
		{
			if (sprite)
			{
#if UNITY_2018_1_OR_NEWER && VECTOR_GRAPHICS
				material = material == null ? new Material(Shader.Find("Sprites/Default")) : material;

				var vectorMaterial = sprite.texture == null ? new Material(Shader.Find("Unlit/Vector")) : new Material(Shader.Find("Unlit/VectorGradient"));
				var width = Mathf.RoundToInt(sprite.bounds.size.x * sprite.pixelsPerUnit);
				var height = Mathf.RoundToInt(sprite.bounds.size.y * sprite.pixelsPerUnit);
				var texture2D = VectorUtils.RenderSpriteToTexture2D(sprite, width, height, vectorMaterial, 8);
				var pivotX = -sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f;
				var pivotY = -sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f;
				sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(pivotX, pivotY), sprite.pixelsPerUnit);
				spriteRenderer.sprite = sprite;
				spriteRenderer.material = material;
#endif
			}
		}
	}
}
