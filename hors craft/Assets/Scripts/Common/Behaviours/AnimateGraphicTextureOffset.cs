// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.AnimateGraphicTextureOffset
using UnityEngine;
using UnityEngine.UI;

namespace Common.Behaviours
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Graphic))]
	public class AnimateGraphicTextureOffset : MonoBehaviour
	{
		public bool realtime;

		public Vector2 textureOffsetPerSecond = Vector2.zero;

		public string propertyName = "_MainTex";

		private Graphic foundGraphic;

		private float lastCheck;

		private Graphic graphic
		{
			get
			{
				if (foundGraphic == null)
				{
					foundGraphic = GetComponent<Graphic>();
				}
				return foundGraphic;
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
			graphic.material.SetTextureOffset(propertyName, graphic.material.GetTextureOffset(propertyName) + delta * textureOffsetPerSecond);
		}
	}
}
