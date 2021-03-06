﻿using System;
using System.IO;
using System.Linq;

namespace CheckAll
{
	internal class MessageWriter
	{
        private const int defaultBufferWidth = 80;

        private static bool? bufferWidthAvailable;

        public void WriteLine(ConsoleColor foreground, string format, params object[] args)
		{
			using (new ForegroundColor(foreground))
			{
				var line = args.Any()
					? string.Format(format, args)
					: format;
				if (line.Length == GetLineWidth())
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
            if (bufferWidthAvailable != false)
            {
                try
                {
                    var width = Console.BufferWidth;
                    bufferWidthAvailable = true;
                    return width;
                }
                catch (Exception)
                {
                    bufferWidthAvailable = false;
                }
            }

            return defaultBufferWidth;
        }
    }
}
