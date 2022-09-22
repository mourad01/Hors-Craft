// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.FarmingHoverAction
using com.ootii.Cameras;
using Common.Managers;
using Gameplay;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Uniblocks
{
	public class FarmingHoverAction : HoverAction
	{
		private GrowthManager growth;

		private WateringContext waterContext;

		private HarvestContext harvestContext;

		private SowContext sowContext;

		private VoxelTextContext voxelTextContext;

		private bool isWatering;

		public FarmingHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
			growth = Manager.Get<GrowthManager>();
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			VoxelInfo gridRaycastHit = hitInfo.gridRaycastHit;
			if (isWatering || gridRaycastHit == null)
			{
				RemoveContexts();
				return;
			}
			GrowableInfoVoxel growableInfo = growth.GetGrowableInfo(GrowableInfoVoxel.GetKey(gridRaycastHit)) as GrowableInfoVoxel;
			if (CanShowText(growableInfo))
			{
				ShowText(growableInfo);
			}
			else
			{
				HideText();
			}
			if (CanSow(growableInfo))
			{
				UpdateSowing();
			}
			else if (CanWater())
			{
				UpdateWatering();
			}
			else if (CanHarvest())
			{
				UpdateHarvesting();
			}
			else
			{
				RemoveContexts();
			}
		}

		private bool CanHarvest()
		{
			return GetHarvestInfo() != null;
		}

		private VoxelInfo GetHarvestInfo()
		{
			return hitInfo.gridRaycast.FirstOrDefault((VoxelInfo gr) => Manager.Get<FarmingManager>().CanHarvest(gr));
		}

		private void UpdateHarvesting()
		{
			if (IsDifferentThanBefore(harvestContext, GetHarvestInfo()))
			{
				RemoveContexts();
				harvestContext = new HarvestContext
				{
					useAction = OnHarvest,
					info = GetHarvestInfo()
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_GROWABLE, harvestContext);
			}
		}

		private void UpdateWatering()
		{
			VoxelInfo wateringInfo = GetWateringInfo();
			if (IsDifferentThanBefore(waterContext, wateringInfo))
			{
				RemoveContexts();
				waterContext = new WateringContext
				{
					useAction = OnWater,
					info = wateringInfo
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_GROWABLE, waterContext);
			}
		}

		private bool CanWater()
		{
			return GetWateringInfo() != null;
		}

		private VoxelInfo GetWateringInfo()
		{
			return hitInfo.gridRaycast.FirstOrDefault((VoxelInfo gr) => Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(gr.GetVoxel()) is GrowerEarthVoxelEvents);
		}

		private bool CanSow(GrowableInfoVoxel growableInfo)
		{
			if (hitInfo.gridRaycastHit.GetVoxel() != Engine.usefulIDs.dirtBlockID)
			{
				return false;
			}
			Index index = new Index(hitInfo.gridRaycastHit.index);
			index.y++;
			VoxelInfo voxelInfo = hitInfo.gridRaycastHit.chunk.GetVoxelInfo(index);
			GrowthData growthData = Manager.Get<GrowthManager>().GetGrowthData(GrowableInfoVoxel.GetKey(voxelInfo));
			if (growthData == null)
			{
				return false;
			}
			if (growableInfo == null || growableInfo.CurrentStageNumber() != growableInfo.growStages.Length + 1 || voxelInfo.GetVoxel() == 0)
			{
				return true;
			}
			return false;
		}

		private void UpdateSowing()
		{
			if (IsDifferentThanBefore(sowContext, hitInfo.gridRaycastHit))
			{
				RemoveContexts();
				sowContext = new SowContext
				{
					useAction = OnSow,
					info = hitInfo.gridRaycastHit
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_GROWABLE, sowContext);
			}
		}

		private bool IsDifferentThanBefore(InteractiveVoxelContext context, VoxelInfo target)
		{
			return context == null || context.info != target;
		}

		private void RemoveContexts()
		{
			if (waterContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_GROWABLE, waterContext);
				waterContext = null;
			}
			if (harvestContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_GROWABLE, harvestContext);
				harvestContext = null;
			}
			if (sowContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_GROWABLE, sowContext);
				sowContext = null;
			}
		}

		private void OnWater()
		{
			GameObject wateringCanPrefab = Manager.Get<FarmingManager>().wateringCanPrefab;
			GameObject gameObject = UnityEngine.Object.Instantiate(wateringCanPrefab);
			PlayerGraphic playerGraphic = PlayerGraphic.GetControlledPlayerInstance();
			playerGraphic.Grab(gameObject);
			VoxelInfo wateredInfo = GetWateringInfo();
			Action item = delegate
			{
				playerGraphic.UnGrab();
				Manager.Get<FarmingManager>().Water(wateredInfo);
				isWatering = false;
			};
			gameObject.GetComponent<ActionsContainer>().actions.Add(item);
			isWatering = true;
			Manager.Get<FarmingManager>().PlayClip(Manager.Get<FarmingManager>().wateringClip);
		}

		private void OnHarvest()
		{
			VoxelInfo harvestInfo = GetHarvestInfo();
			Manager.Get<FarmingManager>().Harvest(harvestInfo);
		}

		private void OnSow()
		{
			Manager.Get<FarmingManager>().Sow(hitInfo.gridRaycastHit);
		}

		private bool CanShowText(GrowableInfo growableInfo)
		{
			growableInfo = GetShowTextGrowableInfo(growableInfo);
			return growableInfo != null && Manager.Get<GrowthManager>().TimeToFullyGrown(growableInfo.GetUniqueId()) > 1;
		}

		private GrowableInfo GetShowTextGrowableInfo(GrowableInfo growableInfo)
		{
			if (Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(hitInfo.gridRaycastHit.GetVoxel()) is GrowerEarthVoxelEvents)
			{
				Index index = new Index(hitInfo.gridRaycastHit.index);
				index.y++;
				VoxelInfo voxelInfo = hitInfo.gridRaycastHit.chunk.GetVoxelInfo(index);
				growableInfo = growth.GetGrowableInfo(GrowableInfoVoxel.GetKey(voxelInfo));
			}
			return growableInfo;
		}

		private void ShowText(GrowableInfo growableInfo)
		{
			if (voxelTextContext == null)
			{
				voxelTextContext = new VoxelTextContext();
			}
			growableInfo = GetShowTextGrowableInfo(growableInfo);
			int num = Manager.Get<GrowthManager>().TimeToFullyGrown(growableInfo.GetUniqueId());
			string text = Manager.Get<TranslationsManager>().GetText("growth.time.left", "Time left: {0}");
			text = text.Replace("{0}", num.ToString());
			CanvasScaler component = Manager.Get<CanvasManager>().canvas.gameObject.GetComponent<CanvasScaler>();
			voxelTextContext.screenPosition = CameraController.instance.MainCamera.WorldToScreenPoint(Engine.VoxelInfoToPosition(hitInfo.gridRaycastHit)) - new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);
			voxelTextContext.text = text;
			voxelTextContext.hasToShow = true;
			if (!MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.VOXEL_TEXT, voxelTextContext))
			{
				MonoBehaviourSingleton<GameplayFacts>.get.NotifyFactChanged(Fact.VOXEL_TEXT);
			}
		}

		private void HideText()
		{
			if (voxelTextContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.VOXEL_TEXT, voxelTextContext);
				voxelTextContext = null;
			}
		}
	}
}
