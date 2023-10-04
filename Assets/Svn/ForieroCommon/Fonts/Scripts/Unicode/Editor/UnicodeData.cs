using System.Collections;
using System.Globalization;
using System.IO;
using ForieroEditor.Coroutines;
using ForieroEngine.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace ForieroEditor.Fonts
{
    public static class UnicodeData
    {
        [MenuItem("Foriero/Fonts/UnicodeData/Download")]
        public static void DownloadUnicodeData()
        {
            EditorCoroutineStart.StartCoroutine(DownloadUnicodeDataCoroutine());
        }

        static IEnumerator DownloadUnicodeDataCoroutine()
        {
            Debug.Log("Downloading UnicodeData");
            UnityWebRequest www = new UnityWebRequest("http://www.unicode.org/Public/UNIDATA/UnicodeData.txt");
            yield return www.SendWebRequest();

            if (www.result.HasError())
            {
                Debug.Log(www.error);
                yield break;
            }

            string cyrillic = "";
            string latin = "";
            string greek = "";
            string cjk = "";
            string common = "";
            string others = "";

            string[] files = Directory.GetFiles(Application.dataPath, "UnicodeData.txt", SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                string fileName = files[0];

                Debug.Log("Saving UnicodeData.txt");
                File.WriteAllBytes(fileName, www.downloadHandler.data);

                string[] lines = File.ReadAllLines(fileName);

                Debug.Log("Parsing UnicodeData.txt");
                foreach (string line in lines)
                {
                    string[] items = line.Split(';');
                    if (items.Length > 1)
                    {
                        var unicode = items[0];
                        uint unicodeInt = System.Convert.ToUInt32(items[0], 16);
                        var desc = items[1];

                        if (desc.Contains("<control>")) continue;

                        char ch = default(char);

                        if (unicode.Length == 4)
                        {
                            if (desc.Contains("LATIN"))
                            {
                                ch = System.Convert.ToChar(unicodeInt);
                                latin += ch.ToString();
                            }
                            else if (desc.Contains("CYRILLIC"))
                            {
                                ch = System.Convert.ToChar(unicodeInt);
                                cyrillic += ch.ToString();
                            }
                            else if (desc.Contains("GREEK"))
                            {
                                ch = System.Convert.ToChar(unicodeInt);
                                greek += ch.ToString();
                            }
                            else if (unicodeInt <= 0x00A9)
                            {
                                ch = System.Convert.ToChar(unicodeInt);
                                common += ch.ToString();
                            }
                            else
                            {
                                ch = System.Convert.ToChar(unicodeInt);
                                others += ch.ToString();
                            }
                        }
                    }
                }

                int cjkFrom = int.Parse("4E00", NumberStyles.AllowHexSpecifier);
                int cjkTo = int.Parse("9FFF", NumberStyles.AllowHexSpecifier);

                for (int i = cjkFrom; i <= cjkTo; i++)
                {
                    cjk += (char)i;
                }

                SaveTXT(fileName, "Others.txt", others);
                SaveTXT(fileName, "Common.txt", common);
                SaveTXT(fileName, "Latin.txt", latin);
                SaveTXT(fileName, "Greek.txt", greek);
                SaveTXT(fileName, "Cyrillic.txt", cyrillic);
                SaveTXT(fileName, "CJK.txt", cjk);
                SaveTXT(fileName, "LatinCyrillicCommon.txt", latin + cyrillic + common);
            }
            else
            {
                Debug.LogError("UnicodeData.txt not found!!!");
            }
        }

        static void SaveTXT(string unicodeDataFileName, string targetName, string content)
        {
            string baseFileName = Path.GetFileNameWithoutExtension(unicodeDataFileName);
            string fileName = baseFileName + targetName;
            string fullFileName = Path.Combine(Path.GetDirectoryName(unicodeDataFileName), fileName);
            File.WriteAllText(fullFileName, content);
        }
    }
}
