// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Data.Serializers.SerializationDefaultAttribute
using System;

namespace com.ootii.Data.Serializers
{
	public class SerializationDefaultAttribute : Attribute
	{
		protected object mValue;

		public object Value => mValue;

		public SerializationDefaultAttribute(object rValue)
		{
			mValue = rValue;
		}
	}
}
