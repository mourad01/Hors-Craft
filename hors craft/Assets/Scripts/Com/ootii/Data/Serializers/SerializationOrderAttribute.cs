// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Data.Serializers.SerializationOrderAttribute
using System;

namespace com.ootii.Data.Serializers
{
	public class SerializationOrderAttribute : Attribute
	{
		protected int mValue;

		public int Value => mValue;

		public SerializationOrderAttribute(int rValue)
		{
			mValue = rValue;
		}
	}
}
