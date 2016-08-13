using System.Text.RegularExpressions;

namespace CheckAll
{
	internal class FileMimeDetail
	{
		private string _mimeDetail;

		public FileMimeDetail(string mimeDetail)
		{
			_mimeDetail = mimeDetail;
		}

		private Match _GetDetail()
		{
			return Regex.Match(_mimeDetail, @"^(?<fileName>.+?):\s+(?<mimeType>.+?);\s+(?<charset>.+?)$");
		}

		public string Charset
		{
			get
			{
				return _GetDetail().Groups["charset"].Value;
			}
		}

		public string FileName
		{
			get
			{
				return _GetDetail().Groups["filename"].Value;
			}
		}

		public string MimeType
		{
			get
			{
				return _GetDetail().Groups["mimeType"].Value;
			}
		}
	}
}
