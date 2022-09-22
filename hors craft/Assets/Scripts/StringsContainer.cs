// DecompilerFi decompiler from Assembly-CSharp.dll class: StringsContainer
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StringsContainer : MonoBehaviour
{
	[Serializable]
	public class StringData
	{
		public string commonName;

		public string value;
	}

	public List<StringData> strings = new List<StringData>();

	public string GetValue(string name)
	{
		return (from s in strings
			where s.commonName == name
			select s).FirstOrDefault()?.value;
	}
}
