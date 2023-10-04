/* Marek Ledvina © Foriero s.r.o. 2022, The Commercial License */
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;

using ForieroEngine.Music.MusicXML.Xsd;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MusicXMLLoadSaveBulk : MonoBehaviour
{
    public Font font;

    scorepartwise score;

    public static string Left(string str, int length)
    {
        if (str == null)
        {
            return str;
        }

        return str.Substring(0, Math.Min(Math.Abs(length), str.Length));
    }

    public static string WrapColor(string s, Color color)
    {
        return "<color=" + color.ToString() + ">" + s + "</color>";
    }

    public List<FileItem> fileItems = new List<FileItem>();

    public class FileItem
    {
        public class Item
        {
            public long loadingTime = -1;
            public long savingTime = -1;
            public long originalFileSize = -1;
            public long savedFileSize = -1;
            public string loadingException = "";
            public string savingException = "";
            public string fileName = "";

        }

        public string fileName;
        public bool exception = false;

        public Item xsdItem = new Item();

        public string ToString2()
        {
            return xsdItem.originalFileSize.ToString().PadRight(10)
                + xsdItem.savedFileSize.ToString().PadRight(10)
                + (string.IsNullOrEmpty(xsdItem.loadingException) ? xsdItem.loadingTime.ToString().PadRight(10) : WrapColor(xsdItem.loadingTime.ToString().PadRight(10), Color.red))
                          + (string.IsNullOrEmpty(xsdItem.savingException) ? xsdItem.savingTime.ToString().PadRight(10) : WrapColor(xsdItem.savingTime.ToString().PadRight(10), Color.red));

            ;
        }

        public override string ToString()
        {
            return Left(Path.GetFileNameWithoutExtension(fileName), 30).PadRight(30)
                       + xsdItem.originalFileSize.ToString().PadRight(10)
                       + xsdItem.savedFileSize.ToString().PadRight(10)
                       + (string.IsNullOrEmpty(xsdItem.loadingException) ? xsdItem.loadingTime.ToString().PadRight(10) : WrapColor(xsdItem.loadingTime.ToString().PadRight(10), Color.red))
                                                                       + (string.IsNullOrEmpty(xsdItem.savingException) ? xsdItem.savingTime.ToString().PadRight(10) : WrapColor(xsdItem.savingTime.ToString().PadRight(10), Color.red));

            ;
        }

        public static string GetHeader()
        {
            return "Filename".PadRight(40)
                      + "Org.Size".PadRight(10)
                      + "Svd.Size".PadRight(10)
                      + "Load(t)".PadRight(10)
                      + "Save(t)".PadRight(10)
                      ;
        }
    }

    void Start()
    {
        Init();
    }

    Vector2 scrollRect = Vector2.zero;

    void OnGUI()
    {
        GUI.skin.font = font;

        scrollRect = GUILayout.BeginScrollView(scrollRect);

        GUILayout.Label(FileItem.GetHeader());

        foreach (FileItem fileItem in fileItems)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(Left(Path.GetFileNameWithoutExtension(fileItem.fileName), 29).PadRight(29)))
            {
                Parse(fileItem);
            }
            if (GUILayout.Button("ORG"))
            {
#if UNITY_EDITOR
                EditorUtility.OpenWithDefaultApp(fileItem.fileName);
#endif
            }
            if (GUILayout.Button("XSD"))
            {
#if UNITY_EDITOR
                EditorUtility.OpenWithDefaultApp(fileItem.xsdItem.fileName);
#endif
            }

            GUILayout.Label(fileItem.ToString2());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    void Init()
    {
        fileItems = new List<FileItem>();

        string[] files = Directory.GetFiles(Application.dataPath, "scores.txt", SearchOption.AllDirectories);
        string directory = Path.GetDirectoryName(files[0]);
        files = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            FileItem fileItem = new FileItem();
            fileItem.fileName = file;
            fileItems.Add(fileItem);
        }
    }

    public void Parse(FileItem fileItem)
    {

        Stopwatch stopWatch = new Stopwatch();
        long lastEllapsedMilliseconds = 0;
        long currentEllapsedMilliseconds = 0;
        stopWatch.Start();


        try
        {
            LoadXML(File.ReadAllBytes(fileItem.fileName));
            fileItem.xsdItem.originalFileSize = new System.IO.FileInfo(fileItem.fileName).Length;
        }
        catch (Exception e)
        {
            fileItem.xsdItem.loadingException = e.ToString();
            fileItem.exception = true;
            UnityEngine.Debug.LogError(e.ToString());
        }

        if (string.IsNullOrEmpty(fileItem.xsdItem.loadingException))
        {
            currentEllapsedMilliseconds = stopWatch.ElapsedMilliseconds;
            fileItem.xsdItem.loadingTime = (currentEllapsedMilliseconds - lastEllapsedMilliseconds);
            lastEllapsedMilliseconds = currentEllapsedMilliseconds;


            try
            {
                fileItem.xsdItem.fileName = Path.Combine(Application.dataPath, "XML/XSD/" + Path.GetFileName(fileItem.fileName));
                Save(fileItem.xsdItem.fileName);
                fileItem.xsdItem.savedFileSize = new System.IO.FileInfo(fileItem.xsdItem.fileName).Length;
            }
            catch (Exception e)
            {
                fileItem.xsdItem.savingException = e.ToString();
                fileItem.exception = true;
                UnityEngine.Debug.LogError(e.ToString());
            }

            if (string.IsNullOrEmpty(fileItem.xsdItem.savingException))
            {
                currentEllapsedMilliseconds = stopWatch.ElapsedMilliseconds;
                fileItem.xsdItem.savingTime = (currentEllapsedMilliseconds - lastEllapsedMilliseconds);
                lastEllapsedMilliseconds = currentEllapsedMilliseconds;
            }

        }
    }


    void LoadXML(byte[] bytes)
    {
        using (MemoryStream xmlStream = new MemoryStream(bytes))
        {
            score = score.Load(xmlStream);
        }
    }

    void Save(string fileName)
    {
        string dir = Path.GetDirectoryName(fileName);

        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (score != null)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (FileStream xmlStream = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
            {
                score.Save(xmlStream);
            }
        }
    }
}
