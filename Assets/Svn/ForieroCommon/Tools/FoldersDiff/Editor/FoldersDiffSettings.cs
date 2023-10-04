using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ForieroEditor;
using UnityEngine;

public class FoldersDiffSettings : ScriptableObject
{    
    public static FoldersDiffSettings instance { get { return Instances.EditorInstance<FoldersDiffSettings>(typeof(FoldersDiffSettings).Name, "Assets/Editor/FoldersDiff/"); } }
    static bool initialized = false;

    public static void InitCloudPaths()
    {
        if (initialized) return;
                
        foreach(var folder in instance.foldersDiff.folders)
        {
            folder.left.cloudPath = FoldersDiffCloudSettings.instance.clouds.Where(x => x.guid == folder.left.guid).Select(x => x.path).FirstOrDefault() ?? "";
            folder.right.cloudPath = FoldersDiffCloudSettings.instance.clouds.Where(x => x.guid == folder.right.guid).Select(x => x.path).FirstOrDefault() ?? "";
        }

        initialized = true;
    }

    public enum PathEnum
    {
        Absolute,
        ProjectRelative,
        CloudRelative
    }

    [Flags]
    public enum DiffFlags
    {
        ChangedFile = 1 << 0,
        IdenticalFile = 1 << 1,
        LeftOnlyFile = 1 << 2,
        LeftOnlyFolder = 1 << 3,
        RightOnlyFile = 1 << 4,
        RightOnlyFolder = 1 << 5
    }

    [Serializable]
	public class FoldersDiff
	{        
        [Serializable]
        public class PathItem
        {
            public string guid = Guid.NewGuid().ToString();

            public bool metaCopy = false;           
            public PathEnum pathEnum = PathEnum.CloudRelative;

            [NonSerialized] public string cloudPath = "";
            [SerializeField] string pathInternal = "";

            public string path {
                get
                {
                    return pathInternal;
                }
                set
                {
                    if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(prependedPath)) pathInternal = value; 
                    else pathInternal = value.Replace(prependedPath, "");
                }
            }

            public string prependedPath
            {
                get
                {
                    switch (pathEnum)
                    {
                        case PathEnum.Absolute: return "";
                        case PathEnum.ProjectRelative: return Directory.GetCurrentDirectory();                            
                        case PathEnum.CloudRelative: return cloudPath;                            
                    }
                    return "";
                }
                set
                {
                    switch (pathEnum)
                    {
                        case PathEnum.Absolute: break;
                        case PathEnum.ProjectRelative: break;
                        case PathEnum.CloudRelative: cloudPath = value; break;
                    }
                }
            }

            string AppendPathInternal(string originalPath, string appendedPath)
            {
                if (appendedPath.StartsWith("/", StringComparison.Ordinal) || appendedPath.StartsWith("\\", StringComparison.Ordinal))
                {
                    appendedPath = appendedPath.Substring(1);
                }
                return Path.Combine(originalPath, appendedPath);
            }

            public string GetPath(string appendedPath = "")
            {
                string r = "";
                switch (pathEnum)
                {
                    case PathEnum.Absolute:
                        r = AppendPathInternal(path, appendedPath);
                        break;
                    case PathEnum.ProjectRelative:                      
                    case PathEnum.CloudRelative:
                        r = AppendPathInternal(prependedPath, path);
                        r = AppendPathInternal(r, appendedPath);
                        break;
                }                
                //Debug.Log(r);
                return r;
            }
        }

        [Serializable]
		public class FoldersDiffItem
		{
			public string name = "";

            public DiffFlags diffFlags = ~DiffFlags.IdenticalFile;

            public bool metaVisible = false;
            public bool dotFoldersVisible = false;

            public PathItem left = new PathItem();
            public PathItem right = new PathItem();
		}

		public List<FoldersDiffItem> folders = new List<FoldersDiffItem>();

		public string[] Names => folders.Select((i) => i.name).ToArray();

		[NonSerialized] public int nameIndex = -1;
		[NonSerialized] public string name = "";

		public FoldersDiffItem SelectedFolder => (nameIndex >= 0 && nameIndex < folders.Count) ? folders[nameIndex] : null;

		public void DoCompareFolders()
		{

		}
    }

    public FoldersDiff foldersDiff = new FoldersDiff();
}
