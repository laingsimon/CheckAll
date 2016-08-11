using System.IO;

namespace CheckAll
{
	internal static class IOExtensions
	{
		public static DirectoryInfo SubDirectory(this DirectoryInfo parent, string name)
		{
			return new DirectoryInfo(Path.Combine(parent.FullName, name));
		}

		public static FileInfo File(this DirectoryInfo parent, string fileName)
		{
			return new FileInfo(Path.Combine(parent.FullName, fileName));
		}
	}
}
