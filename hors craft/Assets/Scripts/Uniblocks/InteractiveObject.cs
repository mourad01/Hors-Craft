// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.InteractiveObject
using UnityEngine;

namespace Uniblocks
{
	public class InteractiveObject : MonoBehaviour
	{
		public bool isUsable = true;

		public bool isRotatable = true;

		public bool isDestroyable = true;

		public HoverActionsConnector hoverContext;

		[Header("Careful! Usable Inside is expensive!")]
		public bool usableInside;

		[HideInInspector]
		public VoxelInfo voxelInfo;

		public static bool isLocked
		{
			get;
			protected set;
		}

		protected virtual void Awake()
		{
			isLocked = false;
			if (usableInside)
			{
				UsableInsideHoverAction.AddObject(this);
			}
		}

		public virtual void OnUse()
		{
			UsableIndicator component = GetComponent<UsableIndicator>();
			if (component != null)
			{
				component.Interact();
			}
		}

		public virtual void Init()
		{
		}

		public virtual void SetRotation(byte rotation)
		{
			base.transform.rotation = Quaternion.Euler(0f, 90 * rotation, 0f);
		}

		public virtual void Rotate()
		{
			VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(voxelInfo.GetVoxel());
			if (instanceForVoxelId != null)
			{
				instanceForVoxelId.OnBlockRotate(voxelInfo);
			}
		}

		public virtual void Destroy()
		{
			Voxel.DestroyBlock(voxelInfo);
			AbstractInteractiveDestroyBehaviour[] components = GetComponents<AbstractInteractiveDestroyBehaviour>();
			AbstractInteractiveDestroyBehaviour[] array = components;
			foreach (AbstractInteractiveDestroyBehaviour abstractInteractiveDestroyBehaviour in array)
			{
				abstractInteractiveDestroyBehaviour.Destroy(base.gameObject);
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
