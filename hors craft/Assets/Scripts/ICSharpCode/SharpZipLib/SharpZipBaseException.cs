// DecompilerFi decompiler from Assembly-CSharp.dll class: ICSharpCode.SharpZipLib.SharpZipBaseException
using System;

namespace ICSharpCode.SharpZipLib
{
	[Serializable]
	public class SharpZipBaseException : Exception
	{
		public SharpZipBaseException()
		{
		}

		public SharpZipBaseException(string message)
			: base(message)
		{
		}

		public SharpZipBaseException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
