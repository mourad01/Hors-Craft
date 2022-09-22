// DecompilerFi decompiler from Assembly-CSharp.dll class: ICSharpCode.SharpZipLib.BZip2.BZip2Exception
using System;

namespace ICSharpCode.SharpZipLib.BZip2
{
	[Serializable]
	public class BZip2Exception : SharpZipBaseException
	{
		public BZip2Exception()
		{
		}

		public BZip2Exception(string message)
			: base(message)
		{
		}

		public BZip2Exception(string message, Exception exception)
			: base(message, exception)
		{
		}
	}
}
