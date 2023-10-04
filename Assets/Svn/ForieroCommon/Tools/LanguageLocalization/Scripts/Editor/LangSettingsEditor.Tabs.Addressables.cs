using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ForieroEditor.Extensions;
using Renci.SshNet;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public partial class LangSettingsEditor : Editor
{
    List<ServerFile> serverFiles = new List<ServerFile>();

    private string[] dirs = new string[0];

    private bool uploading = false;
    void DrawAddresables()
    {
        
        
        if (GUILayout.Button("Clean"))
        {
            Lang.Clean();
        }

        if (GUILayout.Button("Update"))
        {
            Lang.UpdateAddressables();
        }

        if (GUILayout.Button("Build"))
        {
            AddressableAssetSettings.BuildPlayerContent();
        }

        
        
        GUILayout.Label("SFTP");
        o.ftpAuth.user = EditorGUILayout.TextField("User", o.ftpAuth.user);
        o.ftpAuth.paswd = EditorGUILayout.PasswordField("Password", o.ftpAuth.paswd);
        o.ftpAuth.port = EditorGUILayout.IntField("Port ( 10222 )", o.ftpAuth.port);
        o.ftpAuth.host = EditorGUILayout.TextField("Host ( backend.foriero.com )", o.ftpAuth.host);
        o.ftpAuth.hostpath = EditorGUILayout.TextField("Host Path ( /www/unity/voice_over/ )", o.ftpAuth.hostpath);

        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.Box("Platform", GUILayout.ExpandWidth(true));
            if (GUILayout.Button("Refresh", GUILayout.Width(60)))
            {
                dirs = Directory.GetDirectories("ServerData", "*.*", SearchOption.TopDirectoryOnly);
            }
            
            if (GUILayout.Button("Progress", GUILayout.Width(60)))
            {
                EditorApplication.ExecuteMenuItem("Window/General/Progress");
            }
            GUI.enabled = !uploading;
            if (GUILayout.Button("Upload All", GUILayout.Width(80))) { UploadAddressables(); }
            GUI.enabled = true;
        }
        EditorGUILayout.EndHorizontal();
        
        if (uploading) EditorGUILayout.HelpBox($"Upload progress : {Progress.globalProgress}%", MessageType.Info);
        
        GUI.enabled = !uploading;
        foreach (var d in dirs)
        {
            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.Label(d);
                if (GUILayout.Button("Upload", GUILayout.Width(80)))
                {
                    UploadAddressables(d);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        GUI.enabled = true;
    }

    class ServerFile
    {
        public readonly string local = "";
        public readonly string server = "";
        public volatile float progress = 0f;

        public ServerFile(string local, string server)
        {
            this.local = local;
            this.server = server;
        }
    }
    
    void UploadAddressables(string serverData = "ServerData")
    {
        Task.Run(() =>
        {
            uploading = true;
            ConnectionInfo c = new ConnectionInfo(o.ftpAuth.host, o.ftpAuth.port, o.ftpAuth.user,
                new AuthenticationMethod[]
                {
                    new PasswordAuthenticationMethod(o.ftpAuth.user, o.ftpAuth.paswd)
                });

            using (var sftp = new SftpClient(c))
            {
                Debug.Log("SFTP | Connecting...");
                sftp.Connect();

                var hostpath = o.ftpAuth.hostpath.FixUnixPath();
                Debug.Log("SFTP | Checking folder : " + hostpath);
                if (!sftp.Exists(hostpath)) sftp.CreateDirectory(hostpath);

                hostpath = Path.Combine(hostpath, o.voProjectName).FixUnixPath();
                Debug.Log("SFTP | Checking folder : " + hostpath);
                if (!sftp.Exists(hostpath)) sftp.CreateDirectory(hostpath);

                hostpath = Path.Combine(hostpath, o.voVersion).FixUnixPath();
                Debug.Log("SFTP | Checking folder : " + hostpath);
                if (!sftp.Exists(hostpath)) sftp.CreateDirectory(hostpath);

                sftp.ChangeDirectory(hostpath);

                var localFiles = Directory.GetFiles(serverData, "*.*", SearchOption.AllDirectories);

                serverFiles = new List<ServerFile>();

                foreach (var localFile in localFiles)
                {
                    if (localFile.Contains(".DS_Store")) continue;

                    var serverFile = Path.Combine(hostpath, localFile).FixUnixPath().Replace("/ServerData/", "/");
                    Debug.Log("SFTP | Filename : " + serverFile);

                    var d = Path.GetDirectoryName(serverFile).FixUnixPath();
                    Debug.Log("SFTP | Checking folder : " + d);
                    if (!sftp.Exists(d))
                    {
                        Debug.Log("SFTP | Creating folder : " + d);
                        sftp.CreateDirectory(d);
                    }

                    sftp.ChangeDirectory(d);

                    serverFiles.Add(new ServerFile(localFile, serverFile));

                    sftp.ChangeDirectory(hostpath);
                }

                Debug.Log(serverFiles.Count.ToString());

                Parallel.ForEach(serverFiles, (f) =>
                {
                    try
                    {
                        var id = Progress.Start(f.local);

                        using (var uplfileStream = File.OpenRead(f.local))
                        {
                            Debug.Log("SFTP | Uploading : " + f.server);
                            var asyncUploadFile = sftp.BeginUploadFile(uplfileStream, f.server, true,
                                (a) => { }, null, (p) => { f.progress = (float) p / (float) uplfileStream.Length; });

                            while (!asyncUploadFile.IsCompleted)
                            {
                                Progress.Report(id, f.progress);
                                Thread.Sleep(1);
                            }
                        }

                        Progress.Remove(id);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                });

                Debug.Log("SFTP | Disconnecting...");
                sftp.Disconnect();
                Debug.Log("SFTP | Disconnected...");
            }
            uploading = false;
        });
    }
}
