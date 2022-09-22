// DecompilerFi decompiler from Assembly-CSharp.dll class: Spine.Unity.SpineAtlasRegion
using UnityEngine;

namespace Spine.Unity
{
	public class SpineAtlasRegion : PropertyAttribute
	{
		public string atlasAssetField;

		public SpineAtlasRegion(string atlasAssetField = "")
		{
			this.atlasAssetField = atlasAssetField;
		}
	}
}
