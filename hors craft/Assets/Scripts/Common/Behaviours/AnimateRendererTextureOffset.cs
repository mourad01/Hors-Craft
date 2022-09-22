// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.AnimateRendererTextureOffset
using UnityEngine;
using UnityEngine.UI;

namespace Common.Behaviours
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	public class AnimateRendererTextureOffset : MonoBehaviour
	{
		public bool realtime;

		public Vector2 textureOffsetPerSecond = Vector2.zero;

		public string propertyName = "_MainTex";

		private Renderer foundRenderer;

		private float lastCheck;

		private Renderer rend
		{
			get
			{
				if (foundRenderer == null)
				{
					foundRenderer = GetComponent<Renderer>();
				}
				return foundRenderer;
			}
		}

		private void Update()
		{
			if (!realtime)
			{
				Check(Time.deltaTime);
				return;
			}
			Check(Time.realtimeSinceStartup - lastCheck);
			lastCheck = Time.realtimeSinceStartup;
		}

		private void Check(float delta)
		{
			rend.material.SetTextureOffset(propertyName, rend.material.GetTextureOffset(propertyName) + delta * textureOffsetPerSecond);
		}
	}
}
