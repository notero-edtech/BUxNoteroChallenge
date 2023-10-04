using System.IO;
using ForieroEditor.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ForieroEditor.CommandLine
{
    public static class FFMpeg
    {
        public static string cmdFFMpeg
        {
            get
            {
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                return "/opt/local/bin/ffmpeg";
#elif UNITY_EDITOR_WIN
				return @"C:\FFmpeg\bin\ffmpeg.exe";
#endif
            }
        }
        
        public enum Image2MOV{
            png,
            jpg
        }

        //ffmpeg -r 30 -i image_%04d.jpg -codec png out.mov
        public static void ConvertImageSequence2MOV(float frameRate, Image2MOV image2MOV, string sourceDirectory, string imagePattern = "image_%04d", string outputFilePath = "output.mov")
        {
            string args =
                "-y" +               
                " -r " + frameRate.ToString() +
                " -i " + (Path.Combine(sourceDirectory, imagePattern) + "." + image2MOV.ToString()).FixOSPath().DoubleQuotes() +
                //" -vn" +
                //" -filter:v vflip" +
                " -codec png" +
                " " + outputFilePath.FixOSPath().DoubleQuotes();
            
            CMD.GenerateProcess(cmdFFMpeg, args, true, true, true);
        }

        public static void ConvertSelectedAudio(string suffix, string toExtension, string fromExtension = null)
        {
            toExtension = toExtension.ToLower();

            if (!string.IsNullOrEmpty(fromExtension))
            {
                fromExtension = fromExtension.ToLower();
            }

            foreach (Object o in Selection.GetFiltered(typeof(AudioClip), SelectionMode.DeepAssets))
            {
                if (o is AudioClip)
                {
                    string path = Path.Combine(Directory.GetCurrentDirectory(), AssetDatabase.GetAssetPath(o.GetInstanceID()));
                    if (string.IsNullOrEmpty(fromExtension))
                    {
                        ConvertAudio(path, suffix, toExtension);
                    }
                    else if (Path.GetExtension(path).ToLower().Contains(fromExtension))
                    {
                        ConvertAudio(path, suffix, toExtension);
                    }
                }
            }
            AssetDatabase.Refresh();
        }

        public static void ConvertAudio(string inputFileName, string suffix, string toExtension, string ouputFileName = null)
        {
            toExtension = toExtension.ToLower();

            string path = Path.GetDirectoryName(inputFileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(inputFileName);
            string extension = Path.GetExtension(inputFileName).ToLower();

            ouputFileName = ouputFileName ?? Path.Combine(path, fileNameWithoutExtension + suffix + "." + toExtension);

            string codec = "";

            if (toExtension == "mp3")
            {
                codec = "libmp3lame";
            }
            else if (toExtension == "ogg")
            {
                codec = "libvorbis";
            }
            else if (toExtension == "aac")
            {
                codec = "libfaac";
            }
            else if (toExtension == "ac3")
            {
                codec = "ac3";
            }

            if (string.IsNullOrEmpty(codec))
            {
                Debug.LogError("Codec not found for conversion : " + extension + " => " + toExtension);
                return;
            }

            if (extension.Replace(".", "") == toExtension)
            {
                Debug.LogWarning("File will not be converted : " + extension + " => " + toExtension);
                return;
            }

            string args = "-i " + inputFileName.FixOSPath().DoubleQuotes() + " -acodec " + codec + " " + ouputFileName.FixOSPath().DoubleQuotes();
            
            CMD.GenerateProcess(cmdFFMpeg, args);
        }
    }
}
