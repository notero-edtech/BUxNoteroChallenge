using System.Linq;
using UnityEditor;
using UnityEngine;
using ForieroEditor.IO.FileUtilities;
using System.Collections.Generic;
using System.IO;
using ForieroEditor.Extensions;
using static FoldersDiffSettings.FoldersDiff;
using static FoldersDiffSettings;
using System;

using Object = UnityEngine.Object;
using System.Text.RegularExpressions;

public partial class FoldersDiffEditor : EditorWindow
{        
    [MenuItem("Foriero/Tools/FoldersDiff", false, -2000)]
    static void FoldersDiffEditorMenu()
    {
        // Get existing open window or if none, make a new one:
        FoldersDiffEditor window = (FoldersDiffEditor)EditorWindow.GetWindow(typeof(FoldersDiffEditor));
        window.titleContent = new GUIContent("FoldersDiff");
        window.Show();
    }    
        
    FoldersDiffSettings.FoldersDiff fd { get { return FoldersDiffSettings.instance.foldersDiff; } }

    FolderDiffResults diffResults = null;
        
    public class Diffs
    {        
        public class DiffItem
        {
            public string path = "";
            public DiffFlags diffFlag;            
            public FileInfo leftFileInfo;
            public FileInfo rightFileInfo;
        }

        public List<DiffItem> items = new List<DiffItem>();
    }

    public Diffs diffs = null;
    float columnWidth = 0;
    Color bgColor = Color.gray;
    void BGColor(Color c) => GUI.backgroundColor = c;
    void BGColorDefault() => GUI.backgroundColor = bgColor;
        
    Vector2 scrollPosition = Vector2.zero;
    
    private void OnGUI()
    {
        FoldersDiffSettings.InitCloudPaths();

        bgColor = GUI.backgroundColor;
        columnWidth = (position.width + 20) / 2;
               
        EditorGUILayout.BeginHorizontal();
        {
            fd.nameIndex = EditorGUILayout.Popup("Folders : ", fd.nameIndex, fd.Names);
            fd.name = EditorGUILayout.TextField(fd.name);
                       
            GUI.enabled = !string.IsNullOrEmpty(fd.name);
            if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                var newFolder = new FoldersDiffSettings.FoldersDiff.FoldersDiffItem() { name = fd.name };
                fd.folders.Add(newFolder);
                fd.folders.OrderBy((i) => i.name);
                fd.nameIndex = fd.folders.IndexOf(newFolder);
            }
            GUI.enabled = true;

            GUI.enabled = fd.SelectedFolder != null;
            if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                if (fd.nameIndex >= 0 && fd.nameIndex <= fd.folders.Count)
                {
                    fd.folders.RemoveAt(fd.nameIndex);
                    fd.nameIndex = -1;
                }
            }
                        
            if (GUILayout.Button("RENAME", EditorStyles.toolbarButton, GUILayout.Width(60)))
            {
                fd.SelectedFolder.name = fd.name;
            }
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();

        if (fd.SelectedFolder == null) return;

        HorizontalSeparator();
                                
