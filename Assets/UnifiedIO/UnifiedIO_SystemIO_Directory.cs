#if !NETFX_CORE && !UNITY_WEBPLAYER && !UNITY_WEBGL
namespace UnifiedIO
{
	public static class Directory
	{
		/// <summary>
		/// Checks if path exists as a folder.
		/// </summary>
		public static bool Exists(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			return System.IO.Directory.Exists(path);
		}

		/// <summary>
		/// Creates all intermediate folders in path.
		/// Skips already existing folders.
		/// </summary>
		public static void Create(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			if (!System.IO.Directory.Exists(path)) {
				System.IO.Directory.CreateDirectory(path);
			}
		}

		/// <summary>
		/// Renames folder at path with a new name.
		/// Fails if "newName" already exists.
		/// </summary>
		public static void Rename(string path, string newName)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			if (newName.Contains("/") || newName.Contains("\\")) {
				throw new System.IO.FileNotFoundException();
			}

			string newPath = System.IO.Path.GetDirectoryName(path);
			try {
				System.IO.Directory.Move(path, System.IO.Path.Combine(newPath, newName));
			} catch (System.IO.DirectoryNotFoundException) {
				throw new System.IO.FileNotFoundException();
			}
		}

		/// <summary>
		/// Changes path of folder to be "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Move(string sourcePath, string destinationPath)
		{
			sourcePath = SystemIO.Internal.Path.GetFullPath(sourcePath);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);

