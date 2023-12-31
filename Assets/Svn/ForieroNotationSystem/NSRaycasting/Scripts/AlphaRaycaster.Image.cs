/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

public partial class AlphaRaycaster : GraphicRaycaster
{
    private Rect GetImageTextureRect(Image objImage)
    {
        Rect texRect;

        // Case for sprites with redundant transparent areas (Unity trims them internally, so we have to handle that)
        if (objImage.sprite.textureRectOffset.sqrMagnitude > 0)
        {
            texRect = objImage.sprite.packed ? new Rect(objImage.sprite.textureRect.xMin - objImage.sprite.textureRectOffset.x,
            objImage.sprite.textureRect.yMin - objImage.sprite.textureRectOffset.y,
            objImage.sprite.textureRect.width + objImage.sprite.textureRectOffset.x * 2f,
            objImage.sprite.textureRect.height + objImage.sprite.textureRectOffset.y * 2f) : objImage.sprite.rect;
        }
        else texRect = objImage.sprite.textureRect;

        return texRect;
    }

    // Return true if alpha check for image is positive (need to exclude the result).
    private bool AlphaCheckImage(GameObject obj, Image objImage, AlphaRaycasterCheck objAlphaCheck, Vector2 pointerPos, OuterEdgeDetection mode)
    {
        if (mode == OuterEdgeDetection.NativeRect)
        {
            return false;
        }


        var objTrs = obj.transform as RectTransform;
        var pointerLPos = ScreenToLocalObjectPosition(pointerPos, objTrs);
        var objTex = objImage.mainTexture as Texture2D;

        var texRect = GetImageTextureRect(objImage);
        var objSize = objTrs.rect.size;

        
        // Correcting objSize in case "preserve aspect" is enabled 
        if (objImage.preserveAspect)
        {
            if (objSize.x < objSize.y) objSize.y = objSize.x * (texRect.height / texRect.width);
            else objSize.x = objSize.y * (texRect.width / texRect.height);

            // Also we need to cut off empty object space
            var halfPivot = new Vector2(Mathf.Abs(objTrs.pivot.x) == .5f ? 2 : 1, Mathf.Abs(objTrs.pivot.y) == .5f ? 2 : 1);
            if (Mathf.Abs(pointerLPos.x * halfPivot.x) > objSize.x || Mathf.Abs(pointerLPos.y * halfPivot.y) > objSize.y) return true;
        }

        // Evaluating texture coordinates of the targeted spot
        float texCorX = pointerLPos.x + objSize.x * objTrs.pivot.x;
        float texCorY = pointerLPos.y + objSize.y * objTrs.pivot.y;

        #region	TILED_SLICED
        // Will be used if image has a border
        var borderTotalWidth = objImage.sprite.border.x + objImage.sprite.border.z;
        var borderTotalHeight = objImage.sprite.border.y + objImage.sprite.border.w;
        var fillRect = new Rect(objImage.sprite.border.x, objImage.sprite.border.y,
            Mathf.Clamp(objSize.x - borderTotalWidth, 0f, Mathf.Infinity),
            Mathf.Clamp(objSize.y - borderTotalHeight, 0f, Mathf.Infinity));
        var isInsideFillRect = objImage.hasBorder && fillRect.Contains(new Vector2(texCorX, texCorY));

        // Correcting texture coordinates in case image is tiled
        if (objImage.type == Image.Type.Tiled)
        {
            if (isInsideFillRect)
            {
                if (!objImage.fillCenter) return true;

                texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) % (texRect.width - borderTotalWidth);
                texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) % (texRect.height - borderTotalHeight);
            }
            else if (objImage.hasBorder)
            {
                // If objSize is below border size the border areas will shrink
                texCorX *= Mathf.Clamp(borderTotalWidth / objSize.x, 1f, Mathf.Infinity);
                texCorY *= Mathf.Clamp(borderTotalHeight / objSize.y, 1f, Mathf.Infinity);

                if (texCorX > texRect.width - objImage.sprite.border.z && texCorX < objImage.sprite.border.x + fillRect.width)
                    texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) % (texRect.width - borderTotalWidth);
                else if (texCorX > objImage.sprite.border.x + fillRect.width)
                    texCorX = texCorX - fillRect.width + texRect.width - borderTotalWidth;

