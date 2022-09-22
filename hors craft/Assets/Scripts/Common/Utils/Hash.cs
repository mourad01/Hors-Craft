// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.Hash
using System.Security.Cryptography;
using System.Text;

namespace Common.Utils
{
	public class Hash
	{
		public static string CalculateMD5Hash(string input)
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}
