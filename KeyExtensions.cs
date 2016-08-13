using System;

namespace CheckAll
{
	internal static class KeyExtensions
	{
		public static ProcessOutcome GetOutcome(this ConsoleKey key)
		{
			switch (key)
			{
				case ConsoleKey.DownArrow:
					return ProcessOutcome.Ignored;
				case ConsoleKey.UpArrow:
					return ProcessOutcome.StepBack;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}
	}
}
