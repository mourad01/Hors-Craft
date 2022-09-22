// DecompilerFi decompiler from Assembly-CSharp.dll class: ICSharpCode.SharpZipLib.Checksums.IChecksum
namespace ICSharpCode.SharpZipLib.Checksums
{
	public interface IChecksum
	{
		long Value
		{
			get;
		}

		void Reset();

		void Update(int value);

		void Update(byte[] buffer);

		void Update(byte[] buffer, int offset, int count);
	}
}
