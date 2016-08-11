using System;

namespace CheckAll
{
	internal class MessageWriter
	{
		public void WriteLine(ConsoleColor foreground, string format, params object[] args)
		{
			using (new ForegroundColor(foreground))
				WriteLine(format, args);
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
	}
}
