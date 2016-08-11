using System;
using System.Collections.Generic;
using System.Linq;

namespace CheckAll
{
	internal class Request
	{
		private readonly IReadOnlyCollection<string> _unusedArguments;

		public DiffTool DiffFacility { get; private set; }

		public Request(string[] args)
		{
			var unusedArguments = new List<string>();
			foreach (var argument in args)
			{
				if (!_ProcessArgument(argument))
					unusedArguments.Add(argument);
			}

			_unusedArguments = unusedArguments;
		}

		private bool _ProcessArgument(string argument)
		{
			if (argument.Equals("--difftool", StringComparison.OrdinalIgnoreCase))
			{
				DiffFacility = DiffTool.Interactive;
				return true;
			}

			if (argument.Equals("--diff", StringComparison.OrdinalIgnoreCase))
			{
				DiffFacility = DiffTool.CommandLine;
				return true;
			}

			return false;
		}

		public enum DiffTool
		{
			CommandLine,
			Interactive
		}

		internal string GetStatusArguments()
		{
			return string.Join(
				" ",
				_unusedArguments.Select(arg => $"\"{arg}\""));
		}
	}
}
