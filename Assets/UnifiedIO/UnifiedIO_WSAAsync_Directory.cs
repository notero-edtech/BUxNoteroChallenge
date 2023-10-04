#if NETFX_CORE
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace UnifiedIO.WSAAsync
{
	public static class Directory
	{
		public static async Task<bool> Exists(string path)
		{
			try
			{
				string name = Path.GetFileName(path);
				string directoryPath = Path.GetDirectoryName(path);
				StorageFolder directory = await NavigateTo(directoryPath);

				await directory.GetFolderAsync(name);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static async Task Rename(string path, string newName)
		{
			StorageFolder directory = await NavigateTo(path);
			await directory.RenameAsync(newName, NameCollisionOption.FailIfExists);
		}

		public static async Task Delete(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await NavigateTo(directoryPath);

			directory = await directory.GetFolderAsync(name);
			await directory.DeleteAsync();
		}

		public static async Task DeleteContents(string path)
		{
			StorageFolder directory = await NavigateTo(path);

			foreach (var item in await directory.GetItemsAsync()) {
				await item.DeleteAsync();
			}
		}

		public static async Task Create(string path)
		{
			await CreateAndNavigateTo(path);
		}

		public static async Task<string[]> EnumerateFiles(string path, string searchPattern = null,
			SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			StorageFolder directory = await NavigateTo(path);

			if (searchOption == SearchOption.TopDirectoryOnly) {
				if (searchPattern != null) {
					return getStorageItemNamesMatchingPattern(await directory.GetFilesAsync(), searchPattern).ToArray();
				} else {
					return getStorageItemNames(await directory.GetFilesAsync());
				}
			} else {
				var pathsList = new List<string>();
				if (searchPattern != null) {
					await appendStorageFilePathsMatchingPatternRecursively(directory, directory, getRegexSearchPatternFromString(searchPattern), pathsList);
				} else {
					await appendStorageFilePathsRecursively(directory, directory, pathsList);
				}
				return pathsList.ToArray();
			}
		}

		public static async Task<string[]> EnumerateDirectories(string path, string searchPattern = null,
			SearchOption searchOption = SearchOption.TopDirectoryOnly)
		{
			StorageFolder directory = await NavigateTo(path);

			if (searchOption == SearchOption.TopDirectoryOnly) {
				if (searchPattern != null) {
					return getStorageItemNamesMatchingPattern(await directory.GetFoldersAsync(), searchPattern).ToArray();
				} else {
					return getStorageItemNames(await directory.GetFoldersAsync());
				}
			} else {
				var pathsList = new List<string>();
				if (searchPattern != null) {
					await appendStorageFolderPathsMatchingPatternRecursively(directory, directory, getRegexSearchPatternFromString(searchPattern), pathsList);
				} else {
					await appendStorageFolderPathsRecursively(directory, directory, pathsList);
				}
				return pathsList.ToArray();
			}
		}

		public static async Task Move(string sourcePath, string destinationPath)
		{
			string newName = Path.GetFileName(destinationPath);
			string destinationDirectoryPath = Path.GetDirectoryName(destinationPath);
			StorageFolder source = await NavigateTo(sourcePath);
			await moveInside(source, destinationDirectoryPath, newName);
		}

		public static async Task MoveInside(string sourcePath, string destinationPath)
		{
			StorageFolder source = await NavigateTo(sourcePath);
			await moveInside(source, destinationPath, source.DisplayName);
		}

		public static async Task Copy(string sourcePath, string destinationPath)
		{
			string newName = Path.GetFileName(destinationPath);
			string destinationDirectoryPath = Path.GetDirectoryName(destinationPath);
			StorageFolder source = await NavigateTo(sourcePath);
			await copyInside(source, destinationDirectoryPath, newName);
		}

		public static async Task CopyInside(string sourcePath, string destinationPath)
		{
			StorageFolder source = await NavigateTo(sourcePath);
			await copyInside(source, destinationPath, source.DisplayName);
		}

		// assumes argument is a relative path ending in a folder
		public static async Task<StorageFolder> NavigateTo(string path)
		{
			string[] directoryNames = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			StorageFolder current = Internal.Path.BaseFolder;

			foreach (string directoryName in directoryNames) {
				current = await current.GetFolderAsync(directoryName);
			}

			return current;
		}

		// assumes argument is a relative path ending in a folder
		public static async Task<StorageFolder> CreateAndNavigateTo(string path)
		{
			string[] directoryNames = path.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
			StorageFolder current = Internal.Path.BaseFolder;

			foreach (string directoryName in directoryNames) {
				current = await current.CreateFolderAsync(directoryName, CreationCollisionOption.OpenIfExists);
			}

			return current;
		}

		private static Regex getRegexSearchPatternFromString(string searchPattern)
		{
			string regexSearchPattern = Regex.Escape(searchPattern);
			regexSearchPattern = regexSearchPattern.Replace("\\*", ".*");
			regexSearchPattern = regexSearchPattern.Replace("\\?", ".?");
			return new Regex("^" + regexSearchPattern + "$");
		}

		private static List<string> getStorageItemNamesMatchingPattern(IReadOnlyList<IStorageItem> items, string searchPattern)
		{
			Regex regex = getRegexSearchPatternFromString(searchPattern);

			var filenames = new List<string>();

			foreach (IStorageItem item in items) {
				if (regex.IsMatch(item.Name)) {
					filenames.Add(item.Name);
				}
			}

			return filenames;
		}

		private static async Task appendStorageFilePathsMatchingPatternRecursively(
			StorageFolder topmostFolder, StorageFolder parentFolder, Regex pattern, List<string> paths)
		{
			int parentPathLength = topmostFolder.Path.Length;

			foreach (StorageFile file in await parentFolder.GetFilesAsync()) {
				if (pattern.IsMatch(file.Name)) {
					paths.Add(file.Path.Substring(parentPathLength + 1, file.Path.Length - parentPathLength - 1));
				}
			}

			foreach (StorageFolder folder in await parentFolder.GetFoldersAsync()) {
				await appendStorageFilePathsMatchingPatternRecursively(topmostFolder, folder, pattern, paths);
			}
		}

		private static async Task appendStorageFolderPathsMatchingPatternRecursively(
			StorageFolder topmostFolder, StorageFolder parentFolder, Regex pattern, List<string> paths)
		{
			int parentPathLength = topmostFolder.Path.Length;
			IReadOnlyList<IStorageFolder> folders = await parentFolder.GetFoldersAsync();

			foreach (StorageFolder folder in folders) {
				if (pattern.IsMatch(folder.Name)) {
					paths.Add(folder.Path.Substring(parentPathLength + 1, folder.Path.Length - parentPathLength - 1));
				}
			}

			foreach (StorageFolder folder in folders) {
				await appendStorageFolderPathsMatchingPatternRecursively(topmostFolder, folder, pattern, paths);
			}
		}

		private static string[] getStorageItemNames(IReadOnlyList<IStorageItem> items)
		{
			string[] filenames = new string[items.Count];

			int i = 0;
			foreach (IStorageItem item in items) {
				filenames[i++] = item.Name;
			}

			return filenames;
		}

		private static async Task appendStorageFilePathsRecursively(StorageFolder topmostFolder, StorageFolder parentFolder, List<string> paths)
		{
			int parentPathLength = topmostFolder.Path.Length;

			foreach (StorageFile file in await parentFolder.GetFilesAsync()) {
				paths.Add(file.Path.Substring(parentPathLength + 1, file.Path.Length - parentPathLength - 1));
			}

			foreach (StorageFolder folder in await parentFolder.GetFoldersAsync()) {
				await appendStorageFilePathsRecursively(topmostFolder, folder, paths);
			}
		}

		private static async Task appendStorageFolderPathsRecursively(StorageFolder topmostFolder, StorageFolder parentFolder, List<string> paths)
		{
			int parentPathLength = topmostFolder.Path.Length;
			IReadOnlyList<IStorageFolder> folders = await parentFolder.GetFoldersAsync();

			foreach (StorageFolder folder in folders) {
				paths.Add(folder.Path.Substring(parentPathLength + 1, folder.Path.Length - parentPathLength - 1));
			}

			foreach (StorageFolder folder in folders) {
				await appendStorageFolderPathsRecursively(topmostFolder, folder, paths);
			}
		}

		private static async Task moveInside(StorageFolder source, string destinationPath, string name)
		{
			StorageFolder destination = await NavigateTo(destinationPath);

			if (destination.Path != source.Path && !isSubdirectory(destination, source)) {
				StorageFolder targetFolder = await destination.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
				await moveContents(source, targetFolder);
			} else {
				throw new System.IO.IOException();
			}
		}

		private static async Task moveContents(StorageFolder source, StorageFolder destination)
		{
			// For each file, move into destination
			foreach (var file in await source.GetFilesAsync()) {
				await file.MoveAsync(destination, file.Name, NameCollisionOption.ReplaceExisting);
			}

			// For each directory, recursively call with new destination
			foreach (var directory in await source.GetFoldersAsync()) {
				StorageFolder targetFolder = await destination.CreateFolderAsync(directory.DisplayName, CreationCollisionOption.OpenIfExists);
				await moveContents(directory, targetFolder);
			}

			if (await isEmpty(source)) {
				await source.DeleteAsync();
			}
		}

		private static async Task copyInside(StorageFolder source, string destinationPath, string name)
		{
			StorageFolder destination = await NavigateTo(destinationPath);

			if (destination.Path != source.Path && !isSubdirectory(destination, source)) {
				StorageFolder targetFolder = await destination.CreateFolderAsync(name, CreationCollisionOption.OpenIfExists);
				await copyContents(source, targetFolder);
			} else {
				throw new System.IO.IOException();
			}
		}

		private static async Task copyContents(StorageFolder source, StorageFolder destination)
		{
			// For each file, copy into destination
			foreach (var file in await source.GetFilesAsync()) {
				await file.CopyAsync(destination, file.Name, NameCollisionOption.ReplaceExisting);
			}

			// For each directory, recursively call with new destination
			foreach (var directory in await source.GetFoldersAsync()) {
				StorageFolder targetFolder = await destination.CreateFolderAsync(directory.DisplayName, CreationCollisionOption.OpenIfExists);
				await copyContents(directory, targetFolder);
			}
		}

		private static bool isSubdirectory(StorageFolder childCandidate, StorageFolder parentCandidate)
		{
			return childCandidate.Path.StartsWith(parentCandidate.Path);
		}

		private static async Task<bool> isEmpty(StorageFolder directory)
		{
			var items = await directory.GetItemsAsync(0, 1);
			return items.Count == 0;
		}
	}
}
#endif