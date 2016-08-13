using System;
using System.IO;

namespace CheckAll
{
	internal class MessageWriter
	{
		public void WriteLine(ConsoleColor foreground, string format, params object[] args)
		{
			using (new ForegroundColor(foreground))
			{
				var line = string.Format(format, args);
				if (line.Length == Console.BufferWidth)
					Console.Out.Write(line);
				else
					WriteLine(line);
			}
		}

		public void WriteLine(string format, params object[] args)
		{
			Console.Out.WriteLine(format, args);
		}

		public void WriteWarning(string format, params object[] args)
		{
			using (new ForegroundColor(ConsoleColor.DarkYellow))
				WriteLine(format, args);
		}

		public void WriteError(string format, params object[] args)
		{
			using (new ForegroundColor(ConsoleColor.Red))
				WriteLine(format, args);
		}

		internal void WriteLines(FileInfo fileInfo, ConsoleColor foregroundColor, string lineFormat = null)
		{
			using (var fileStream = fileInfo.OpenRead())
			using (var reader = new StreamReader(fileStream))
			using (new ForegroundColor(foregroundColor))
			{
				string line = null;
				while ((line = reader.ReadLine()) != null)
				{
					var lineToWrite = lineFormat == null
						? line
						: string.Format(lineFormat, line);

					Console.Out.WriteLine(lineToWrite);
				}
			}
		}

		internal int GetLineWidth()
		{
			return Console.BufferWidth;
		}
	}
}
