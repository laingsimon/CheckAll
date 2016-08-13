namespace CheckAll
{
	internal static class StringExtensions
	{
		public static string TrimToMaximumLength(this string originalString, int maximumLength, string elipsis)
		{
			if (originalString.Length <= maximumLength)
				return originalString;

			var startOffset = originalString.Length - (maximumLength - elipsis.Length);
			return elipsis + originalString.Substring(startOffset);
		}
	}
}
