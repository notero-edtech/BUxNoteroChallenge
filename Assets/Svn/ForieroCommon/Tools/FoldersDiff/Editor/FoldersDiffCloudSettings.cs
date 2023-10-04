using System;
using System.Collections.Generic;
using ForieroEditor;
using UnityEngine;

public class FoldersDiffCloudSettings : ScriptableObject
{    
    public static FoldersDiffCloudSettings instance { get { return Instances.EditorInstance<FoldersDiffCloudSettings>(typeof(FoldersDiffCloudSettings).Name, "Assets/Editor/FoldersDiff/"); } }

    [Serializable]
    public class CloudItem
    {
        public string guid;
        public string path;
    }

    public List<CloudItem> clouds = new List<CloudItem>();


}
