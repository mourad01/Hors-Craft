// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.BlueprintDataFiles
using System.IO;
using UnityEngine;

namespace Uniblocks
{
	public static class BlueprintDataFiles
	{
		public static BlueprintData ReadDataFromResources(string blueprintResourceName)
		{
			string path = "Blueprints/" + blueprintResourceName;
			TextAsset textAsset = Resources.Load<TextAsset>(path);
			if (textAsset != null)
			{
				MemoryStream stream = new MemoryStream(textAsset.bytes, writable: false);
				return BlueprintData.Deserialize(stream);
			}
			return null;
		}
	}
}
