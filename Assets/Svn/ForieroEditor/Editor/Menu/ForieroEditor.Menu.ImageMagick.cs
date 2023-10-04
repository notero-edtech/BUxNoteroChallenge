using System.IO;
using ForieroEditor.CommandLine;
using UnityEditor;
using UnityEngine;

namespace ForieroEditor.Menu
{
    public static partial class MenuItems
    {
        [MenuItem("Assets/ImageMagick/Jpg/10")] public static void ImageMagickJpg10() => ImageMagickInternal.ConvertToJPG(10);
        [MenuItem("Assets/ImageMagick/Jpg/20")] public static void ImageMagickJpg20() => ImageMagickInternal.ConvertToJPG(20);
        [MenuItem("Assets/ImageMagick/Jpg/30")] public static void ImageMagickJpg30() => ImageMagickInternal.ConvertToJPG(30);
        [MenuItem("Assets/ImageMagick/Jpg/40")] public static void ImageMagickJpg40() => ImageMagickInternal.ConvertToJPG(40);
        [MenuItem("Assets/ImageMagick/Jpg/50")] public static void ImageMagickJpg50() => ImageMagickInternal.ConvertToJPG(50);
        [MenuItem("Assets/ImageMagick/Jpg/60")] public static void ImageMagickJpg60() => ImageMagickInternal.ConvertToJPG(60);
        [MenuItem("Assets/ImageMagick/Jpg/70")] public static void ImageMagickJpg70() => ImageMagickInternal.ConvertToJPG(70);
        [MenuItem("Assets/ImageMagick/Jpg/80")] public static void ImageMagickJpg80() => ImageMagickInternal.ConvertToJPG(80);
        [MenuItem("Assets/ImageMagick/Jpg/90")] public static void ImageMagickJpg90() => ImageMagickInternal.ConvertToJPG(90);
        [MenuItem("Assets/ImageMagick/Jpg/100")] public static void ImageMagickJpg100() => ImageMagickInternal.ConvertToJPG(100);

        [MenuItem("Assets/ImageMagick/TGA")] public static void ImageMagickTGA() => ImageMagickInternal.ConvertToTGA();

        [MenuItem("Assets/ImageMagick/Quantize")] public static void ImageMagickPngQuant() => ImageMagickInternal.ConvertToPngQuant();
        [MenuItem("Assets/ImageMagick/Drop Shadow")] public static void ImageMagickDropShadow() => ImageMagickInternal.DropShadow();
        [MenuItem("Assets/ImageMagick/Shadow Outlines")] public static void ImageMagickShadowOutlines() => ImageMagickInternal.ShadowOutlines();
        [MenuItem("Assets/ImageMagick/Soft Edges")] public static void ImageMagickSoftEdges() => ImageMagickInternal.SoftEdges();
        [MenuItem("Assets/ImageMagick/Gray")] public static void ImageMagickGray() => ImageMagickInternal.ConvertToGray();
        [MenuItem("Assets/ImageMagick/Blur/0.5")] public static void ImageMagickBlur05() => ImageMagickInternal.Blur(0.5f);
        [MenuItem("Assets/ImageMagick/Blur/1.0")] public static void ImageMagickBlur10() => ImageMagickInternal.Blur(1f);
        [MenuItem("Assets/ImageMagick/Blur/1.5")] public static void ImageMagickBlur15() => ImageMagickInternal.Blur(1.5f);
        [MenuItem("Assets/ImageMagick/Blur/2.0")] public static void ImageMagickBlur20() => ImageMagickInternal.Blur(2f);
        [MenuItem("Assets/ImageMagick/Blur/2.5")] public static void ImageMagickBlur25() => ImageMagickInternal.Blur(2.5f);
        [MenuItem("Assets/ImageMagick/Blur/3.0")] public static void ImageMagickBlur30() => ImageMagickInternal.Blur(3f);
        [MenuItem("Assets/ImageMagick/Blur/3.5")] public static void ImageMagickBlur45() => ImageMagickInternal.Blur(3.5f);
        [MenuItem("Assets/ImageMagick/Blur/4.0")] public static void ImageMagickBlur50() => ImageMagickInternal.Blur(4f);
        [MenuItem("Assets/ImageMagick/Blur/4.5")] public static void ImageMagickBlur55() => ImageMagickInternal.Blur(4.5f);
        [MenuItem("Assets/ImageMagick/Blur/5.0")] public static void ImageMagickBlur60() => ImageMagickInternal.Blur(5f);
        [MenuItem("Assets/ImageMagick/Trim")] public static void ImageMagickTrim() => ImageMagickInternal.Trim(false);
        [MenuItem("Assets/ImageMagick/Trim Uniformly")] public static void ImageMagickTrimUniformly() => ImageMagickInternal.Trim(true);
        [MenuItem("Assets/ImageMagick/Watermark")] public static void ImageMagickWatermark() => ImageMagickInternal.Watermark();
        
