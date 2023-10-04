#if NETFX_CORE
using System.Threading.Tasks;

namespace UnifiedIO
{
	public static class File
	{
		/// <summary>
		/// Checks if path exists as a file.
		/// </summary>
		public static bool Exists(string path)
		{
			return Task.Run(() => WSAAsync.File.Exists(path)).Result;
		}

		/// <summary>
		/// Creates an empty file at path.
		/// Intermediate folders of path must exist.
		/// </summary>
		public static void CreateEmpty(string path)
		{
			Task.Run(() => WSAAsync.File.CreateEmpty(path)).Wait();
		}

		/// <summary>
		/// Renames file at path with a new name.
		/// Fails if "newName" already exists.
		/// </summary>
		public static void Rename(string path, string newName)
		{
			Task.Run(() => WSAAsync.File.Rename(path, newName)).Wait();
		}

		/// <summary>
		/// Changes path of file to be "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Move(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.File.Move(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Copies file as "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Copy(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.File.Copy(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Moves file to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void MoveInside(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.File.MoveInside(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Copies file to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void CopyInside(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.File.CopyInside(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Deletes a file.
		/// </summary>
		public static void Delete(string path)
		{
			Task.Run(() => WSAAsync.File.Delete(path)).Wait();
		}

		/// <summary>
		/// Creates a read stream over the file at path.
		/// </summary>
		public static System.IO.Stream GetReadStream(string path)
		{
			return Task.Run(() => WSAAsync.File.GetReadStream(path)).Result;
		}

		/// <summary>
		/// Creates a write stream over the file at path.
		/// </summary>
		public static System.IO.Stream GetWriteStream(string path)
		{
			return Task.Run(() => WSAAsync.File.GetWriteStream(path)).Result;
		}

		/// <summary>
		/// Creates an append write stream over the file at path.
		/// </summary>
		public static System.IO.Stream GetAppendStream(string path)
		{
			return Task.Run(() => WSAAsync.File.GetAppendStream(path)).Result;
		}

		/// <summary>
		/// Reads content of file at path as bytes. Accepts an optional file reading position and number of bytes to read (0 to read until end of file).
		/// </summary>
		public static byte[] ReadBytes(string path, int position = 0, int nBytes = 0)
		{
			return Task.Run(() => WSAAsync.File.ReadBytes(path, position, nBytes)).Result;
		}

		/// <summary>
		/// Creates (or fully overwrites) a file at path with the bytes in "content".
		/// </summary>
		public static void WriteBytes(string path, byte[] content)
		{
			Task.Run(() => WSAAsync.File.WriteBytes(path, content)).Wait();
		}

		/// <summary>
		/// Opens (or creates) a file at path and writes the bytes in "content" at file offset "position".
		/// </summary>
		public static void WriteBytes(string path, byte[] content, int position)
		{
			Task.Run(() => WSAAsync.File.WriteBytes(path, content, position)).Wait();
		}

		/// <summary>
		/// Opens or creates a file at path and writes the bytes in "content" at the end of the file.
		/// </summary>
		public static void AppendBytes(string path, byte[] content)
		{
			Task.Run(() => WSAAsync.File.AppendBytes(path, content)).Wait();
		}

		/// <summary>
		/// Reads content of file at path as text.
		/// </summary>
		public static string ReadText(string path)
		{
			return Task.Run(() => WSAAsync.File.ReadText(path)).Result;
		}

		/// <summary>
		/// Creates a file at path, containing the text in "content".
		/// </summary>
		public static void WriteText(string path, string content)
		{
			Task.Run(() => WSAAsync.File.WriteText(path, content)).Wait();
		}

		/// <summary>
		/// Opens or creates a file at path and writes the text in "content" at the end of the file.
		/// </summary>
		public static void AppendText(string path, string content)
		{
			Task.Run(() => WSAAsync.File.AppendText(path, content)).Wait();
		}

		/// <summary>
		/// Reads content of file at path as lines of text.
		/// </summary>
		public static System.Collections.Generic.IList<string> ReadLines(string path)
		{
			return Task.Run(() => WSAAsync.File.ReadLines(path)).Result;
		}

		/// <summary>
		/// Creates a file at path, containing the lines of text in "content".
		/// </summary>
		public static void WriteLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			Task.Run(() => WSAAsync.File.WriteLines(path, content)).Wait();
		}

		/// <summary>
		/// Opens or creates a file at path and writes the lines of text in "content" at the end of the file.
		/// </summary>
		public static void AppendLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			Task.Run(() => WSAAsync.File.AppendLines(path, content)).Wait();
		}
	}
}
#endif