using System;
using System.IO;

namespace CheckAll
{
	internal class FileProcessor
	{
		private readonly MessageWriter _messageWriter;
		private readonly Git _git;

		public FileProcessor(
			MessageWriter messageWriter,
			Git git)
		{
			_messageWriter = messageWriter;
			_git = git;

			Console.CancelKeyPress += Console_CancelKeyPress;
		}

		private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{

		}

		public ProcessOutcome ProcessFile(GitStatusLine file, Request request, int? fileCount)
		{
			if (fileCount != null)
				_WriteFileName(file, fileCount);

			if (file.UnstagedDeletion)
				return _TryAgainIfInvalidInput(() => _ProcessDeletion(file, request), () => _WriteFileName(file, fileCount));
			if (file.UnstagedModification)
				return _TryAgainIfInvalidInput(() => _ProcessModification(file, request), () => _WriteFileName(file, fileCount));
			if (file.UnstagedFile)
				return _TryAgainIfInvalidInput(() => _ProcessNewFile(file, request), () => _WriteFileName(file, fileCount));

			throw new InvalidOperationException("File status is not understood - " + file.FileName);
		}

		private void _WriteFileName(GitStatusLine file, int? fileCount)
		{
			_messageWriter.WriteLine(ConsoleColor.Yellow, $"{file.Index+1}/{fileCount ?? 0}: {file.FileName}");
		}

		private ProcessOutcome _TryAgainIfInvalidInput(Func<ProcessOutcome> fileAction, Action writeFileName)
		{
			ProcessOutcome outcome;
			while ((outcome = fileAction()) == ProcessOutcome.InvalidInput)
			{
				_messageWriter.WriteError("Unrecognised input");
				writeFileName();
			}

			return outcome;
		}

		private ProcessOutcome _ProcessNewFile(GitStatusLine file, Request request)
		{
			var option = _GetOption(ConsoleColor.DarkGreen, "New: <enter>: Add, D: Delete, V: View, M: Modify");

			switch (option)
			{
				case ConsoleKey.Enter:
					_git.Add(file.FileName);
					return ProcessOutcome.Processed;
				case ConsoleKey.D:
				case ConsoleKey.Delete:
					_git.GetFile(file).Delete();
					return ProcessOutcome.Processed;
				case ConsoleKey.V:
					_messageWriter.WriteLines(_git.GetFile(file), ConsoleColor.DarkGreen, "+ {0}");
					return _ProcessNewFile(file, request);
				case ConsoleKey.DownArrow:
					return ProcessOutcome.Ignored;
				case ConsoleKey.M:
					return _ModifyFileBeforeCommit(file, request);
				case ConsoleKey.UpArrow:
					return ProcessOutcome.StepBack;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}

		private ProcessOutcome _ModifyFileBeforeCommit(GitStatusLine file, Request request)
		{
			var localFile = _git.GetFile(file);

			using (new FileBackup(localFile))
			{
				_git.DiffTool(file.FileName);

				Console.TreatControlCAsInput = true;

				var outcome = ProcessFile(file, request, null);

				Console.TreatControlCAsInput = false;

				return outcome;
			}
		}

		private ConsoleKey _GetOption(ConsoleColor foreground, string message)
		{
			_messageWriter.WriteLine(foreground, message);
			var keyInfo = Console.ReadKey();

			return keyInfo.Key;
		}

		private ProcessOutcome _ProcessModification(GitStatusLine file, Request request)
		{
			switch (request.DiffFacility)
			{
				case Request.DiffTool.Interactive:
					_git.DiffTool(file.FileName);
					break;
				default:
					_git.Diff(file.FileName);
					break;
			}

			var option = _GetOption(ConsoleColor.White, "Modified: <enter>: Add, R: Revert, M: Modify");

			switch (option)
			{
				case ConsoleKey.Enter:
				case ConsoleKey.Y:
					_git.Add(file.FileName);
					return ProcessOutcome.Processed;
				case ConsoleKey.R:
					_git.Checkout(file.FileName);
					return ProcessOutcome.Processed;
				case ConsoleKey.DownArrow:
					return ProcessOutcome.Ignored;
				case ConsoleKey.M:
					return _ModifyFileBeforeCommit(file, request);
				case ConsoleKey.UpArrow:
					return ProcessOutcome.StepBack;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}

		private ProcessOutcome _ProcessDeletion(GitStatusLine file, Request request)
		{
			var option = _GetOption(ConsoleColor.DarkRed, "Deleted: <enter>: Add, R: Restore, V: View");

			switch (option)
			{
				case ConsoleKey.Enter:
				case ConsoleKey.Y:
					_git.Add(file.FileName);
					return ProcessOutcome.Processed;
				case ConsoleKey.R:
					_git.Checkout(file.FileName);
					return ProcessOutcome.Processed;
				case ConsoleKey.DownArrow:
					return ProcessOutcome.Ignored;
				case ConsoleKey.V:
					if (request.DiffFacility == Request.DiffTool.CommandLine)
						_git.Diff(file.FileName);
					else
						_git.DiffTool(file.FileName);
					return _ProcessDeletion(file, request);
				case ConsoleKey.UpArrow:
					return ProcessOutcome.StepBack;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}
	}
}
