#if NETFX_CORE
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace UnifiedIO.WSAAsync
{
	public static class File
	{
		public static async Task<bool> Exists(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);

			try {
				StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);
				await directory.GetFileAsync(name);
				return true;
			} catch (Exception) {
				return false;
			}
		}

		public static async Task CreateEmpty(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);
			await directory.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
		}

		public static async Task Rename(string path, string newName)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.GetFileAsync(name);
			await file.RenameAsync(newName, NameCollisionOption.FailIfExists);
		}

		public static async Task Move(string sourcePath, string destinationPath)
		{
			string name = Path.GetFileName(sourcePath);
			string sourceDirectoryPath = Path.GetDirectoryName(sourcePath);
			string newName = Path.GetFileName(destinationPath);
			string destinationDirectoryPath = Path.GetDirectoryName(destinationPath);
			await move(name, sourceDirectoryPath, newName, destinationDirectoryPath);
		}

		public static async Task MoveInside(string sourcePath, string destinationPath)
		{
			string name = Path.GetFileName(sourcePath);
			string sourceDirectoryPath = Path.GetDirectoryName(sourcePath);
			await move(name, sourceDirectoryPath, name, destinationPath);
		}

		public static async Task Copy(string sourcePath, string destinationPath)
		{
			string name = Path.GetFileName(sourcePath);
			string sourceDirectoryPath = Path.GetDirectoryName(sourcePath);
			string newName = Path.GetFileName(destinationPath);
			string destinationDirectoryPath = Path.GetDirectoryName(destinationPath);
			await copy(name, sourceDirectoryPath, newName, destinationDirectoryPath);
		}

		public static async Task CopyInside(string sourcePath, string destinationPath)
		{
			string name = Path.GetFileName(sourcePath);
			string sourceDirectoryPath = Path.GetDirectoryName(sourcePath);
			await copy(name, sourceDirectoryPath, name, destinationPath);
		}

		public static async Task Delete(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.GetFileAsync(name);
			await file.DeleteAsync();
		}

		public static async Task<Stream> GetReadStream(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.GetFileAsync(name);
			return await file.OpenStreamForReadAsync();
		}

		public static async Task<Stream> GetWriteStream(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			return await file.OpenStreamForWriteAsync();
		}

		public static async Task<Stream> GetAppendStream(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
			var stream = await file.OpenStreamForWriteAsync();
			stream.Seek(0, SeekOrigin.End);
			return stream;
		}

		public static async Task<byte[]> ReadBytes(string path, int position = 0, int nBytes = 0)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.GetFileAsync(name);
			byte[] bytes = null;

			using (Stream stream = await file.OpenStreamForReadAsync())
			{
				if (position > 0) {
					stream.Position = position;
				}

				int bytesLeftInFile = (int) (stream.Length - stream.Position);
				if (nBytes == 0 || nBytes > bytesLeftInFile) {
					nBytes = bytesLeftInFile;
				}

				if (nBytes < 0) {
					nBytes = 0;
				}

				bytes = new byte[nBytes];
				readStreamToBuffer(stream, bytes);
			}

			return bytes;
		}

		public static async Task WriteBytes(string path, byte[] content)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteBytesAsync(file, content);
		}

		public static async Task WriteBytes(string path, byte[] content, int position)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
			using (Stream f = await file.OpenStreamForWriteAsync())
			{
				if (position > 0) {
					f.Position = position;
				}
				await f.WriteAsync(content, 0, content.Length);
			}
		}

		public static async Task AppendBytes(string path, byte[] content)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);

			using (Stream f = await file.OpenStreamForWriteAsync())
			{
				f.Seek(0, SeekOrigin.End);
				await f.WriteAsync(content, 0, content.Length);
			}
		}

		public static async Task<string> ReadText(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.GetFileAsync(name);
			return await FileIO.ReadTextAsync(file);
		}

		public static async Task WriteText(string path, string content)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteTextAsync(file, content);
		}

		public static async Task AppendText(string path, string content)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
			await FileIO.AppendTextAsync(file, content);
		}

		public static async Task<System.Collections.Generic.IList<string>> ReadLines(string path)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.GetFileAsync(name);
			return await FileIO.ReadLinesAsync(file);
		}

		public static async Task WriteLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.ReplaceExisting);
			await FileIO.WriteLinesAsync(file, content);
		}

		public static async Task AppendLines(string path, System.Collections.Generic.IEnumerable<string> content)
		{
			string name = Path.GetFileName(path);
			string directoryPath = Path.GetDirectoryName(path);
			StorageFolder directory = await WSAAsync.Directory.NavigateTo(directoryPath);

			StorageFile file = await directory.CreateFileAsync(name, CreationCollisionOption.OpenIfExists);
			await FileIO.AppendLinesAsync(file, content);
		}

		private static int readStreamToBuffer(System.IO.Stream stream, byte[] buffer)
		{
			int readOffset = 0;
			int nBytesLeft = buffer.Length;

			do {
				// "Read" can read any number of bytes, even if more exist, so do a loop until everything is read.
				int n = stream.Read(buffer, readOffset, nBytesLeft);
				readOffset += n;
				nBytesLeft -= n;
			} while (nBytesLeft > 0 && stream.Position < stream.Length);

			return readOffset;
		}

		private static async Task move(string name, string sourcePath, string newName, string destinationPath)
		{
			StorageFolder source = await WSAAsync.Directory.NavigateTo(sourcePath);
			StorageFolder destination = await WSAAsync.Directory.NavigateTo(destinationPath);
			StorageFile file = await source.GetFileAsync(name);
			await file.MoveAsync(destination, newName, NameCollisionOption.ReplaceExisting);
		}

		private static async Task copy(string name, string sourcePath, string newName, string destinationPath)
		{
			StorageFolder source = await WSAAsync.Directory.NavigateTo(sourcePath);
			StorageFolder directoryPath = await WSAAsync.Directory.NavigateTo(destinationPath);
			StorageFile file = await source.GetFileAsync(name);
			await file.CopyAsync(directoryPath, newName, NameCollisionOption.ReplaceExisting);
		}
	}
}
#endif