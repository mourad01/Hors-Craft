// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.Reactors.ChangeMaterialReactor
using UnityEngine;

namespace Common.Behaviours.Reactors
{
	public class ChangeMaterialReactor : AbstractRestorableReactor
	{
		[Header("New material to be replaced when reacted.")]
		public Material newMaterial;

		[Header("Material index (leave it be if there's only one material).")]
		public uint materialIndex;

		[Header("Destination. Leave empty if renderer is self.")]
		public Renderer replaceMaterialIn;

		private Renderer rend;

		private Material baseMaterial;

		private void Awake()
		{
			rend = ((!(replaceMaterialIn != null)) ? GetComponent<Renderer>() : replaceMaterialIn);
			baseMaterial = rend.materials[materialIndex];
		}

		public override void React()
		{
			ChangeMaterialTo(newMaterial);
		}

		public override void Restore()
		{
			ChangeMaterialTo(baseMaterial);
		}

		private void ChangeMaterialTo(Material mat)
		{
			Material[] materials = rend.materials;
			materials[materialIndex] = mat;
			rend.materials = materials;
		}
	}
}
