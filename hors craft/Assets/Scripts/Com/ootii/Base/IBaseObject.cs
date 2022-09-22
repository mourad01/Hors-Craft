// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Base.IBaseObject
namespace com.ootii.Base
{
	public interface IBaseObject
	{
		string GUID
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}

		string GenerateGUID();
	}
}
