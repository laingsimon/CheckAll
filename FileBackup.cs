using System;
using System.IO;
using System.Linq;

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

		internal bool IsSameAsCurrent()
		{
			var currentContent = File.ReadAllBytes(_file.FullName);
			return currentContent.SequenceEqual(_originalContent);
		}
	}
}
