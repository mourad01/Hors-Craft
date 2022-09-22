// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldsFragment
using Common.Managers;
using States;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldsFragment : Fragment
{
	public class WorldsFragmentStart : FragmentStartParameter
	{
		public bool initFromPause;

		public WorldsFragmentStart(bool initFromPause)
		{
			this.initFromPause = initFromPause;
		}
	}

	private const string WAS_SHOWN = "was_world_shop_shown";

	public static bool areWorldsFromPause;

	public GameObject filterSelection;

	public GameObject worldPrefab;

	public GameObject scrollHolder;

	public GameObject filterPrefab;

	public GameObject filterHolder;

	public GameObject oneTimeTitle;

	public GridLayoutGroup gridOfWorlds;

	public float sizeOfCell = 0.45f;

	public string selectedFilter = "ALL";

	public List<GameObject> worldsElements;

	private Dictionary<string, GameObject> filters;

	private WorldsFragmentStart fragmentParameter;

	public Sprite starterImage;

	public override void Init(FragmentStartParameter parameter)
	{
		fragmentParameter = (parameter as WorldsFragmentStart);
		if (fragmentParameter == null)
		{
			fragmentParameter = new WorldsFragmentStart(initFromPause: true);
			areWorldsFromPause = true;
		}
		else
		{
			areWorldsFromPause = false;
		}
		CheckIfShowTitle();
		base.transform.localPosition = Vector3.zero;
		base.transform.localScale = Vector3.one;
		GetComponent<RectTransform>().offsetMax = Vector2.zero;
		GetComponent<RectTransform>().offsetMin = Vector2.zero;
		SetSizeOfCell();
		LoadFilters(Singleton<UltimateCraftModelDownloader>.get.GetFilters());
		OnFilterChange("ALL");
		Manager.Get<StateMachineManager>().StartCoroutine(CreateWorlds());
	}

	public void CheckIfShowTitle()
	{
		bool active = PlayerPrefs.GetInt("was_world_shop_shown", 0) == 0;
		oneTimeTitle.SetActive(active);
		PlayerPrefs.SetInt("was_world_shop_shown", 1);
	}

	public void LoadFilters(Dictionary<string, WorldFilter> downloadedFilters)
	{
		filters = new Dictionary<string, GameObject>();
		foreach (KeyValuePair<string, WorldFilter> downloadedFilter in downloadedFilters)
		{
			CreateFilterElement(downloadedFilter.Value);
		}
	}

	private void CreateFilterElement(WorldFilter filterType)
	{
		GameObject gameObject = Object.Instantiate(filterPrefab);
		filters.Add(filterType.id, gameObject);
		gameObject.transform.SetParent(filterHolder.transform);
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
		{
			OnFilterChange(filterType.id);
		});
		gameObject.GetComponentInChildren<Text>().text = filterType.text;
	}

	private void OnFilterChange(string filter)
	{
		SetSelectionPosition(filter);
		selectedFilter = filter;
		List<WorldData> sortedData = Sort(filter);
		ApplyFilter(sortedData);
	}

	private void SetSelectionPosition(string filter)
	{
		filterSelection.GetComponent<RectTransform>().anchoredPosition = filters[filter].GetComponent<RectTransform>().anchoredPosition;
		filterSelection.GetComponent<RectTransform>().offsetMin = filterSelection.GetComponent<RectTransform>().offsetMin.WithY(12f);
		filterSelection.GetComponent<RectTransform>().offsetMax = filterSelection.GetComponent<RectTransform>().offsetMax.WithY(0f);
	}

	private List<WorldData> Sort(string filter)
	{
		if (filter.Equals("ALL"))
		{
			return SortAllSimple();
		}
		if (filter.Equals("MY_WORLDS") || filter.Equals("MY WORLDS"))
		{
			return SortMyWorlds();
		}
		return SortByTag(Singleton<UltimateCraftModelDownloader>.get.GetFilter(filter).tag);
	}

	private List<WorldData> SortAllSimple()
	{
		List<WorldData> allWorlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		allWorlds.Sort((WorldData a, WorldData b) => a.sortOrder - b.sortOrder);
		return allWorlds;
	}

	private List<WorldData> SortAll()
	{
		HashSet<string> hashSet = new HashSet<string>();
		List<WorldData> allWorlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		List<WorldData> list = new List<WorldData>();
		List<WorldData> list2 = FilterTag("popular");
		if (list2.Count > 0)
		{
			list2.Sort((WorldData a, WorldData b) => b.cost - a.cost);
		}
		else
		{
			list2 = FilterTag("popular", bought: true);
			list2.Sort((WorldData a, WorldData b) => (int)(Singleton<PlayerData>.get.playerWorlds.GetTimeOfBuy(b.uniqueId) - Singleton<PlayerData>.get.playerWorlds.GetTimeOfBuy(a.uniqueId)));
		}
		if (list2.Count > 0)
		{
			list.Add(list2[0]);
			hashSet.Add(list2[0].uniqueId);
		}
		List<WorldData> list3 = FilterTag("new");
		int num = 0;
		if (list3.Count > 0)
		{
			list3.Sort((WorldData a, WorldData b) => (int)(b.serverTime - a.serverTime));
			for (int i = 0; i < list3.Count && i < 2; i++)
			{
				list.Add(list3[i]);
				hashSet.Add(list3[i].uniqueId);
				num++;
			}
		}
		if (num < 2)
		{
			list3 = FilterTag("new", bought: true);
			list3.Sort((WorldData a, WorldData b) => (int)(Singleton<PlayerData>.get.playerWorlds.GetTimeOfBuy(b.uniqueId) - Singleton<PlayerData>.get.playerWorlds.GetTimeOfBuy(a.uniqueId)));
			for (int j = 0; j < list3.Count; j++)
			{
				if (num >= 2)
				{
					break;
				}
				list.Add(list3[j]);
				hashSet.Add(list3[j].uniqueId);
				num++;
			}
		}
		List<WorldData> toAdd = SortMyWorlds();
		allWorlds.Sort((WorldData a, WorldData b) => (int)(b.serverTime - a.serverTime));
		AddToListRestricted(list, toAdd, hashSet);
		AddToListRestricted(list, list2, hashSet);
		AddToListRestricted(list, list3, hashSet);
		AddToListRestricted(list, allWorlds, hashSet);
		return list;
	}

	private void AddToListRestricted(List<WorldData> finalList, List<WorldData> toAdd, HashSet<string> restricted)
	{
		foreach (WorldData item in toAdd)
		{
			if (!restricted.Contains(item.uniqueId))
			{
				finalList.Add(item);
				restricted.Add(item.uniqueId);
			}
		}
	}

	private void ApplyFilter(List<WorldData> sortedData)
	{
		worldsElements.ForEach(delegate(GameObject element)
		{
			element.SetActive(value: false);
		});
		int num = 0;
		foreach (WorldData sortedDatum in sortedData)
		{
			GameObject gameObject = FindElementWithConnectedId(sortedDatum.uniqueId);
			if (!(gameObject == null))
			{
				gameObject.SetActive(value: true);
				gameObject.transform.SetSiblingIndex(num);
				num++;
			}
		}
	}

	private GameObject FindElementWithConnectedId(string id)
	{
		return worldsElements.Find(delegate(GameObject element)
		{
			WorldShopElement component = element.GetComponent<WorldShopElement>();
			return id == component.worldId;
		});
	}

	private List<WorldData> FilterTag(string tag, bool bought = false)
	{
		List<WorldData> allWorlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		allWorlds.RemoveAll(delegate(WorldData world)
		{
			if (world.tags == null)
			{
				return true;
			}
			bool flag = Singleton<PlayerData>.get.playerWorlds.IsWorldBought(world.uniqueId);
			if (bought)
			{
				flag = !flag;
			}
			return !world.tags.Contains(tag) || flag;
		});
		return allWorlds;
	}

	private List<WorldData> SortByTag(string tag)
	{
		List<WorldData> list = FilterTag(tag);
		list.Sort((WorldData a, WorldData b) => (int)(b.serverTime - a.serverTime));
		List<WorldData> list2 = FilterTag(tag, bought: true);
		list2.Sort((WorldData a, WorldData b) => (int)(b.serverTime - a.serverTime));
		list.AddRange(list2);
		return list;
	}

	private List<WorldData> SortMyWorlds()
	{
		List<WorldData> allWorlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		allWorlds.RemoveAll((WorldData world) => !Singleton<PlayerData>.get.playerWorlds.IsWorldBought(world.uniqueId));
		allWorlds.Sort((WorldData a, WorldData b) => (int)(Singleton<PlayerData>.get.playerWorlds.GetTimeOfBuy(b.uniqueId) - Singleton<PlayerData>.get.playerWorlds.GetTimeOfBuy(a.uniqueId)));
		return allWorlds;
	}

	private void SetSizeOfCell()
	{
		float num = GetComponent<RectTransform>().rect.width * sizeOfCell;
		Vector2 spacing = gridOfWorlds.spacing;
		float num2 = num - spacing.x;
		gridOfWorlds.cellSize = new Vector2(num2, num2 * 0.5f);
	}

	private IEnumerator CreateWorlds()
	{
		List<WorldData> worlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		worlds.Sort((WorldData a, WorldData b) => a.sortOrder - b.sortOrder);
		worldsElements = new List<GameObject>();
		foreach (WorldData world in worlds)
		{
			CreateElement(world);
			OnFilterChange(selectedFilter);
			yield return new WaitForSecondsRealtime(0.01f);
		}
	}

	private void OnWorldClick(WorldData data)
	{
		string uniqueId = data.uniqueId;
		if (Manager.Get<SavedWorldManager>().IsWorldReadyToPlay(uniqueId))
		{
			IfWorldIsValidForPlay(uniqueId);
		}
		else
		{
			ShowBuyWindow(data);
		}
	}

	private void ShowBuyWindow(WorldData data)
	{
		Manager.Get<StateMachineManager>().PushState<GenericPopupState>(new GenericPopupStateStartParameter
		{
			configureRightButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "world.shop.buy";
				text.defaultText = "BUY";
				button.onClick.AddListener(delegate
				{
					TryToBuy(data);
				});
			},
			configureLeftButton = delegate(Button button, TranslateText text)
			{
				text.translationKey = "world.shop.cancel";
				text.defaultText = "CANCEL";
				button.onClick.AddListener(delegate
				{
					Manager.Get<StateMachineManager>().PopState();
				});
			},
			configureMessage = delegate(TranslateText text)
			{
				text.translationKey = "world.shop.question";
				text.defaultText = "Do you want to buy this world?";
			}
		});
	}

	private void TryToBuy(WorldData data)
	{
		if (Singleton<PlayerData>.get.playerWorlds.IsWorldBought(data.uniqueId))
		{
			OnAbleToPlay(data);
		}
		else
		{
			Manager.Get<UltimateSoftCurrencyManager>().TryToBuySomething(data.cost, delegate
			{
				FindElementWithConnectedId(data.uniqueId).GetComponent<WorldShopElement>().InitButton(owned: true, delegate
				{
					OnWorldClick(data);
				});
				Singleton<PlayerData>.get.playerWorlds.OnWorldBought(data.uniqueId, data.cost);
				OnAbleToPlay(data);
			}, null, delegate
			{
				if (areWorldsFromPause)
				{
					Manager.Get<StateMachineManager>().PopStatesUntil<PauseState>();
				}
				else
				{
					Manager.Get<StateMachineManager>().PopStatesUntil<WorldShopState>();
				}
			});
		}
	}

	private void OnAbleToPlay(WorldData data)
	{
		EnableLoading(enable: true);
		Manager.Get<SavedWorldManager>().DownloadWorldForPlayer(data.uniqueId, delegate
		{
			IfWorldIsValidForPlay(data.uniqueId);
		});
	}

	private void Update()
	{
	}

	private void EnableLoading(bool enable)
	{
		Manager.Get<StateMachineManager>().PushState<EmptyLoadingState>(new LoadingStartParameter("changing", "Changing World"));
	}

	private void IfWorldIsValidForPlay(string id)
	{
		if (Manager.Get<SavedWorldManager>().CheckIfCurrentAndValid(id))
		{
			Manager.Get<StateMachineManager>().PushState<GameplayState>();
		}
		else
		{
			ChangeWorld(id);
		}
		Manager.Get<SavedWorldManager>().OnPlayWorld(id);
	}

	private void ChangeWorld(string id)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<WorldsChanger>().ChangeWorld(id, !fragmentParameter.initFromPause);
	}

	private GameObject CreateElement(WorldData data)
	{
		WorldShopElement element = Object.Instantiate(worldPrefab.gameObject).GetComponent<WorldShopElement>();
		string key = (!string.IsNullOrEmpty(data.descriptionId)) ? data.descriptionId : "shop.description.empty";
		string key2 = (!string.IsNullOrEmpty(data.titleId)) ? data.titleId : "shop.title.empty";
		element.Init(data.uniqueId, (!data.startingWorld) ? null : starterImage, data.GetBlocks(), data.cost, Manager.Get<TranslationsManager>().GetText(key2, "World"), Manager.Get<TranslationsManager>().GetText(key, "Start your adventure here!"), data.tags, Singleton<PlayerData>.get.playerWorlds.IsWorldBought(data.uniqueId), delegate
		{
			OnWorldClick(data);
		});
		Manager.Get<SavedWorldManager>().TryToShopSprite(data.uniqueId, delegate(Sprite sprite)
		{
			if (element.mainScreen != null)
			{
				element.mainScreen.sprite = sprite;
			}
		});
		Manager.Get<SavedWorldManager>().StartCoroutine(RevalidateBlocksFromElements(element));
		element.gameObject.transform.SetParent(scrollHolder.transform);
		element.transform.localScale = Vector3.one;
		worldsElements.Add(element.gameObject);
		return element.gameObject;
	}

	private IEnumerator RevalidateBlocksFromElements(WorldShopElement element)
	{
		yield return new WaitForEndOfFrame();
		element.SetBlocksSpacing();
		SetSelectionPosition(selectedFilter);
	}
}