                if (texCorY > texRect.height - objImage.sprite.border.w && texCorY < objImage.sprite.border.y + fillRect.height)
                    texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) % (texRect.height - borderTotalHeight);
                else if (texCorY > objImage.sprite.border.y + fillRect.height)
                    texCorY = texCorY - fillRect.height + texRect.height - borderTotalHeight;
            }
            else
            {
                if (texCorX > texRect.width) texCorX %= texRect.width;
                if (texCorY > texRect.height) texCorY %= texRect.height;
            }
        }
        // Correcting texture coordinates in case image is sliced
        else if (objImage.type == Image.Type.Sliced)
        {
            if (isInsideFillRect)
            {
                if (!objImage.fillCenter) return true;

                texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) * ((texRect.width - borderTotalWidth) / fillRect.width);
                texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) * ((texRect.height - borderTotalHeight) / fillRect.height);
            }
            else
            {
                // If objSize is below border size the border areas will shrink
                texCorX *= Mathf.Clamp(borderTotalWidth / objSize.x, 1f, Mathf.Infinity);
                texCorY *= Mathf.Clamp(borderTotalHeight / objSize.y, 1f, Mathf.Infinity);

                if (texCorX > objImage.sprite.border.x && texCorX < objImage.sprite.border.x + fillRect.width)
                    texCorX = objImage.sprite.border.x + (texCorX - objImage.sprite.border.x) * ((texRect.width - borderTotalWidth) / fillRect.width);
                else if (texCorX > objImage.sprite.border.x + fillRect.width)
                    texCorX = texCorX - fillRect.width + texRect.width - borderTotalWidth;

                if (texCorY > objImage.sprite.border.y && texCorY < objImage.sprite.border.y + fillRect.height)
                    texCorY = objImage.sprite.border.y + (texCorY - objImage.sprite.border.y) * ((texRect.height - borderTotalHeight) / fillRect.height);
                else if (texCorY > objImage.sprite.border.y + fillRect.height)
                    texCorY = texCorY - fillRect.height + texRect.height - borderTotalHeight;
            }
        }
        #endregion
        // Correcting texture coordinates by scale in case simple or filled image
        else
        {
            texCorX *= texRect.width / objSize.x;
            texCorY *= texRect.height / objSize.y;
        }

        // For filled images, check if targeted spot is outside of the filled area
        #region FILLED
        if (objImage.type == Image.Type.Filled)
        {
            var nCorX = texRect.height > texRect.width ? texCorX * (texRect.height / texRect.width) : texCorX;
            var nCorY = texRect.width > texRect.height ? texCorY * (texRect.width / texRect.height) : texCorY;
            var nWidth = texRect.height > texRect.width ? texRect.height : texRect.width;
            var nHeight = texRect.width > texRect.height ? texRect.width : texRect.height;

            if (objImage.fillMethod == Image.FillMethod.Horizontal)
            {
                if (objImage.fillOrigin == (int)Image.OriginHorizontal.Left && texCorX / texRect.width > objImage.fillAmount) return true;
                if (objImage.fillOrigin == (int)Image.OriginHorizontal.Right && texCorX / texRect.width < (1 - objImage.fillAmount)) return true;
            }

            if (objImage.fillMethod == Image.FillMethod.Vertical)
            {
                if (objImage.fillOrigin == (int)Image.OriginVertical.Bottom && texCorY / texRect.height > objImage.fillAmount) return true;
                if (objImage.fillOrigin == (int)Image.OriginVertical.Top && texCorY / texRect.height < (1 - objImage.fillAmount)) return true;
            }

            #region RADIAL_90
            if (objImage.fillMethod == Image.FillMethod.Radial90)
            {
                if (objImage.fillOrigin == (int)Image.Origin90.BottomLeft)
                {
                    if (objImage.fillClockwise && Mathf.Atan(nCorY / nCorX) / (Mathf.PI / 2) < (1 - objImage.fillAmount)) return true;
                    if (!objImage.fillClockwise && Mathf.Atan(nCorY / nCorX) / (Mathf.PI / 2) > objImage.fillAmount) return true;
                }

                if (objImage.fillOrigin == (int)Image.Origin90.TopLeft)
                {
                    if (objImage.fillClockwise && nCorY < -(1 / Mathf.Tan((1 - objImage.fillAmount) * Mathf.PI / 2)) * nCorX + nHeight) return true;
                    if (!objImage.fillClockwise && nCorY > -(1 / Mathf.Tan(objImage.fillAmount * Mathf.PI / 2)) * nCorX + nHeight) return true;
                }

                if (objImage.fillOrigin == (int)Image.Origin90.TopRight)
                {
                    if (objImage.fillClockwise && nCorY > Mathf.Tan((1 - objImage.fillAmount) * Mathf.PI / 2) * (nCorX - nWidth) + nHeight) return true;
                    if (!objImage.fillClockwise && nCorY < Mathf.Tan(objImage.fillAmount * Mathf.PI / 2) * (nCorX - nWidth) + nHeight) return true;
                }

                if (objImage.fillOrigin == (int)Image.Origin90.BottomRight)
                {
                    if (objImage.fillClockwise && nCorY > (1 / Mathf.Tan((1 - objImage.fillAmount) * Mathf.PI / 2)) * (nWidth - nCorX)) return true;
                    if (!objImage.fillClockwise && nCorY < (1 / Mathf.Tan(objImage.fillAmount * Mathf.PI / 2)) * (nWidth - nCorX)) return true;
                }
            }
            #endregion

            #region RADIAL_180
            if (objImage.fillMethod == Image.FillMethod.Radial180)
            {
                if (objImage.fillOrigin == (int)Image.Origin180.Bottom)
                {
                    if (objImage.fillClockwise && Mathf.Atan2(nCorY, 2 * (nCorX - nWidth / 2)) < (1 - objImage.fillAmount) * Mathf.PI) return true;
                    if (!objImage.fillClockwise && Mathf.Atan2(texCorY, 2 * (nCorX - nWidth / 2)) > objImage.fillAmount * Mathf.PI) return true;
                }

                if (objImage.fillOrigin == (int)Image.Origin180.Left)
                {
                    if (objImage.fillClockwise && Mathf.Atan2(nCorX, -2 * (nCorY - nHeight / 2)) < (1 - objImage.fillAmount) * Mathf.PI) return true;
                    if (!objImage.fillClockwise && Mathf.Atan2(nCorX, -2 * (nCorY - nHeight / 2)) > objImage.fillAmount * Mathf.PI) return true;
                }

                if (objImage.fillOrigin == (int)Image.Origin180.Top)
                {
                    if (objImage.fillClockwise && Mathf.Atan2(nHeight - nCorY, -2 * (nCorX - nWidth / 2)) < (1 - objImage.fillAmount) * Mathf.PI) return true;
                    if (!objImage.fillClockwise && Mathf.Atan2(nHeight - nCorY, -2 * (nCorX - nWidth / 2)) > objImage.fillAmount * Mathf.PI) return true;
                }

                if (objImage.fillOrigin == (int)Image.Origin180.Right)
                {
                    if (objImage.fillClockwise && Mathf.Atan2(nWidth - nCorX, 2 * (nCorY - nHeight / 2)) < (1 - objImage.fillAmount) * Mathf.PI) return true;
                    if (!objImage.fillClockwise && Mathf.Atan2(nWidth - nCorX, 2 * (nCorY - nHeight / 2)) > objImage.fillAmount * Mathf.PI) return true;
                }
            }
            #endregion

            #region RADIAL_360
            if (objImage.fillMethod == Image.FillMethod.Radial360)
            {
                if (objImage.fillOrigin == (int)Image.Origin360.Bottom)
                {
                    if (objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) + Mathf.PI / 2;
                        var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle < checkAngle) return true;
                    }
                    if (!objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) + Mathf.PI / 2;
                        var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle > checkAngle) return true;
                    }
                }

                if (objImage.fillOrigin == (int)Image.Origin360.Right)
                {
                    if (objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2);
                        var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle < checkAngle) return true;
                    }
                    if (!objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2);
                        var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle > checkAngle) return true;
                    }
                }

                if (objImage.fillOrigin == (int)Image.Origin360.Top)
                {
                    if (objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI / 2;
                        var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle < checkAngle) return true;
                    }
                    if (!objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI / 2;
                        var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle > checkAngle) return true;
                    }
                }

                if (objImage.fillOrigin == (int)Image.Origin360.Left)
                {
                    if (objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI;
                        var checkAngle = Mathf.PI * 2 * (1 - objImage.fillAmount);
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle < checkAngle) return true;
                    }
                    if (!objImage.fillClockwise)
                    {
                        var angle = Mathf.Atan2(nCorY - nHeight / 2, nCorX - nWidth / 2) - Mathf.PI;
                        var checkAngle = Mathf.PI * 2 * objImage.fillAmount;
                        angle = angle < 0 ? Mathf.PI * 2 + angle : angle;
                        if (angle > checkAngle) return true;
                    }
                }
            }
            #endregion

        }
        #endregion

        var tx = (int)(texCorX + texRect.x);
        var ty = (int)(texCorY + texRect.y);

        if (mode == OuterEdgeDetection.ClosestRect || mode == OuterEdgeDetection.Fill)
        {
            int w = (int)texRect.width;
            int h = (int)texRect.height;

            var pixels = objTex.GetPixels((int)texRect.xMin, (int)texRect.yMin, w, h);

            if (mode == OuterEdgeDetection.ClosestRect)
            {
                int minX = int.MaxValue;
                int minY = int.MaxValue;
                int maxX = int.MinValue;
                int maxY = int.MinValue;

                for (int j = 0; j < h; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        var p = pixels[i + j * w];
                        if (p.a > 0.5f)
                        {
                            minX = Mathf.Min(minX, i);
                            minY = Mathf.Min(minY, j);

                            maxX = Mathf.Max(maxX, i);
                            maxY = Mathf.Max(maxY, j);
                        }
                    }
                }

#if DEVELOPMENT_BUILD
                if (tx >= 0 && ty >= 0 && tx < w && ty < h)
                {
                    pixels[tx + ty * w] = Color.red;
                }

                pixels[minX + minY * w] = Color.green;
                pixels[maxX + minY * w] = Color.green;
                pixels[minX + maxY * w] = Color.green;
                pixels[maxX + maxY * w] = Color.green;

                var tex2 = new Texture2D(w, h);
                tex2.SetPixels(pixels);
                tex2.Apply();
                RaycastingTest.detectedTexture = tex2;
#endif

                return !(tx >= minX && ty >= minY && tx <= maxX && ty <= maxY);
            }


            var replacementColor = Color.cyan;

            FloodFill(pixels, w, h, 0, 0, w - 1, h - 1, new Point(0, 0), replacementColor, 0.5f);
            FloodFill(pixels, w, h, 0, 0, w - 1, h - 1, new Point(w - 1, h - 1), replacementColor, 0.5f);

            for (int i = 0; i < pixels.Length; i++)
            {
                var p = pixels[i];
                if (p.Equals(replacementColor))
                {
                    p = Color.clear;
                }
                else
                {
                    p = Color.white;
                }
                pixels[i] = p;
            }

            var hit = false;

            if (tx >= 0 && ty >= 0 && tx < w && ty < h)
            {
                hit = pixels[tx + ty * w].a >= 1;
            }

#if DEVELOPMENT_BUILD
            if (tx >= 0 && ty >= 0 && tx < w && ty < h)
            {
                pixels[tx + ty * w] = Color.red; 
            }
            var tex = new Texture2D(w, h);
            tex.SetPixels(pixels);
            tex.Apply();
            RaycastingTest.detectedTexture = tex;
#endif

            return !hit;
        }


        // Getting targeted pixel alpha from object's texture 
        float alpha = objTex.GetPixel(tx, ty).a;

        // Deciding if we need to exclude the object from results list
        if (objAlphaCheck)
        {
            if (objAlphaCheck.includeMaterialAlpha) alpha *= objImage.color.a;
            if (alpha < objAlphaCheck.alphaThreshold) return true;
        }
        else
        {
            if (IncludeMaterialAlpha) alpha *= objImage.color.a;
            if (alpha < AlphaThreshold) return true;
        }

        return false;
    }
}
