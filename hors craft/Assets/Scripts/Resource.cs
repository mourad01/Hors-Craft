// DecompilerFi decompiler from Assembly-CSharp.dll class: Resource
using Common.Managers;
using System;
using System.Xml.Serialization;

[Serializable]
public class Resource
{
	[XmlAttribute("id")]
	public int id;

	[XmlAttribute("count")]
	public int count;

	[XmlIgnore]
	public int blockId => Manager.Get<CraftingManager>().GetBlockId(id);

	[XmlIgnore]
	public int weaponId => Manager.Get<CraftingManager>().GetBlockId(id);

	public Resource()
	{
	}

	public Resource(int id, int count)
	{
		this.id = id;
		this.count = count;
	}
}