        EditorGUILayout.BeginHorizontal();
        {
            fd.SelectedFolder.left.pathEnum = (FoldersDiffSettings.PathEnum)EditorGUILayout.EnumPopup(fd.SelectedFolder.left.pathEnum, GUILayout.Width(80));
            GUI.enabled = fd.SelectedFolder.left.pathEnum == FoldersDiffSettings.PathEnum.CloudRelative;
            fd.SelectedFolder.left.prependedPath = GUILayout.TextField(fd.SelectedFolder.left.prependedPath, GUILayout.Width(columnWidth - 80 - 25 - 40 - 22));
            if (GUILayout.Button("...", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                var leftCloudFolder = EditorUtility.OpenFolderPanel("Left Cloud Folder", "", "");
                if (!string.IsNullOrEmpty(leftCloudFolder)) fd.SelectedFolder.left.prependedPath = leftCloudFolder;
            }
            GUI.enabled = true;

            BGColor(fd.SelectedFolder.metaVisible ? Color.green : Color.red);
            if(GUILayout.Button("Meta Visible", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                fd.SelectedFolder.metaVisible = !fd.SelectedFolder.metaVisible;
            }
            BGColorDefault();

            GUI.enabled = fd.SelectedFolder.right.pathEnum == FoldersDiffSettings.PathEnum.CloudRelative;
            if (GUILayout.Button("...", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                var rightCloudFolder = EditorUtility.OpenFolderPanel("Right Cloud Folder", "", "");
                if (!string.IsNullOrEmpty(rightCloudFolder)) fd.SelectedFolder.right.prependedPath = rightCloudFolder;
            }
            fd.SelectedFolder.right.prependedPath = GUILayout.TextField(fd.SelectedFolder.right.prependedPath, GUILayout.Width(columnWidth - 80 - 25 - 40 - 22));
            GUI.enabled = true;
            fd.SelectedFolder.right.pathEnum = (FoldersDiffSettings.PathEnum)EditorGUILayout.EnumPopup(fd.SelectedFolder.right.pathEnum, GUILayout.Width(80));
        }
        EditorGUILayout.EndHorizontal();

        HorizontalSeparator();

        EditorGUILayout.BeginHorizontal();
        {            
            fd.SelectedFolder.left.path = GUILayout.TextField(fd.SelectedFolder.left.path, GUILayout.Width(columnWidth - 25 - 40 - 80 - 18));

            BGColor(fd.SelectedFolder.left.metaCopy ? Color.green : Color.red);
            if (GUILayout.Button("Copy Meta", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                fd.SelectedFolder.left.metaCopy = !fd.SelectedFolder.left.metaCopy;
            }
            BGColorDefault();

            if (GUILayout.Button("...", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                var leftFolder = EditorUtility.OpenFolderPanel("Left Folder", "", "");
                if (!string.IsNullOrEmpty(leftFolder)) fd.SelectedFolder.left.path = leftFolder;
            }

            GUI.enabled = !string.IsNullOrEmpty(fd.SelectedFolder.left.path) && !string.IsNullOrEmpty(fd.SelectedFolder.right.path);
            if (GUILayout.Button("Compare", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                diffResults = Folders.DiffFolders(fd.SelectedFolder.left.GetPath(), fd.SelectedFolder.right.GetPath(), "*");
                diffs = new Diffs();

                diffs.items.AddRange(diffResults.ChangedFiles.Select(x => new Diffs.DiffItem() {
                    path = x,
                    diffFlag = DiffFlags.ChangedFile,
                    leftFileInfo = new FileInfo(fd.SelectedFolder.left.GetPath(x)),
                    rightFileInfo = new FileInfo(fd.SelectedFolder.right.GetPath(x))

                }));

                diffs.items.AddRange(diffResults.IdenticalFiles.Select(x => new Diffs.DiffItem() { path = x, diffFlag = DiffFlags.IdenticalFile }));
                diffs.items.AddRange(diffResults.LeftOnlyFiles.Select(x => new Diffs.DiffItem() { path = x, diffFlag = DiffFlags.LeftOnlyFile }));
                diffs.items.AddRange(diffResults.LeftOnlyFolders.Select(x => new Diffs.DiffItem() { path = x, diffFlag = DiffFlags.LeftOnlyFolder }));
                diffs.items.AddRange(diffResults.RightOnlyFiles.Select(x => new Diffs.DiffItem() { path = x, diffFlag = DiffFlags.RightOnlyFile }));
                diffs.items.AddRange(diffResults.RightOnlyFolders.Select(x => new Diffs.DiffItem() { path = x, diffFlag = DiffFlags.RightOnlyFolder }));

                diffs.items.Sort((a, b) => string.Compare(a.path, b.path, System.StringComparison.Ordinal));
            }
            GUI.enabled = true;

            if (GUILayout.Button("...", EditorStyles.toolbarButton, GUILayout.Width(25)))
            {
                var rightFolder = EditorUtility.OpenFolderPanel("Right Folder", "", "");
                if (!string.IsNullOrEmpty(rightFolder)) fd.SelectedFolder.right.path = rightFolder;
            }

            BGColor(fd.SelectedFolder.right.metaCopy ? Color.green : Color.red);
            if (GUILayout.Button("Copy Meta", EditorStyles.toolbarButton, GUILayout.Width(80)))
            {
                fd.SelectedFolder.right.metaCopy = !fd.SelectedFolder.right.metaCopy;
            }
            BGColorDefault();

            fd.SelectedFolder.right.path = GUILayout.TextField(fd.SelectedFolder.right.path, GUILayout.Width(columnWidth - 25 - 40 - 80 - 18));
        }
        EditorGUILayout.EndHorizontal();

        HorizontalSeparator();

        EditorGUILayout.BeginHorizontal();
        {
            foreach(var f in System.Enum.GetValues(typeof(DiffFlags)))
            {
                var b = ((DiffFlags)f & fd.SelectedFolder.diffFlags) == (DiffFlags)f;
                BGColor(b ? Color.green : bgColor);
                if(GUILayout.Button(f.ToString(), EditorStyles.toolbarButton))
                {
                    if (b) fd.SelectedFolder.diffFlags &= ~(DiffFlags)f;
                    else fd.SelectedFolder.diffFlags |= (DiffFlags)f;
                }
                BGColorDefault();
            }
        }
        EditorGUILayout.EndHorizontal();

        HorizontalSeparator();

        if (diffResults == null) return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        {
            foreach (var item in diffs.items)
            {
                if (!fd.SelectedFolder.metaVisible && item.path.EndsWith(".meta", System.StringComparison.Ordinal)) continue;
                if (!fd.SelectedFolder.dotFoldersVisible && Regex.Match(item.path, @"(\/\..*\/|\\\..*\\)").Success) continue;
                if (item.path.EndsWith(".DS_Store")) continue;
                OnGUIEntry(item);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    void HorizontalSeparator(float height = 3)
    {
        GUILayout.Box("", GUILayout.Height(height), GUILayout.ExpandWidth(true));
    }

    void Ping(PathItem pathItem, string path)
    {
        if(GUILayout.Button("P", EditorStyles.toolbarButton, GUILayout.Width(25)))
        {
            var itemPath = pathItem.GetPath(path);
            if (File.Exists(itemPath)) itemPath = Path.GetDirectoryName(itemPath);
            EditorUtility.OpenWithDefaultApp(itemPath);
        }
    }

    void OnGUIEntry(Diffs.DiffItem diffItem)
    {       
        EditorGUILayout.BeginHorizontal();
        {
            switch (diffItem.diffFlag)
            {
                case DiffFlags.ChangedFile:
                    BGColor(DateTime.Compare(diffItem.leftFileInfo.LastWriteTime, diffItem.rightFileInfo.LastWriteTime) > 0 ? Color.green : Color.red);
                    {
                        GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f - 120f - 80f));
                        GUILayout.Label((diffItem.leftFileInfo.Length / 1000f).ToString() + "kB", EditorStyles.toolbarButton, GUILayout.Width(80f));
                        GUILayout.Label(diffItem.leftFileInfo.LastWriteTime.ToString(), EditorStyles.toolbarButton, GUILayout.Width(120f));
                    }
                    BGColorDefault();
                    Ping(fd.SelectedFolder.left, diffItem.path);

                    if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(40)))
                    {
                        var d = Path.GetDirectoryName(fd.SelectedFolder.right.GetPath(diffItem.path));
                        if (!Directory.Exists(d)) Directory.CreateDirectory(d);

                        var source = fd.SelectedFolder.left.GetPath(diffItem.path);
                        var destination = fd.SelectedFolder.right.GetPath(diffItem.path);

                        File.Copy(source, destination, true);
                        diffItem.diffFlag = DiffFlags.IdenticalFile;

                        if (destination.IsInProjectFolder())
                        {
                            AssetDatabase.Refresh();
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(destination.GetAssetPathFromFullPath()));
                        }
                    }

                    if (GUILayout.Button("<", EditorStyles.toolbarButton, GUILayout.Width(40)))
                    {
                        var d = Path.GetDirectoryName(fd.SelectedFolder.left.GetPath(diffItem.path));
                        if (!Directory.Exists(d)) Directory.CreateDirectory(d);

                        var source = fd.SelectedFolder.right.GetPath(diffItem.path);
                        var destination = fd.SelectedFolder.left.GetPath(diffItem.path);

                        File.Copy(source, destination, true);
                        diffItem.diffFlag = DiffFlags.IdenticalFile;

                        if (destination.IsInProjectFolder())
                        {
                            AssetDatabase.Refresh();
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(destination.GetAssetPathFromFullPath()));
                        }
                    }

                    Ping(fd.SelectedFolder.right, diffItem.path);
                    BGColor(DateTime.Compare(diffItem.rightFileInfo.LastWriteTime, diffItem.leftFileInfo.LastWriteTime) > 0 ? Color.green : Color.red);
                    {
                        GUILayout.Label(diffItem.rightFileInfo.LastWriteTime.ToString(), EditorStyles.toolbarButton, GUILayout.Width(120f));
                        GUILayout.Label((diffItem.rightFileInfo.Length / 1000f).ToString() + "kB", EditorStyles.toolbarButton, GUILayout.Width(80f));
                        GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f - 120f - 80f));                                               
                    }
                    BGColorDefault();
                    break;
                case DiffFlags.IdenticalFile when (fd.SelectedFolder.diffFlags & DiffFlags.IdenticalFile) == DiffFlags.IdenticalFile:
                    BGColor(Color.green); GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    Ping(fd.SelectedFolder.left, diffItem.path);
                    GUILayout.Label("Identical", EditorStyles.toolbarButton, GUILayout.Width(80));
                    Ping(fd.SelectedFolder.right, diffItem.path);
                    BGColor(Color.green); GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    break;
                case DiffFlags.LeftOnlyFile when (fd.SelectedFolder.diffFlags & DiffFlags.LeftOnlyFile) == DiffFlags.LeftOnlyFile:
                    BGColor(Color.green); GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    Ping(fd.SelectedFolder.left, diffItem.path);
                    if (GUILayout.Button(">", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        var d = Path.GetDirectoryName(fd.SelectedFolder.right.GetPath(diffItem.path));
                        if (!Directory.Exists(d)) Directory.CreateDirectory(d);

                        var source = fd.SelectedFolder.left.GetPath(diffItem.path);
                        var destination = fd.SelectedFolder.right.GetPath(diffItem.path);

                        File.Copy(source, destination);
                        diffItem.diffFlag = DiffFlags.IdenticalFile;

                        if (destination.IsInProjectFolder())
                        {
                            AssetDatabase.Refresh();
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(destination.GetAssetPathFromFullPath()));                            
                        }
                    }
                    Ping(fd.SelectedFolder.right, diffItem.path);
                    BGColor(Color.red); GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    break;
                case DiffFlags.LeftOnlyFolder when (fd.SelectedFolder.diffFlags & DiffFlags.LeftOnlyFolder) == DiffFlags.LeftOnlyFolder:
                    BGColor(Color.green); GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    Ping(fd.SelectedFolder.left, diffItem.path);
                    if (GUILayout.Button(">>", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        
                    }
                    Ping(fd.SelectedFolder.right, diffItem.path);
                    BGColor(Color.red); GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    break;
                case DiffFlags.RightOnlyFile when (fd.SelectedFolder.diffFlags & DiffFlags.RightOnlyFile) == DiffFlags.RightOnlyFile:
                    BGColor(Color.red); GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    Ping(fd.SelectedFolder.left, diffItem.path);
                    if (GUILayout.Button("<", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        var d = Path.GetDirectoryName(fd.SelectedFolder.left.GetPath(diffItem.path));
                        if (!Directory.Exists(d)) Directory.CreateDirectory(d);

                        var source = fd.SelectedFolder.right.GetPath(diffItem.path);
                        var destination = fd.SelectedFolder.left.GetPath(diffItem.path);

                        File.Copy(source, destination);
                        diffItem.diffFlag = DiffFlags.IdenticalFile;

                        if (destination.IsInProjectFolder())
                        {
                            AssetDatabase.Refresh();
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(destination.GetAssetPathFromFullPath()));
                        }
                    }
                    Ping(fd.SelectedFolder.right, diffItem.path);
                    BGColor(Color.green); GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    break;
                case DiffFlags.RightOnlyFolder when (fd.SelectedFolder.diffFlags & DiffFlags.RightOnlyFolder) == DiffFlags.RightOnlyFolder:
                    BGColor(Color.red); GUILayout.Label("", EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    Ping(fd.SelectedFolder.left, diffItem.path);
                    if (GUILayout.Button("<<", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {

                    }
                    Ping(fd.SelectedFolder.right, diffItem.path);
                    BGColor(Color.green); GUILayout.Label(diffItem.path, EditorStyles.toolbarButton, GUILayout.Width(columnWidth - 75f)); BGColorDefault();
                    break;
            }
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(FoldersDiffSettings.instance);

            FoldersDiffCloudSettings.instance.clouds = new List<FoldersDiffCloudSettings.CloudItem>();

            foreach(var folder in FoldersDiffSettings.instance.foldersDiff.folders)
            {
                FoldersDiffCloudSettings.instance.clouds.Add(new FoldersDiffCloudSettings.CloudItem() { guid = folder.left.guid, path = folder.left.cloudPath });
                FoldersDiffCloudSettings.instance.clouds.Add(new FoldersDiffCloudSettings.CloudItem() { guid = folder.right.guid, path = folder.right.cloudPath });
            }

            EditorUtility.SetDirty(FoldersDiffCloudSettings.instance);
        }
    }
}
