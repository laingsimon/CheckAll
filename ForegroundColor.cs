using System;

namespace CheckAll
{
	internal class ForegroundColor : IDisposable
	{
		private readonly ConsoleColor _oldColour;

		public ForegroundColor(ConsoleColor newColour)
		{
			_oldColour = Console.ForegroundColor;
			Console.ForegroundColor = newColour;
		}

		public void Dispose()
		{
			Console.ForegroundColor = _oldColour;
		}
	}
}
