using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CheckAll
{
	internal class Git
	{
		private DirectoryInfo _repository;
		private static readonly FileInfo _gitExecutable = new FileInfo(@"c:\Program Files\Git\bin\git.exe");

		public Git(DirectoryInfo repository)
		{
			_repository = repository;
		}

		public bool IsAGitRepository()
		{
			return _repository.SubDirectory(".git").Exists;
		}

		internal GitStatus GetStatus(Request request)
		{
			var lines = new List<string>();
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = "status --porcelain " + request.GetStatusArguments(),
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					WorkingDirectory = _repository.FullName
				}
			};
			process.OutputDataReceived += (sender, args) => lines.Add(args.Data);

			process.Start();
			process.BeginOutputReadLine();

			process.WaitForExit();

			return new GitStatus(lines.Where(l => l != null).ToArray());
		}

		internal void Status(Request request)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = "status " + request.GetStatusArguments(),
					WorkingDirectory = _repository.FullName,
					UseShellExecute = false
				}
			};

			process.Start();
			process.WaitForExit();
		}

		internal void Add(string fileName)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = $"add -A \"{fileName}\"",
					WorkingDirectory = _repository.FullName,
					UseShellExecute = false
				}
			};

			process.Start();
			process.WaitForExit();
		}

		internal FileInfo GetFile(string fileName)
		{
			return _repository.File(fileName.Replace("/", "\\"));
		}

		internal void Show(string fileName, string gitRef)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = $"--no-pager show \"{gitRef}:{fileName}\"",
					WorkingDirectory = _repository.FullName,
					UseShellExecute = false
				}
			};

			process.Start();
			process.WaitForExit();
		}

		internal void Checkout(string fileName)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = $"checkout \"{fileName}\"",
					WorkingDirectory = _repository.FullName,
					UseShellExecute = false
				}
			};

			process.Start();
			process.WaitForExit();
		}

		internal void Diff(string fileName)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = $"diff \"{fileName}\"",
					WorkingDirectory = _repository.FullName,
					UseShellExecute = false
				}
			};

			process.Start();
			process.WaitForExit();
		}

		internal void DiffTool(string fileName)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = _gitExecutable.FullName,
					Arguments = $"difftool \"{fileName}\"",
					WorkingDirectory = _repository.FullName
				}
			};

			process.Start();
			process.WaitForExit();
		}
	}
}
