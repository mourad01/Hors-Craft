// DecompilerFi decompiler from Assembly-CSharp.dll class: RealBlock
using System;
using System.Xml.Serialization;

[Serializable]
public class RealBlock
{
	[XmlAttribute("id")]
	public int id;

	[XmlAttribute("count")]
	public int count;

	public RealBlock()
	{
	}

	public RealBlock(int id, int count)
	{
		this.id = id;
		this.count = count;
	}
}
