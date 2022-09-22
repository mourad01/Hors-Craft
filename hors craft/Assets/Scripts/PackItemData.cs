// DecompilerFi decompiler from Assembly-CSharp.dll class: PackItemData
using System;
using System.Xml.Serialization;

[XmlInclude(typeof(PackItemData))]
public abstract class PackItemData
{
	public PackItemType type;

	public abstract Type GetConnectedType();

	public abstract void GrantItem();

	public abstract void FillWithRandom();

	public abstract void TryParseData(string data);

	public abstract string ToParsable();

	public abstract bool IsValid();
}
