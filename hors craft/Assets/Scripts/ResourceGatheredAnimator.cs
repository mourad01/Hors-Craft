// DecompilerFi decompiler from Assembly-CSharp.dll class: ResourceGatheredAnimator
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceGatheredAnimator : TopNotification
{
	public class ResourceInformation : ShowInformation
	{
		public int resourceId;

		public ResourceInformation(int resourceId, float timetoHide = 0.35f)
		{
			timeToHide = timetoHide;
			this.resourceId = resourceId;
			setOnTop = false;
		}

		public virtual Sprite GetImage()
		{
			return Manager.Get<CraftingManager>().GetResourceImage(resourceId);
		}
	}

	public class CraftableInformation : ResourceInformation
	{
		public ushort blockId;

		public CraftableInformation(ushort blockId, float timetoHide = 0.35f)
			: base(0)
		{
			timeToHide = timetoHide;
			this.blockId = blockId;
		}

		public override Sprite GetImage()
		{
			return VoxelSprite.GetVoxelSprite(blockId);
		}
	}

	public Image image;

	public override void SetElement(ShowInformation information)
	{
		image.color = Color.clear;
		image.enabled = true;
		image.sprite = (information as ResourceInformation).GetImage();
	}

	public override void HideImmediately()
	{
		CancelInvoke();
		showing = false;
		image.enabled = false;
		queue = new Queue<ShowInformation>();
	}
}
