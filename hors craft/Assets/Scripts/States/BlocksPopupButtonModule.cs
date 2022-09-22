// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlocksPopupButtonModule
using Common.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class BlocksPopupButtonModule : GameplayModule
	{
		public Button blocksButton;

		public Image blocksButtonImage;

		private ushort lastVoxelSet;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.BLOCK_HELD,
			Fact.MCPE_STEERING
		};

		public override void Init()
		{
			base.Init();
			if (blocksButton != null)
			{
				blocksButton.onClick.AddListener(OnBlocksButtonClciked);
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (!MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				if (base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: false);
				}
				return;
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			if (changedFacts.Contains(Fact.BLOCK_HELD) || changedFacts.Contains(Fact.MCPE_STEERING))
			{
				OnBlockHeldChange();
			}
		}

		private void OnBlocksButtonClciked()
		{
			BlocksPopupState.Show();
		}

		private void OnBlockHeldChange()
		{
			HeldBlockContext heldBlockContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HeldBlockContext>(Fact.BLOCK_HELD).FirstOrDefault();
			if (heldBlockContext != null)
			{
				Voxel voxelType = Engine.GetVoxelType(heldBlockContext.block);
				Sprite voxelSprite = VoxelSprite.GetVoxelSprite(voxelType);
				blocksButtonImage.sprite = voxelSprite;
				if (voxelType.GetUniqueID() != lastVoxelSet)
				{
					Manager.Get<StateMachineManager>().StartCoroutine(AnimateButtonFocus());
					lastVoxelSet = voxelType.GetUniqueID();
				}
			}
		}

		private IEnumerator AnimateButtonFocus()
		{
			yield return new WaitForSecondsRealtime(0.1f);
			if (blocksButton != null)
			{
				blocksButton.GetComponent<Animator>().SetTrigger("ButtonFocus");
			}
		}
	}
}
