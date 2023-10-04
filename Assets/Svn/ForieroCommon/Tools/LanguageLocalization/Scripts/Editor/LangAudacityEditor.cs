using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System;

[CustomEditor(typeof(LangAudacity))]
public class LangAudacityEditor : Editor {

	LangAudacity o;

	void OnEnable(){
		o = target as LangAudacity;
	}

	string path; 

	public override void OnInspectorGUI (){
		EditorGUILayout.BeginHorizontal();
		o.audacityPath = EditorGUILayout.TextField("Audacity Path : ", o.audacityPath);
		if(GUILayout.Button("...", GUILayout.Width(30))){
			if(!string.IsNullOrEmpty(path = EditorUtility.SaveFolderPanel("Audacity Directory", "", ""))){
				o.audacityPath = path;
				EditorUtility.SetDirty(target);
			}
		}
		EditorGUILayout.EndHorizontal();


		EditorGUILayout.BeginHorizontal();
		o.exportPath = EditorGUILayout.TextField("Export Path : ", o.exportPath);
		if(GUILayout.Button("...", GUILayout.Width(30))){
			if(!string.IsNullOrEmpty(path = EditorUtility.SaveFolderPanel("Export Directory", "", ""))){
				o.exportPath = path.Replace(Application.dataPath, "");
				EditorUtility.SetDirty(target);
			}
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Refresh")){
			Refresh ();
		}
		if(GUILayout.Button("Export All")){
			ExportAll ();
		}
		EditorGUILayout.EndHorizontal();

		foreach(string s in o.labelFiles){
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(Path.GetFileName(Path.GetDirectoryName(s)));

			if(GUILayout.Button("Export", GUILayout.Width(50))){
				Export (s);
			}
			EditorGUILayout.EndHorizontal();
		}
	}

	void Export(string labelsPath){
		ExportCommandLine(Path.GetDirectoryName(labelsPath), o.exportPath);
		AssetDatabase.Refresh();
	}

	void ExportAll(){
		foreach(string s in o.labelFiles){
			Export (s);
		}
	}

	void Refresh(){
		if(!string.IsNullOrEmpty(o.audacityPath)){
			o.labelFiles = Directory.GetFiles(o.audacityPath, "labels.txt", SearchOption.AllDirectories);
			EditorUtility.SetDirty(o);
		}
	}

	public static void ExportCommandLine(string pwd, string exportPath){
		System.Diagnostics.Process p = new System.Diagnostics.Process();
		string cmd = "/opt/local/bin/mp3splt";
		string args = "-A labels.txt -d '" + Application.dataPath + exportPath + "' output.mp3";
		UnityEngine.Debug.Log(cmd + " " + args); 
		try
		{
			p.StartInfo.FileName = cmd;
			p.StartInfo.Arguments = args;
			p.StartInfo.WorkingDirectory = pwd;
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.Start();
			string output = p.StandardOutput.ReadToEnd();
			string error = p.StandardError.ReadToEnd();
			p.WaitForExit();
			p.Close();
			if(!string.IsNullOrEmpty(output)) {
				Debug.Log("ERROR : " + output);	
			}
			
			if(!string.IsNullOrEmpty(error)) {
				Debug.LogError("ERROR : " + error);	
			}
		}
		catch (System.Exception e)
		{
			UnityEngine.Debug.LogWarning( e.Message);
		}
		finally {
			p.Dispose();
			System.GC.Collect();
		}
	}
}
