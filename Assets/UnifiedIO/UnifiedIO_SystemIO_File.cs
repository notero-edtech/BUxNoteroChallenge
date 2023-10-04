#if !NETFX_CORE && !UNITY_WEBPLAYER && !UNITY_WEBGL
namespace UnifiedIO
{
	public static class File
	{
		/// <summary>
		/// Checks if path exists as a file.
		/// </summary>
		public static bool Exists(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			return System.IO.File.Exists(path);
		}

		/// <summary>
		/// Creates an empty file at path.
		/// Intermediate folders of path must exist.
		/// </summary>
		public static void CreateEmpty(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			string dirPath = System.IO.Path.GetDirectoryName(path);

			if (System.IO.Directory.Exists(dirPath)) {
				System.IO.File.Create(path).Dispose();
			} else {
				var e = new System.IO.FileNotFoundException("Unable to find directory: " + dirPath);
				throw e;
			}
		}

		/// <summary>
		/// Renames file at path with a new name.
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
				System.IO.File.Move(path, System.IO.Path.Combine(newPath, newName));
			} catch (System.IO.DirectoryNotFoundException) {
				throw new System.IO.FileNotFoundException();
			}
		}

		/// <summary>
		/// Changes path of file to be "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Move(string path, string destinationPath)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);
			if (System.IO.File.Exists(destinationPath)) {
				System.IO.File.Delete(destinationPath);
			}
			System.IO.File.Move(path, destinationPath);
		}

		/// <summary>
		/// Copies file as "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Copy(string path, string destinationPath)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);
			if (System.IO.File.Exists(destinationPath))  {
				System.IO.File.Delete(destinationPath);
			}
			System.IO.File.Copy(path, destinationPath);
		}

		/// <summary>
		/// Moves file to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void MoveInside(string path, string destinationPath)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);
			string name = System.IO.Path.GetFileName(path);
			string newPath = System.IO.Path.Combine(destinationPath, name);
			if (System.IO.File.Exists(newPath)) {
				System.IO.File.Delete(newPath);
			}
			System.IO.File.Move(path, newPath);
		}

		/// <summary>
		/// Copies file to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void CopyInside(string path, string destinationPath)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			destinationPath = SystemIO.Internal.Path.GetFullPath(destinationPath);
			string name = System.IO.Path.GetFileName(path);
			string newPath = System.IO.Path.Combine(destinationPath, name);
			if (System.IO.File.Exists(newPath)) {
				System.IO.File.Delete(newPath);
			}
			System.IO.File.Copy(path, newPath);
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		public static void Delete(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			if (System.IO.File.Exists(path)) {
				System.IO.File.Delete(path);
			} else {
				var e = new System.IO.FileNotFoundException("Unable to find the specified file: " + path);
				throw e;
			}
		}

		/// <summary>
		/// Creates a read stream over the file at path.
		/// </summary>
		public static System.IO.Stream GetReadStream(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			try {
				return System.IO.File.OpenRead(path);
			} catch (System.IO.DirectoryNotFoundException) {
				throw new System.IO.FileNotFoundException();
			}
		}

		/// <summary>
		/// Creates a write stream over the file at path.
		/// </summary>
		public static System.IO.Stream GetWriteStream(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			try {
				return System.IO.File.Create(path);
			} catch (System.IO.DirectoryNotFoundException) {
				throw new System.IO.FileNotFoundException();
			}
		}

		/// <summary>
		/// Creates an append write stream over the file at path.
		/// </summary>
		public static System.IO.Stream GetAppendStream(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			try {
				return System.IO.File.Open(path, System.IO.FileMode.Append);
			} catch (System.IO.DirectoryNotFoundException) {
				throw new System.IO.FileNotFoundException();
			}
		}

		/// <summary>
		/// Reads content of file at path as bytes. Accepts an optional file reading position and number of bytes to read (0 to read until end of file).
		/// </summary>
		public static byte[] ReadBytes(string path, int position = 0, int nBytes = 0)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			byte[] buffer = null;
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
			{
				if (position > 0) {
					fs.Position = position;
				}

				int bytesLeftInFile = (int)(fs.Length - fs.Position);
				if (nBytes == 0 || nBytes > bytesLeftInFile) {
					nBytes = bytesLeftInFile;
				}

				if (nBytes < 0) {
					nBytes = 0;
				}

				buffer = new byte[nBytes];
				readStreamToBuffer(fs, buffer);
			}
			return buffer;
		}

		/// <summary>
		/// Creates (or fully overwrites) a file at path with the bytes in "content".
		/// </summary>
		public static void WriteBytes(string path, byte[] content)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
			{
				fs.Write(content, 0, content.Length);
			}
		}

		/// <summary>
		/// Opens (or creates) a file at path and writes the bytes in "content" at file offset "position".
		/// </summary>
		public static void WriteBytes(string path, byte[] content, int position)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
			{
				if (position > 0) {
					fs.Position = position;
				}
				fs.Write(content, 0, content.Length);
			}
		}

		/// <summary>
		/// Opens or creates a file at path and writes the bytes in "content" at the end of the file.
		/// </summary>
		public static void AppendBytes(string path, byte[] bytes)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			using (var fs = new System.IO.FileStream(path, System.IO.FileMode.Append, System.IO.FileAccess.Write))
			{
				fs.Write(bytes, 0, bytes.Length);
			}
		}

		/// <summary>
		/// Reads content of file at path as text.
		/// </summary>
		public static string ReadText(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			using (var sr = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
			{
				return sr.ReadToEnd();
			}
		}

		/// <summary>
		/// Creates a file at path, containing the text in "content".
		/// </summary>
		public static void WriteText(string path, string content)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			using (var sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8))
			{
				sw.Write(content);
			}
		}

		/// <summary>
		/// Opens or creates a file at path and writes the text in "content" at the end of the file.
		/// </summary>
		public static void AppendText(string path, string content)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);
			using (var sw = new System.IO.StreamWriter(path, true, System.Text.Encoding.UTF8))
			{
				sw.Write(content);
			}
		}

		/// <summary>
		/// Reads content of file at path as lines of text.
		/// </summary>
		public static System.Collections.Generic.IList<string> ReadLines(string path)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			var lines = new System.Collections.Generic.List<string>();
			string line;

			using (var sr = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
			{
				while ((line = sr.ReadLine()) != null) {
					lines.Add(line);
				}
			}

			return lines.ToArray();
		}

		/// <summary>
		/// Creates a file at path, containing the lines of text in "content".
		/// </summary>
		public static void WriteLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			using (var sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.UTF8))
			{
				foreach (string line in content) {
					sw.WriteLine(line);
				}
			}
		}

		/// <summary>
		/// Opens or creates a file at path and writes the lines of text in "content" at the end of the file.
		/// </summary>
		public static void AppendLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			path = SystemIO.Internal.Path.GetFullPath(path);

			using (var sw = new System.IO.StreamWriter(path, true, System.Text.Encoding.UTF8))
			{
				foreach (string line in content) {
					sw.WriteLine(line);
				}
			}
		}

		private static int readStreamToBuffer(System.IO.FileStream fs, byte[] buffer)
		{
			int readOffset = 0;
			int nBytesLeft = buffer.Length;

			do {
				// "Read" can read any number of bytes, even if more exist, so do a loop until everything is read.
				int n = fs.Read(buffer, readOffset, nBytesLeft);
				readOffset += n;
				nBytesLeft -= n;
			} while (nBytesLeft > 0 && fs.Position < fs.Length);

			return readOffset;
		}

		// The following methods are simpler but don't compile for Windows Phone. Here simply for reference.

		/*
		public static void WriteText(string path, string content)
		{
			path = SystemIO.Path.GetFullPath(path);
			System.IO.File.WriteAllText(path, content);
		}

		public static string ReadText(string path)
		{
			path = SystemIO.Path.GetFullPath(path);
			return System.IO.File.ReadAllText(path);
		}

		public static void WriteBytes(string path, byte[] content)
		{
			path = SystemIO.Path.GetFullPath(path);
			System.IO.File.WriteAllBytes(path, content);
		}

		public static byte[] ReadBytes(string path)
		{
			path = SystemIO.Path.GetFullPath(path);
			return System.IO.File.ReadAllBytes(path);
		}

		public static void WriteLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			path = SystemIO.Path.GetFullPath(path);

			string[] lines;
			if (content.GetType() == typeof(string[])) {
				lines = (string[]) content;
			} else {
				lines = content.ToArray();
			}
			System.IO.File.WriteAllLines(path, lines);
		}

		public static System.Collections.Generic.IList<string> ReadLines(string path)
		{
			path = SystemIO.Path.GetFullPath(path);
			return System.IO.File.ReadAllLines(path);
		}
		*/
	}
}
#endif