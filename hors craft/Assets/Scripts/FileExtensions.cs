// DecompilerFi decompiler from Assembly-CSharp.dll class: FileExtensions
using System.IO;

public static class FileExtensions
{
	public static void CreateDirectoryIfNotExists(this string folder)
	{
		if (!folder.IsNullOrEmpty())
		{
			string directoryName = Path.GetDirectoryName(folder);
			if (!directoryName.IsNullOrEmpty() && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
		}
	}
}
