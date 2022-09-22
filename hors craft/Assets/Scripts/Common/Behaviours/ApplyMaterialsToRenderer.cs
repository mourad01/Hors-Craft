// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.ApplyMaterialsToRenderer
using UnityEngine;

namespace Common.Behaviours
{
	[RequireComponent(typeof(Renderer))]
	[ExecuteInEditMode]
	public class ApplyMaterialsToRenderer : MonoBehaviour
	{
		public Material[] materials;

		private Renderer render;

		private void OnEnable()
		{
			render = GetComponent<Renderer>();
		}

		private void OnGUI()
		{
			render.materials = materials;
		}
	}
}
