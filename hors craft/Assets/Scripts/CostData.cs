// DecompilerFi decompiler from Assembly-CSharp.dll class: CostData
using System;

[Serializable]
public class CostData
{
	public string id;

	public string name;

	public int cost;

	public ushort TryToGetNumericId()
	{
		return ushort.Parse(id);
	}
}
