// DecompilerFi decompiler from Assembly-CSharp.dll class: com.ootii.Graphics.Text
using com.ootii.Collections;
using UnityEngine;

namespace com.ootii.Graphics
{
	public class Text
	{
		public Transform Transform;

		public string Value = string.Empty;

		public Vector3 Position = Vector3.zero;

		public Color Color = Color.white;

		public Texture2D Texture;

		public float ExpirationTime;

		private static ObjectPool<Text> sPool = new ObjectPool<Text>(20, 5);

		public static int Length => sPool.Length;

		public static Text Allocate()
		{
			return sPool.Allocate();
		}

		public static void Release(Text rInstance)
		{
			if (!object.ReferenceEquals(rInstance, null))
			{
				if (rInstance.Texture != null)
				{
					UnityEngine.Object.Destroy(rInstance.Texture);
					rInstance.Texture = null;
				}
				rInstance.Transform = null;
				rInstance.Value = string.Empty;
				rInstance.Position = Vector3.zero;
				rInstance.Color = Color.white;
				rInstance.ExpirationTime = 0f;
				sPool.Release(rInstance);
			}
		}
	}
}