        static class ImageMagickInternal
        {
            public static void ConvertToTGA()
            {
                switch (EditorUtility.DisplayDialogComplex("To TGA", "TGA selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.TGASelected("_tga");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.TGASelected("");
                        break;
                }
            }

            public static void ConvertToJPG(int quality)
            {
                switch (EditorUtility.DisplayDialogComplex("To Jpg", "Jpg selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.JpgSelected("_jpg", quality);
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.JpgSelected("");
                        break;
                }
            }
                        
            public static void ConvertToPngQuant()
            {
                switch (EditorUtility.DisplayDialogComplex("Quantize", "Quantize selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.PngQuantSelected("_quant");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.PngQuantSelected("");
                        break;
                }
            }
                      
            public static void DropShadow()
            {
                switch (EditorUtility.DisplayDialogComplex("Drop Shadow", "Drow Shadow selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.DropShadowSelected("_shadow");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.DropShadowSelected("");
                        break;
                }
            }

            public static void ShadowOutlines()
            {
                switch (EditorUtility.DisplayDialogComplex("Shadow Outlines", "Shadow Outlines selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.DropShadowOutlineSelected("_shadowoutline");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.DropShadowOutlineSelected("");
                        break;
                }
            }
                        
            public static void SoftEdges()
            {
                switch (EditorUtility.DisplayDialogComplex("Soft Edges", "Soft Edges selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.SoftEdgesSelected("_softedges");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.SoftEdgesSelected("");
                        break;
                }
            }
                        
            public static void ConvertToGray()
            {
                switch (EditorUtility.DisplayDialogComplex("Gray", "Gray selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.GraySelected("_gray");
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.GraySelected("");
                        break;
                }
            }
                        
            public static void Blur(float sigma = 2.0f)
            {
                switch (EditorUtility.DisplayDialogComplex("Blur", "Blur selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.BlurSelected("_blur", sigma);
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.BlurSelected("", sigma);
                        break;
                }
            }
                       
            public static void Trim(bool uniformly)
            {
                switch (EditorUtility.DisplayDialogComplex("Trim", "Trim selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                {
                    case 0:
                        Imagemagick.TrimSelected("_trim", uniformly);
                        break;
                    case 1:

                        break;
                    case 2:
                        Imagemagick.TrimSelected("", uniformly);
                        break;
                }
            }
                        
            public static void Watermark()
            {
                string watermark = Path.Combine(Directory.GetCurrentDirectory(), "watermark.png");
                if (File.Exists(watermark))
                {
                    switch (EditorUtility.DisplayDialogComplex("Watermark", "Watermark selected Texture2D(s)?", "Yes - Suffix", "No", "Yes"))
                    {
                        case 0:
                            Imagemagick.WatermarkSelected(watermark, "_watermark");
                            break;
                        case 1:

                            break;
                        case 2:
                            Imagemagick.WatermarkSelected(watermark, "");
                            break;
                    }
                }
                else
                {
                    Debug.LogError("File not exists : " + watermark);
                }
            }
        }
    }
}
