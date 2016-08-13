using System;
using System.Linq;

namespace CheckAll
{
	internal class Application
	{
		private readonly FileProcessor _fileProcessor;
		private readonly Git _git;
		private readonly MessageWriter _writer;

		public Application(
			MessageWriter writer,
			Git git,
			FileProcessor fileProcessor)
		{
			_writer = writer;
			_git = git;
			_fileProcessor = fileProcessor;
		}

		internal void Execute(Request request)
		{
			if (!_git.IsAGitRepository())
			{
				_writer.WriteError("Not a git repository");
				return;
			}

			var status = _git.GetStatus(request);

			var files = status.GetFiles().ToArray();

			var index = 0;
			while (index < files.Length && index >= 0)
			{
				var file = files[index];
				var outcome = _fileProcessor.ProcessFile(file, request, files.Length);

				switch(outcome)
				{
					case ProcessOutcome.StepBack:
						if (index > 0)
							index--;
						else
							_writer.WriteError("At first file");
						break;
					default:
						index++;
						break;
				}
			}

			_writer.WriteLine(ConsoleColor.Blue, new string('_', _writer.GetLineWidth()));
			_writer.WriteLine("");

			_git.Status(request);
		}
	}
}
