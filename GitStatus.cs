using System.Collections.Generic;
using System.Linq;

namespace CheckAll
{
	internal class GitStatus
	{
		private readonly IReadOnlyCollection<string> _lines;

		public GitStatus(IReadOnlyCollection<string> lines)
		{
			_lines = lines;
		}

		public IEnumerable<GitStatusLine> GetFiles()
		{
			var index = 0;
			foreach (var line in _lines)
			{
				var file = new GitStatusLine(line, index);

				if (file.UnstagedDeletion || file.UnstagedFile || file.UnstagedModification || file.ModifiedFile)
				{
					yield return file;
					index++;
				}
			}
		}

		public int UnstagedFiles => GetFiles().Count(f => f.UnstagedFile);
		public int UnstagedDeletions => GetFiles().Count(f => f.UnstagedDeletion);
		public int UnstagedModifications => GetFiles().Count(f => f.UnstagedModification || f.ModifiedFile);
	}
}
