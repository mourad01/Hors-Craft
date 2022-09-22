// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Data.Serializers.SerializationNameAttribute
using System;

namespace com.ootii.Data.Serializers
{
	public class SerializationNameAttribute : Attribute
	{
		protected string mValue;

		public string Value => mValue;

		public SerializationNameAttribute(string rValue)
		{
			mValue = rValue;
		}
	}
}
