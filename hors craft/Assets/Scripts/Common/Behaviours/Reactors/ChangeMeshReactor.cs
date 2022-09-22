// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.Reactors.ChangeMeshReactor
using UnityEngine;

namespace Common.Behaviours.Reactors
{
	public class ChangeMeshReactor : AbstractRestorableReactor
	{
		[Header("New mesh to be replaced when reacted.")]
		public Mesh newMesh;

		[Header("Destination. Leave empty if meshFilter is self.")]
		public MeshFilter replaceMeshIn;

		private MeshFilter meshFilter;

		private Mesh baseMesh;

		private void Awake()
		{
			meshFilter = ((!(replaceMeshIn != null)) ? GetComponent<MeshFilter>() : replaceMeshIn);
			baseMesh = meshFilter.mesh;
		}

		public override void React()
		{
			meshFilter.mesh = newMesh;
		}

		public override void Restore()
		{
			meshFilter.mesh = baseMesh;
		}
	}
}
