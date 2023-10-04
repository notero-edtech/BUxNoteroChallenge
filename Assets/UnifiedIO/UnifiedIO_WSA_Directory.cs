#if NETFX_CORE
using System.Threading.Tasks;

#if !WINDOWS_UWP
namespace System.IO
{
	public enum SearchOption
	{
		AllDirectories,
		TopDirectoryOnly,
	}
}
#endif

namespace UnifiedIO
{
	public static class Directory
	{
		/// <summary>
		/// Checks if path exists as a folder.
		/// </summary>
		public static bool Exists(string path)
		{
			return Task.Run(() => WSAAsync.Directory.Exists(path)).Result;
		}

		/// <summary>
		/// Creates all intermediate folders in path.
		/// Skips already existing folders.
		/// </summary>
		public static void Create(string path)
		{
			Task.Run(() => WSAAsync.Directory.Create(path)).Wait();
		}

		/// <summary>
		/// Renames folder at path with a new name.
		/// Fails if "newName" already exists.
		/// </summary>
		public static void Rename(string path, string newName)
		{
			Task.Run(() => WSAAsync.Directory.Rename(path, newName)).Wait();
		}

		/// <summary>
		/// Changes path of folder to be "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Move(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.Directory.Move(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Copies folder as "destinationPath".
		/// Intermediate folders of "destinationPath" must exist.
		/// Fails if "destinationPath" already exists.
		/// </summary>
		public static void Copy(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.Directory.Copy(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Moves folder to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void MoveInside(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.Directory.MoveInside(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Copies folder to the inside of "destinationPath".
		/// Fails if file/folder of the same name already exists inside "destinationPath".
		/// </summary>
		public static void CopyInside(string path, string destinationPath)
		{
			Task.Run(() => WSAAsync.Directory.CopyInside(path, destinationPath)).Wait();
		}

		/// <summary>
		/// Deletes a folder and all its contents.
		/// </summary>
		public static void Delete(string path)
		{
			Task.Run(() => WSAAsync.Directory.Delete(path)).Wait();
		}

		/// <summary>
		/// Empties a folder.
		/// </summary>
		public static void DeleteContents(string path)
		{
			Task.Run(() => WSAAsync.Directory.DeleteContents(path)).Wait();
		}

		/// <summary>
		/// Gets an array of files inside a folder.
		/// Accepts a search pattern that can use * and ?.
		/// Accepts a SearchOption for recursive enumeration or top directory only (default).
		/// </summary>
		public static string[] GetFiles(string path, string searchPattern = null,
			System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
		{
			return Task.Run(() => WSAAsync.Directory.EnumerateFiles(path, searchPattern, searchOption)).Result;
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
			return Task.Run(() => WSAAsync.Directory.EnumerateDirectories(path, searchPattern, searchOption)).Result;
		}

		[System.Obsolete("Deprecated because it never actually returned an \"enumeration\". Please use GetDirectories instead.")]
		public static string[] EnumerateDirectories(string path, string searchPattern = null,
			System.IO.SearchOption searchOption = System.IO.SearchOption.TopDirectoryOnly)
		{
			return GetDirectories(path, searchPattern, searchOption);
		}
	}
}
#endif