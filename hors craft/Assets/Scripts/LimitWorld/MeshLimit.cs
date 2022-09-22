// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.MeshLimit
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LimitWorld
{
	[CreateAssetMenu(menuName = "ScriptableObjects/LimitWorld/Limit/MeshLimit")]
	public class MeshLimit : Limit
	{
		[SerializeField]
		public GameObject prefab;

		private GameObject _instance;

		public override EventTypeLW eventType
		{
			[CompilerGenerated]
			get
			{
				return EventTypeLW.OnlyInit;
			}
		}

		protected override Action initialAction
		{
			[CompilerGenerated]
			get
			{
				return SetupMesh;
			}
		}

		private GameObject instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = UnityEngine.Object.Instantiate(prefab, limitShape.center, Quaternion.identity);
				}
				return _instance;
			}
		}

		private void SetupMesh()
		{
			instance.transform.localScale = limitShape.GetRadius(doChunkCorrection) * 2f;
			Vector3 center = limitShape.center;
			center.y = 0f;
			instance.transform.position = center;
		}

		public override bool ProcessEvent(DataLW data)
		{
			return true;
		}

		public override void ResetLimit()
		{
			base.ResetLimit();
			UnityEngine.Object.Destroy(instance);
		}

		public override void ReSetup()
		{
			base.ReSetup();
			SetupMesh();
		}
	}
}
