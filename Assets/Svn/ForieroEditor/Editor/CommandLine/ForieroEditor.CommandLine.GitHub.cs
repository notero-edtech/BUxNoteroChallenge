using System;
using System.IO;
using ForieroEditor.Extensions;

namespace ForieroEditor.CommandLine
{
    public static class GitHub
	{
		public static void GetRepositoryFiles(string gitHubPath, string assetsPath, Action<string> onOutput = null, Action<string> onError = null)
		{
			var cmd = "/opt/local/bin/svn";
			var path = Path.Combine (Directory.GetCurrentDirectory (), assetsPath).FixOSPath ();
			var args = "export --trust-server-cert --non-interactive --force \"" + gitHubPath + "\" \"" + path + "\"";
			var directory = Path.GetDirectoryName (path);
			if (!Directory.Exists (directory)) { Directory.CreateDirectory (directory); }
			CMD.GenerateProcess(cmd, args, true, onOutput: onOutput, onError: onError);
		}
		
		public static void GetRepositoryFile(string user, string repo, string file, string assetsPath, Action<string> onOutput = null, Action<string> onError = null)
		{
			var cmd = "curl";
			var path = Path.Combine(Directory.GetCurrentDirectory(), assetsPath).FixOSPath();
			var args = $"\"https://raw.githubusercontent.com/{user}/{repo}/{file}\" -o \"" + path + "\"";
			var directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory)) { Directory.CreateDirectory(directory); }
			CMD.GenerateProcess(cmd, args, true, onOutput: onOutput, onError: onError);
		}
	}
}
