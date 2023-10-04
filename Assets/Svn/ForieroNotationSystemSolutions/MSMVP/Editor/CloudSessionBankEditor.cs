using System.Collections.Generic;
using System.IO;
using ForieroEngine.Music.MusicXML.MXL;
using MoreLinq;
using UnityEditor;
using UnityEngine;

public class CloudSessionBankEditor
{
    [MenuItem("Foriero/NS/Cloud/Copy all .mxl.bytes into Resources")]
    public static void CopyAllMXL() => CopyAll("*.mxl.bytes");
    
    [MenuItem("Foriero/NS/Cloud/Copy all .mp3 into Resources")]
    public static void CopyAllMP3() => CopyAll("*.mp3");
    
    [MenuItem("Foriero/NS/Cloud/Copy all .ogg into Resources")]
    public static void CopyAllOGG() => CopyAll("*.ogg");
    
    [MenuItem("Foriero/NS/Cloud/Copy all .sib into Resources")]
    public static void CopyAllSIB() => CopyAll("*.sib");
    
    public static void CopyAll(string extension)
    {
        var folder = EditorUtility.OpenFolderPanel("Score Folder", Directory.GetCurrentDirectory(), "");
        if (string.IsNullOrEmpty(folder)) return;
        
        var mxls = Directory.GetFiles(folder, extension, SearchOption.AllDirectories);
        var r = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Resources/Scores");
        //Debug.Log(r);
        if (!Directory.Exists(r)) Directory.CreateDirectory(r);
        mxls.ForEach(mxl =>
        {
            var p = mxl.Replace(folder, "");
            //Debug.Log(p);
            var d = Path.GetDirectoryName(p).Remove(0, 1);
            //Debug.Log(d);
            var f = Path.GetFileName(p);
            //Debug.Log(f);
            var fd = Path.Combine(r, d);
            //Debug.Log(fd);
            if (!Directory.Exists(fd)) Directory.CreateDirectory(fd);
            var fp = Path.Combine(fd, f);
            File.Copy(mxl, fp);
        });
        AssetDatabase.Refresh();
    }

    [MenuItem("Foriero/NS/Cloud/PDFs Missing SIBs")]
    public static void CheckSibeliusFiles()
    {
        var folder = EditorUtility.OpenFolderPanel("Score Folder", Directory.GetCurrentDirectory(), "");
        if (string.IsNullOrEmpty(folder)) return;
        
        List<string> missingSibeliusFiles = new();
        var pdfs = Directory.GetFiles(folder, "*.pdf", SearchOption.AllDirectories);
        pdfs.ForEach(pdf =>
        {
            var musicxml = Path.ChangeExtension(pdf, ".musicxml");
            var sib = Path.ChangeExtension(pdf, ".sib");
            if (!File.Exists(sib)) missingSibeliusFiles.Add(sib.Replace(folder, ""));
        });

        var txtFile = Path.Combine(folder, "missing_files.txt");
        File.WriteAllLines(txtFile, missingSibeliusFiles);
    }

    [MenuItem("Foriero/NS/Cloud/MusicXML -> MXL")]
    public static void MusicXMLtoMXL()
    {
        var folder = EditorUtility.OpenFolderPanel("Score Folder", Directory.GetCurrentDirectory(), "");
        if (string.IsNullOrEmpty(folder)) return;
        
        var musicxmls = Directory.GetFiles(folder, "*.musicxml", SearchOption.AllDirectories);
        musicxmls.ForEach(musicxml =>
        {
            var musicxmlBytes = File.ReadAllBytes(musicxml);
            var mxlBytes = musicxmlBytes.Zip(Path.GetFileName(musicxml));
            var mxlFileNamePath = Path.ChangeExtension(musicxml, ".mxl");
            var mxlBytesFileNamePath = Path.ChangeExtension(musicxml, ".mxl.bytes");
            File.WriteAllBytes(mxlFileNamePath, mxlBytes);
            File.WriteAllBytes(mxlBytesFileNamePath, mxlBytes);
        });
    }
}
