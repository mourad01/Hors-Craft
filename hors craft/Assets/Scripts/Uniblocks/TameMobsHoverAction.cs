// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.TameMobsHoverAction
using com.ootii.Cameras;
using Common.Managers;
using GameUI;
using UnityEngine;

namespace Uniblocks
{
	public class TameMobsHoverAction : HoverAction
	{
		private const float TOO_CLOSE_DISTANCE = 1f;

		private const float TAME_INTERVAL = 0.5f;

		private const float MOVE_AWAY_DISTANCE = 1f;

		private TamePanelController tamePanel;

		private TamePanelContext tameContext;

		private Pettable pettable;

		private float nextTamePossibility;

		public TameMobsHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			tameContext = new TamePanelContext
			{
				setPanel = SetTamePanel,
				onTame = OnClickTame,
				onMoveModeChange = OnClickMoveMode
			};
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.TAME_PANEL_CONFIG, tameContext);
		}

		private void SetTamePanel(TamePanelController tamePanel)
		{
			this.tamePanel = tamePanel;
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			if (tamePanel == null)
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			if (hitInfo.hit.collider != null)
			{
				pettable = hitInfo.hit.collider.gameObject.GetComponentInParent<Pettable>();
				if (pettable != null)
				{
					if (!CanTamePet(pettable))
					{
						return;
					}
					if (pettable.GetComponent<Patient>() != null && pettable.GetComponent<Patient>().gotDisese)
					{
						flag = true;
					}
					else
					{
						if (pettable.isSpecialPet)
						{
							flag2 = true;
						}
						tamePanel.ShowAndPosition(pettable.transform, pettable);
						tamePanel.Refresh(pettable);
						if (pettable.tamed)
						{
							MoveIfTooClose(pettable);
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag2)
			{
				tamePanel.EnableOnlyTameButton(enable: false);
			}
			else if (flag)
			{
				tamePanel.Enable(enable: false);
			}
		}

		private bool CanTamePet(Pettable pettableObject)
		{
			if (!Manager.Contains<PetManager>())
			{
				return false;
			}
			PetManager petManager = Manager.Get<PetManager>();
			if (petManager == null)
			{
				return true;
			}
			if (!petManager.UseWhiteListing)
			{
				return true;
			}
			if (!(pettableObject is IPettable))
			{
				return false;
			}
			if (petManager.CanTamePet(pettableObject as IPettable))
			{
				return true;
			}
			return false;
		}

		private void MoveIfTooClose(Pettable pettable)
		{
			if (pettable.following && hitInfo.hit.distance < 1f)
			{
				pettable.mob.MoveAway(hitInfo.hit.point - CameraController.instance.MainCamera.transform.position, 1f);
			}
		}

		private void OnClickTame()
		{
			if (pettable != null && Time.time > nextTamePossibility)
			{
				pettable.Tame();
				nextTamePossibility = Time.time + 0.5f;
			}
		}

		private void OnClickMoveMode()
		{
			if (pettable != null)
			{
				pettable.MoveModeChange();
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.IN_FRONT_OF_TAMEABLE);
			}
		}
	}
}
