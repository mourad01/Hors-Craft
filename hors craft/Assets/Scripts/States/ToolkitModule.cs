// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ToolkitModule
using Common.GameUI;
using Common.Managers;
using Common.Utils;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using TsgCommon;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ToolkitModule : GameplayModule
	{
		public class Slot
		{
			public int i;

			public ushort voxel;

			public float pressedTime;

			public SimpleRepeatButton repeatButton;

			public Text countText;
		}

		private const string KEY = "toolkit.slots";

		private const float HOLD_TIME = 1f;

		public List<GameObject> slotGOs;

		private List<Slot> slots;

		private int selectedSlot;

		private bool blockLimitEnabled;

		protected override Fact[] listenedFacts => new Fact[3]
		{
			Fact.MCPE_STEERING,
			Fact.BLOCK_HELD,
			Fact.BLOCK_COUNT_CHANGED
		};

		public override void Init()
		{
			if (Application.isPlaying)
			{
				selectedSlot = 0;
				blockLimitEnabled = Manager.Get<ModelManager>().blocksUnlocking.IsBlocksLimitedEnabled();
				LoadSlots();
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.TOOLKIT_ENABLED);
				base.Init();
			}
		}

		protected override void Update()
		{
			for (int i = 0; i < slots.Count; i++)
			{
				Slot slot = slots[i];
				if (slot.repeatButton.pressed)
				{
					slot.pressedTime += Time.deltaTime;
					if (slot.pressedTime > 1f)
					{
						OnButtonHold(slot);
						slot.pressedTime = 0f;
						slot.repeatButton.transform.localScale = Vector3.one;
					}
				}
				else if (slot.pressedTime > 0f)
				{
					OnButtonPressed(slot);
					slot.pressedTime = 0f;
				}
			}
		}

		private void OnButtonPressed(Slot slot)
		{
			selectedSlot = slot.i;
			if (slot.voxel == ushort.MaxValue)
			{
				BlocksPopupState.Show();
			}
			else
			{
				ExampleInventory.HeldBlock = slot.voxel;
			}
		}

		private void OnButtonHold(Slot slot)
		{
			selectedSlot = slot.i;
			BlocksPopupState.Show();
		}

		private void LoadSlots()
		{
			if (Application.isPlaying)
			{
				for (int i = 0; i < slotGOs.Count; i++)
				{
					GetIcon(slotGOs[i]).enabled = false;
				}
				if (PlayerPrefs.HasKey("toolkit.slots"))
				{
					LoadSlotsFromPrefs();
					return;
				}
				CreateSlots();
				SetCurrentSlot(ExampleInventory.HeldBlock);
			}
		}

		private void LoadSlotsFromPrefs()
		{
			List<ushort> list = JSONHelper.Deserialize<List<ushort>>(PlayerPrefs.GetString("toolkit.slots"));
			slots = new List<Slot>();
			for (int i = 0; i < slotGOs.Count; i++)
			{
				Slot slot = new Slot();
				slot.i = i;
				slot.pressedTime = 0f;
				if (i < list.Count)
				{
					slot.voxel = list[i];
					GetIcon(slotGOs[i]).sprite = VoxelSprite.GetVoxelSprite(slot.voxel);
					GetIcon(slotGOs[i]).enabled = true;
				}
				else
				{
					slot.voxel = ushort.MaxValue;
					GetIcon(slotGOs[i]).enabled = false;
				}
				slot.repeatButton = slotGOs[i].GetComponentInChildren<SimpleRepeatButton>();
				slot.countText = slotGOs[i].GetComponentInChildren<Text>();
				SetBlockCount(slot);
				slots.Add(slot);
			}
		}

		private void CreateSlots()
		{
			slots = new List<Slot>();
			for (int i = 0; i < slotGOs.Count; i++)
			{
				slots.Add(new Slot
				{
					i = i,
					voxel = ushort.MaxValue,
					pressedTime = 0f,
					repeatButton = slotGOs[i].GetComponentInChildren<SimpleRepeatButton>(),
					countText = slotGOs[i].GetComponentInChildren<Text>()
				});
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			bool flag = MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING);
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
			if (changedFacts.Contains(Fact.BLOCK_HELD))
			{
				HeldBlockContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<HeldBlockContext>(Fact.BLOCK_HELD);
				if (factContext != null)
				{
					SetCurrentSlot(factContext.block);
				}
			}
			if (changedFacts.Contains(Fact.BLOCK_COUNT_CHANGED))
			{
				slots.ForEach(delegate(Slot s)
				{
					SetBlockCount(s);
				});
			}
		}

		private Slot FindFirstEmptyOrDefault()
		{
			return slots.FirstOrDefault((Slot s) => s.voxel == ushort.MaxValue) ?? slots[selectedSlot];
		}

		private void SetCurrentSlot(ushort voxel)
		{
			SetSlot(slots[selectedSlot], voxel);
		}

		private void SetSlot(Slot slot, ushort voxel)
		{
			slot.voxel = voxel;
			GetIcon(slotGOs[slot.i]).sprite = VoxelSprite.GetVoxelSprite(voxel);
			GetIcon(slotGOs[slot.i]).enabled = true;
			SetBlockCount(slot);
			SaveSlots();
		}

		private void SetBlockCount(Slot slot)
		{
			if (blockLimitEnabled && slot.voxel != ushort.MaxValue && !TsgCommon.MonoBehaviourSingleton<AdUnlockHandler>.get.IsAdsFree)
			{
				int blockCount = Singleton<BlocksController>.get.GetBlockCount(slot.voxel);
				if (blockCount == int.MaxValue || blockCount == -1 || Engine.GetVoxelType(slot.voxel).rarityCategory == Voxel.RarityCategory.UNLIMITED)
				{
					slot.countText.text = string.Empty;
				}
				else
				{
					slot.countText.text = blockCount.ToString();
				}
			}
			else
			{
				slot.countText.text = string.Empty;
			}
		}

		private void SaveSlots()
		{
			PlayerPrefs.SetString("toolkit.slots", JSONHelper.ToJSON((from s in slots
				select s.voxel).ToList()));
		}

		private Image GetIcon(GameObject slot)
		{
			return slot.GetComponentsInChildren<Image>().Last();
		}
	}
}
