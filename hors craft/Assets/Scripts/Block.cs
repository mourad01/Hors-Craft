// DecompilerFi decompiler from Assembly-CSharp.dll class: Block
using System;
using System.Xml.Serialization;

[Serializable]
public class Block
{
	[XmlAttribute("id")]
	public int id;

	[XmlAttribute("count")]
	public int count;

	public Block()
	{
	}

	public Block(int id, int count)
	{
		this.id = id;
		this.count = count;
	}
}