			if (!areSameDirectory(sourcePath, destinationPath) && !isSubdirectory(destinationPath, sourcePath))
			{
				if (!System.IO.Directory.Exists(destinationPath)) {
					System.IO.Directory.CreateDirectory(destinationPath);
				}
				move(sourcePath, destinationPath);
			}
			else
			{
				throw new System.IO.IOException();
			}
		}

		/// <summary>
		/// Copies folder as "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Copy(string sourcePath, string destinationPath)
		{
			sourcePath = SystemIO.Internal.Path.GetFullPath(sourcePath);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);

			if (!areSameDirectory(sourcePath, destinationPath) && !isSubdirectory(destinationPath, sourcePath))
			{
				if (!System.IO.Directory.Exists(destinationPath)) {
					System.IO.Directory.CreateDirectory(destinationPath);
				}
				copy(sourcePath, destinationPath);
			}
			else
			{
				throw new System.IO.IOException();
			}
		}

		/// <summary>
		/// Moves folder to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void MoveInside(string sourcePath, string destinationPath)
		{
			sourcePath = SystemIO.Internal.Path.GetFullPath(sourcePath);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);
			string name = System.IO.Path.GetFileName(sourcePath);
			string destDir = System.IO.Path.Combine(destinationPath, name);

			if (!areSameDirectory(sourcePath, destDir) && !isSubdirectory(destDir, sourcePath))
			{
				if (!System.IO.Directory.Exists(destDir)) {
					System.IO.Directory.CreateDirectory(destDir);
				}
				move(sourcePath, destDir);
			}
			else
			{
				throw new System.IO.IOException();
			}
		}

		/// <summary>
		/// Copies folder to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void CopyInside(string sourcePath, string destinationPath)
		{
			sourcePath = SystemIO.Internal.Path.GetFullPath(sourcePath);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);

			string name = System.IO.Path.GetFileName(sourcePath);
			string destDir = System.IO.Path.Combine(destinationPath, name);

			if (!areSameDirectory(sourcePath, destDir) && !isSubdirectory(destDir, sourcePath))
			{
				if (!System.IO.Directory.Exists(destDir)) {
					System.IO.Directory.CreateDirectory(destDir);
				}
				copy(sourcePath, destDir);
			}
			else
			{
				throw new System.IO.IOException();
			}
		}

		/// <summary>
		/// Deletes a folder and all its contents.
		/// </summary>
		public static void Delete(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			if (System.IO.Directory.Exists(path)) {
				System.IO.Directory.Delete(path, true);
			} else {
				var e = new System.IO.FileNotFoundException("Unable to find the specified directory: " + path);
				throw e;
			}
		}

		/// <summary>
		/// Empties a folder.
		/// </summary>
		public static void DeleteContents(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			foreach (string dir in System.IO.Directory.GetDirectories(path)) {
				System.IO.Directory.Delete(dir, true);
			}
			foreach (string file in System.IO.Directory.GetFiles(path)) {
				System.IO.File.Delete(file);
			}
		}

		/// <summary>
		/// Gets an array of files inside a folder.
		/// Accepts a search pattern that can use * and ?.
		/// Accepts a SearchOption for recursive enumeration or top directory only (default).
		/// </summary>
		public static string[] GetFiles(string path, string searchPattern = null,
			System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			string[] items;
			if (searchPattern != null) {
				items = System.IO.Directory.GetFiles(path, searchPattern, searchOption);
			} else {
				items = System.IO.Directory.GetFiles(path, "*", searchOption);
			}

			for (int j = 0; j < items.Length; j++) {
				items[j] = items[j].Substring(path.Length + 1, items[j].Length - path.Length - 1);
			}

			return items;
		}

		[System.Obsolete("Deprecated because it never actually returned an \"enumeration\". Please use GetFiles instead.")]
		public static string[] EnumerateFiles(string path, string searchPattern = null,
			System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
		{
			return GetFiles(path, searchPattern, searchOption);
		}

		/// <summary>
		/// Gets an array of folders inside a folder.
		/// Accepts a search pattern that can use * and ?.
		/// Accepts a SearchOption for recursive enumeration or top directory only (default).
		/// </summary>
		public static string[] GetDirectories(string path, string searchPattern = null,
			System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			string[] items;
			if (searchPattern != null) {
				items = System.IO.Directory.GetDirectories(path, searchPattern, searchOption);
			} else {
				items = System.IO.Directory.GetDirectories(path, "*", searchOption);
			}

			for (int j = 0; j < items.Length; j++) {
				items[j] = items[j].Substring(path.Length + 1, items[j].Length - path.Length - 1);
			}

			return items;
		}

		[System.Obsolete("Deprecated because it never actually returned an \"enumeration\". Please use GetDirectories instead.")]
		public static string[] EnumerateDirectories(string path, string searchPattern = null,
			System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
		{
			return GetDirectories(path, searchPattern, searchOption);
		}

		private static void move(string sourcePath, string destinationPath)
		{
			string [] files = System.IO.Directory.GetFiles(sourcePath);

			foreach (string filePath in files)
			{
				string name = System.IO.Path.GetFileName(filePath);
				string dest = System.IO.Path.Combine(destinationPath, name);
				if (System.IO.File.Exists(dest)) {
					System.IO.File.Delete(dest);
				}
				System.IO.File.Move(filePath, dest);
			}

			string [] directories = System.IO.Directory.GetDirectories(sourcePath);

			foreach (string dirPath in directories)
			{
				string name = System.IO.Path.GetFileName(dirPath);
				string destDir = System.IO.Path.Combine(destinationPath, name);
				if (!System.IO.Directory.Exists(destDir)) {
					System.IO.Directory.CreateDirectory(destDir);
				}
				move(dirPath, destDir);
			}

			if (isEmpty(sourcePath))
			{
				System.IO.Directory.Delete(sourcePath, true);
			}
		}

		private static void copy(string sourcePath, string destinationPath)
		{
			string [] files = System.IO.Directory.GetFiles(sourcePath);

			foreach (string filePath in files)
			{
				string name = System.IO.Path.GetFileName(filePath);
				string dest = System.IO.Path.Combine(destinationPath, name);
				if (System.IO.File.Exists(dest)) {
					System.IO.File.Delete(dest);
				}
				System.IO.File.Copy(filePath, dest);
			}

			string [] directories = System.IO.Directory.GetDirectories(sourcePath);

			foreach (string dirPath in directories)
			{
				string name = System.IO.Path.GetFileName(dirPath);
				string destDir = System.IO.Path.Combine(destinationPath, name);
				if (!System.IO.Directory.Exists(destDir)) {
					System.IO.Directory.CreateDirectory(destDir);
				}
				copy(dirPath, destDir);
			}
		}

		private static bool areSameDirectory(string dir1, string dir2)
		{
			var di1 = new System.IO.DirectoryInfo(dir1);
			var di2 = new System.IO.DirectoryInfo(dir2);
			return di2.Parent.FullName == di1.FullName;
		}

		private static bool isSubdirectory(string childCandidate, string parentCandidate)
		{
			var di1 = new System.IO.DirectoryInfo(parentCandidate);
			var di2 = new System.IO.DirectoryInfo(childCandidate);

			while (di2.Parent != null) {
				if (di2.Parent.FullName == di1.FullName) {
					return true;
				}
				di2 = di2.Parent;
			}

			return false;
		}

		private static bool isEmpty(string path)
		{
			return System.IO.Directory.GetFileSystemEntries(path).Length == 0;
			// better, but .NET 4.0 only:
			// System.Collections.Generic.IEnumerable<string> items = System.IO.Directory.EnumerateFileSystemEntries(path);
			// using (var item = items.GetEnumerator())
			// {
			// 	return !item.MoveNext();
			// }
		}
	}
}
#endif