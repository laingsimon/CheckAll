using System;
using System.Diagnostics;

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
		}

		public ProcessOutcome ProcessFile(GitStatusLine file, Request request, int? fileCount)
		{
			if (fileCount != null)
				_WriteFileName(file, fileCount);

			if (!file.ProcessingReversible && file.Processed)
			{
				_GetOption(ConsoleColor.DarkYellow, "Cannot manage file, processing irreversible");
				return ProcessOutcome.Ignored;
			}

			if (file.UnstagedDeletion)
				return _TryAgainIfInvalidInput(() => _ProcessDeletion(file, request), () => _WriteFileName(file, fileCount));
			if (file.UnstagedModification || file.ModifiedFile)
				return _TryAgainIfInvalidInput(() => _ProcessModification(file, request), () => _WriteFileName(file, fileCount));
			if (file.UnstagedFile)
				return _TryAgainIfInvalidInput(() => _ProcessNewFile(file, request), () => _WriteFileName(file, fileCount));

			throw new InvalidOperationException("File status is not understood - " + file.FileName);
		}

		private void _WriteFileName(GitStatusLine file, int? fileCount)
		{
			var lineWidth = _messageWriter.GetLineWidth();
			var stringPrefix = $"{file.Index + 1}/{fileCount ?? 0}: ";

			var fileName = file.FileName.TrimToMaximumLength(lineWidth - stringPrefix.Length, "...");
			_messageWriter.WriteLine(ConsoleColor.Yellow, $"{stringPrefix}{fileName}");
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
			var option = _GetOption(
				ConsoleColor.DarkGreen,
				file.Processed
				  ? "New (staged): <enter>: leave staged, U: Unstage"
				  : "New: <enter>: Add, D: Delete, V: View, M: Modify");

			switch (option)
			{
				case ConsoleKey.Enter:
					if (file.Processed)
						return ProcessOutcome.Processed;

					_git.Add(file.FileName);
					file.Processed = true;
					file.ProcessingReversible = true;
					return ProcessOutcome.Processed;
				case ConsoleKey.D:
				case ConsoleKey.Delete:
					_git.GetFile(file).Delete();
					file.Processed = true;
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
				case ConsoleKey.U:
					file.Processed = false;
					_git.Reset(file.FileName, true);
					return ProcessOutcome.Processed;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}

		private ProcessOutcome _ModifyFileBeforeCommit(GitStatusLine file, Request request)
		{
			var localFile = _git.GetFile(file);

			using (var backup = new FileBackup(localFile))
			{
				if (file.UnstagedFile)
				{
					var process = Process.Start(_git.GetFile(file).FullName);
					process.WaitForExit();
				}
				else
					_git.DiffTool(file.FileName);

				if (backup.IsSameAsCurrent())
				{
					_messageWriter.WriteWarning("File has not changed");
					return ProcessFile(file, request, null);
				}

				Console.TreatControlCAsInput = true;

				var outcome = ProcessFile(file, request, null);
				if (outcome == ProcessOutcome.Processed)
					file.ProcessingReversible = false;

				Console.TreatControlCAsInput = false;

				return outcome;
			}
		}

		private ConsoleKey _GetOption(ConsoleColor foreground, string message)
		{
			_messageWriter.WriteLine(foreground, message);
			var keyInfo = Console.ReadKey();

			Console.SetCursorPosition(0, Console.CursorTop);

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

			var option = _GetOption(
				ConsoleColor.White,
				file.Processed
				  ? "Modified (staged): <enter> (leave staged), U: Unstage"
				  : "Modified: <enter>: Add, R: Revert, M: Modify");

			switch (option)
			{
				case ConsoleKey.Enter:
				case ConsoleKey.Y:
					if (file.Processed)
						return ProcessOutcome.Processed;

					_git.Add(file.FileName);
					file.Processed = true;
					file.ProcessingReversible = true;
					return ProcessOutcome.Processed;
				case ConsoleKey.R:
					_git.Checkout(file.FileName);
					file.Processed = true;
					return ProcessOutcome.Processed;
				case ConsoleKey.DownArrow:
					return ProcessOutcome.Ignored;
				case ConsoleKey.M:
					return _ModifyFileBeforeCommit(file, request);
				case ConsoleKey.U:
					_git.Reset(file.FileName, true);
					file.Processed = false;
					return ProcessOutcome.Processed;
				case ConsoleKey.UpArrow:
					return ProcessOutcome.StepBack;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}

		private ProcessOutcome _ProcessDeletion(GitStatusLine file, Request request)
		{
			var option = _GetOption(
				ConsoleColor.DarkRed,
				file.Processed
				  ? "Deleted (staged): <enter> <leave staged), U: Unstage"
				  : "Deleted: <enter>: Add, R: Restore, V: View");

			switch (option)
			{
				case ConsoleKey.Enter:
				case ConsoleKey.Y:
					_git.Add(file.FileName);
					file.Processed = true;
					file.ProcessingReversible = true;
					return ProcessOutcome.Processed;
				case ConsoleKey.R:
					_git.Checkout(file.FileName);
					file.Processed = true;
					return ProcessOutcome.Processed;
				case ConsoleKey.DownArrow:
					return ProcessOutcome.Ignored;
				case ConsoleKey.V:
					if (request.DiffFacility == Request.DiffTool.CommandLine)
						_git.Diff(file.FileName);
					else
						_git.DiffTool(file.FileName);
					return _ProcessDeletion(file, request);
				case ConsoleKey.U:
					_git.Reset(file.FileName, true);
					file.Processed = false;
					return ProcessOutcome.Processed;
				case ConsoleKey.UpArrow:
					return ProcessOutcome.StepBack;
				default:
					return ProcessOutcome.InvalidInput;
			}
		}
	}
}
