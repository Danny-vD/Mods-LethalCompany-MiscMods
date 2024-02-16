namespace MonitorLabels.ExtensionMethods
{
	/// <summary>
	/// Contains extension methods for <see cref="System.String"/>
	/// </summary>
	public static class StringExtensions
	{
		/// <summary>
		/// Returns a new string where a space is inserted before each capital, skipping the first char
		/// </summary>
		public static string InsertSpaceBeforeCapitals(this string text)
		{
			string capitals = text.ToUpper();
			string copyText = text;

			if (text.Length is 0 or 1)
			{
				return copyText;
			}

			for (int i = text.Length - 1; i >= 1; i--)
			{
				if (text[i] == capitals[i])
				{
					copyText = copyText.Insert(i, " ");
				}
			}

			return copyText;
		}
	}
}