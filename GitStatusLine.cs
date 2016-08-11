namespace CheckAll
{
	internal class GitStatusLine
	{
		private readonly string _line;

		public GitStatusLine(string line, int index)
		{
			_line = line;
			Index = index;
		}

		public bool IgnoredFile => _IndexMneumonic('!') && _WorkingTreeMneumonic('!');
		public bool UnstagedFile => _IndexMneumonic('?') && _WorkingTreeMneumonic('?');
		public bool UnstagedModification => _WorkingTreeMneumonic('M') && _IndexMneumonic(' ');
		public bool UnstagedDeletion => _WorkingTreeMneumonic('D') && _IndexMneumonic(' ');

		public int Index { get; }
		public string FileName => _line.Substring(2).Trim();

		public bool _IndexMneumonic(char mneumonic)
		{
			return _line[0] == mneumonic;
		}

		public bool _WorkingTreeMneumonic(char mneumonic)
		{
			return _line[1] == mneumonic;
		}

		public override string ToString()
		{
			return _line;
		}
	}
}
