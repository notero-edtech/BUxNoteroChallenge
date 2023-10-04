using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class GitKeepEditor : EditorWindow
{
    //find . -name .git -prune -o -type d -empty -exec touch {}/.gitkeep \;//
    [MenuItem("Foriero/Tools/GitKeep", false, -2000)]
    static void GitKeepEditorMenu()
    {
        // Get existing open window or if none, make a new one:
        GitKeepEditor window = (GitKeepEditor)EditorWindow.GetWindow(typeof(GitKeepEditor));
        window.titleContent = new GUIContent("Git Keep");
        window.Show();
    }

    class DirInfo
    {
        public string directory = "";
        public string gitkeepFileName => Path.Combine(directory, ".gitkeep");
        public bool isEmpty = false;
        public bool hasGitKeep = false;
        public int entriesCount = 0;
    }

    List<DirInfo> dirs = new List<DirInfo>();

    private void OnEnable()
    {
    
    }

    private Vector2 v = Vector2.zero;
    private void OnGUI()
    {
        if(GUILayout.Button("Find Empty Folders"))
        {
            var directories = Directory.GetDirectories(ForieroEditor.Paths.Assets, "*", SearchOption.AllDirectories);
            dirs = directories.Select(d =>
                new DirInfo()
                {
                    directory = d,
                    isEmpty = IsDirectoryEmpty(d),
                    hasGitKeep = HasGitKeep(d),
                    entriesCount = EntriesCount(d)
                }
            ).ToList();            
        }

        v = EditorGUILayout.BeginScrollView(v);
        foreach (var d in dirs)
        {
            if (!d.isEmpty && !d.hasGitKeep) continue;
            if (d.directory.Contains(".svn")) continue;

            GUILayout.BeginHorizontal();
            {
                if (d.isEmpty || (!d.isEmpty && d.hasGitKeep && d.entriesCount == 1)) GUILayout.Box("EMPTY");
                
                if (GUILayout.Button("O"))
                {
                    EditorUtility.OpenWithDefaultApp(d.directory);
                }

                GUILayout.Label(d.directory);
                GUILayout.FlexibleSpace();

                if (d.hasGitKeep)
                {
                    if (GUILayout.Button("Delete .gitkeep"))
                    {
                        File.Delete(d.gitkeepFileName);
                        d.hasGitKeep = false;
                    }
                }
                else
                {
                    if (GUILayout.Button("Add .gitkeep"))
                    {
                        File.WriteAllText(d.gitkeepFileName, "");
                        d.hasGitKeep = true;
                    }
                }
                                
                if (GUILayout.Button("Delete Directory"))
                {
                    if (EditorUtility.DisplayDialog("Delete", $"Delete directory : {d.directory}", "Yes", "No"))
                    {
                        Directory.Delete(d.directory);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    public bool IsDirectoryEmpty(string path) => !Directory.EnumerateFileSystemEntries(path).Any();
    public bool HasGitKeep(string path) => File.Exists(Path.Combine(path, ".gitkeep"));
    public int EntriesCount(string path) => Directory.EnumerateFileSystemEntries(path).Count();
}
