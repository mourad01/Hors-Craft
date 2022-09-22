// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.MechPlayerGraphic
using Common.Managers;
using Common.Utils;
using System;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class MechPlayerGraphic : PlayerGraphic, IFormChangeListener
	{
		public GameObject levelUpPrefab;

		private FormChanger formChanger;

		protected void Start()
		{
			formChanger = GetComponentInChildren<FormChanger>();
			ProgressManager progressManager = Manager.Get<ProgressManager>();
			progressManager.onLevelUpCallbacks = (Action)Delegate.Combine(progressManager.onLevelUpCallbacks, new Action(LevelUp));
		}

		private void LevelUp()
		{
			Transform transform = GetComponentInChildren<FormChanger>().transform;
			GameObject gameObject = UnityEngine.Object.Instantiate(levelUpPrefab, transform, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
		}

		public void OnFormChange()
		{
			Bounds bounds = RenderersBounds.MaximumBounds(base.gameObject);
			Vector3 center = bounds.center;
			Vector3 size = bounds.size;
			float y = size.y;
			Vector3 extents = bounds.extents;
			float z = extents.z;
			GetComponentInParent<PlayerMovement>().SetControllerSize(center, y, z);
		}

		public override int GetCurrentCloth(BodyPart part)
		{
			return 0;
		}

		public override void Grab(GameObject go)
		{
		}

		public override void UnGrab()
		{
		}

		public override void ShowBodyAndLegs()
		{
		}

		public override void HideBodyAndLegs()
		{
		}

		public override void HideHands()
		{
		}

		public override void SetAllClothes()
		{
		}

		public override void SetBodyPartMaterial(BodyPart part, int index)
		{
		}

		public override void SetHat(GameObject hat)
		{
		}

		public override void SetRandomSkin(SkinList skinList)
		{
		}

		public override void SetRandomSkinWithGender(SkinList skinList, Skin.Gender gender)
		{
		}

		public override void SetSkin(SkinList skinList, int index)
		{
		}

		public override void SetWholeBodyl(int index)
		{
		}

		public override void ShowHands()
		{
		}

		public override void ToggleHead(bool newState)
		{
		}
	}
}
