using System;
using System.IO;

namespace CheckAll
{
	internal class FileBackup : IDisposable
	{
		private readonly FileInfo _file;
		private readonly byte[] _originalContent;

		public FileBackup(FileInfo file)
		{
			_file = file;
			_originalContent = File.ReadAllBytes(file.FullName);
		}

		public void Dispose()
		{
			File.WriteAllBytes(_file.FullName, _originalContent);
		}
	}
}
