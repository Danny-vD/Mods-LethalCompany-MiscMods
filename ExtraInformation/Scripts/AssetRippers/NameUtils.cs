namespace ExtraInformation.AssetRippers
{
	public static class NameUtils
	{
		/// <summary>
		/// <para>Returns the same string without the (clone) part</para>
		/// <para>It does this by removing anything after the first '('</para>
		/// </summary>
		public static string RemoveCloneFromString(string name)
		{
			int removeStartIndex = name.IndexOf('(');

			return removeStartIndex == -1 ? name : name[..removeStartIndex];
		}

		public static string ReplaceSlashesInName(string value)
		{
			return value.Replace('\\', '_').Replace('/', '_');
		}
	}
}