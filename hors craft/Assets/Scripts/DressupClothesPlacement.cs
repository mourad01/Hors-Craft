// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupClothesPlacement
using System.Collections.Generic;
using UnityEngine;

public class DressupClothesPlacement : MonoBehaviour
{
	public Placement[] prefabPlacements;

	public Dictionary<DressupSkin.Placement, List<GameObject>> instantiatedObjects = new Dictionary<DressupSkin.Placement, List<GameObject>>();

	public bool getRandomHair;

	public bool ignoreStart;

	private DressupClothesSaveLoad clothesLoader = new DressupClothesSaveLoad();

	public Transform highBagPlacement;

	private int editorHairIndex = -1;

	private void Start()
	{
		if (!ignoreStart)
		{
			SpawnRandomHair(GetComponentInChildren<PlayerGraphic>().gender);
			if (!GetComponentInChildren<PlayerGraphic>().isNPC)
			{
				LoadClothesFromPrefs();
				SpawnShoes();
			}
		}
	}

	public void UpdateShoesMaterial()
	{
		if (instantiatedObjects.ContainsKey(DressupSkin.Placement.Shoes))
		{
			foreach (GameObject item in instantiatedObjects[DressupSkin.Placement.Shoes])
			{
				PlayerGraphic componentInChildren = GetComponentInChildren<PlayerGraphic>();
				Material clothes = SkinList.instance.GetClothes(componentInChildren.GetCurrentCloth(BodyPart.Legs));
				item.GetComponent<Renderer>().material = clothes;
			}
		}
	}

	public void SpawnItem(DressupSkin skin, bool bought = false)
	{
		if (skin == null)
		{
			return;
		}
		if (instantiatedObjects.ContainsKey(skin.placement))
		{
			if (instantiatedObjects[skin.placement] != null)
			{
				DespawnItem(skin.placement);
			}
			instantiatedObjects.Remove(skin.placement);
		}
		Placement placement = GetPlacement(skin.placement);
		List<GameObject> list = new List<GameObject>();
		Transform[] transforms = placement.transforms;
		foreach (Transform parent in transforms)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(skin.modelPrefab, parent);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			DressupItem component = gameObject.GetComponent<DressupItem>();
			if (component != null && skin.placement == DressupSkin.Placement.Bag)
			{
				component.ItemCreated(new DressupLongBagItemContext(highBagPlacement));
			}
			if (skin.placement == DressupSkin.Placement.Shoes)
			{
				PlayerGraphic componentInChildren = GetComponentInChildren<PlayerGraphic>();
				Material clothes = SkinList.instance.GetClothes(componentInChildren.GetCurrentCloth(BodyPart.Legs));
				gameObject.GetComponent<Renderer>().material = clothes;
			}
			list.Add(gameObject);
		}
		instantiatedObjects.Add(skin.placement, list);
		if (bought)
		{
			clothesLoader.SaveSkin(skin.placement, skin.id);
		}
	}

	public void TakeItemOff(DressupSkin skin)
	{
		DespawnItem(skin.placement);
		clothesLoader.SaveSkin(skin.placement, -1);
	}

	public void DespawnItem(DressupSkin.Placement placement)
	{
		if (instantiatedObjects.ContainsKey(placement))
		{
			List<GameObject> list = instantiatedObjects[placement];
			foreach (GameObject item in list)
			{
				UnityEngine.Object.Destroy(item);
			}
			list.Clear();
		}
	}

	private Placement GetPlacement(DressupSkin.Placement placement)
	{
		for (int i = 0; i < prefabPlacements.Length; i++)
		{
			if (prefabPlacements[i].placement == placement)
			{
				return prefabPlacements[i];
			}
		}
		UnityEngine.Debug.LogError("CLOTHING PLACEMENT NOT FOUND");
		return default(Placement);
	}

	public void LoadClothesFromPrefs()
	{
		Placement[] array = prefabPlacements;
		for (int i = 0; i < array.Length; i++)
		{
			Placement placement = array[i];
			DespawnItem(placement.placement);
		}
		Placement[] array2 = prefabPlacements;
		for (int j = 0; j < array2.Length; j++)
		{
			Placement placement2 = array2[j];
			int skinIndex = clothesLoader.GetSkinIndex(placement2.placement);
			SpawnById(skinIndex);
		}
	}

	public void SpawnById(int id)
	{
		if (id >= 0)
		{
			DressupSkin skin = DressupSkinList.instance.possibleSkins.Find((DressupSkin x) => x.id == id);
			SpawnItem(skin);
		}
	}

	public void HideHat()
	{
		if (instantiatedObjects.ContainsKey(DressupSkin.Placement.Hat) && !instantiatedObjects[DressupSkin.Placement.Hat].IsNullOrEmpty() && instantiatedObjects[DressupSkin.Placement.Hat][0] != null)
		{
			instantiatedObjects[DressupSkin.Placement.Hat][0].SetActive(value: false);
		}
	}

	public void ToggleHair(bool newState)
	{
		if (instantiatedObjects.ContainsKey(DressupSkin.Placement.Hair) && !(instantiatedObjects[DressupSkin.Placement.Hair][0] == null))
		{
			instantiatedObjects[DressupSkin.Placement.Hair][0].SetActive(newState);
		}
	}

	public void ShowHat()
	{
		if (instantiatedObjects.ContainsKey(DressupSkin.Placement.Hat) && instantiatedObjects[DressupSkin.Placement.Hat] != null && instantiatedObjects[DressupSkin.Placement.Hat].Count > 0 && !(instantiatedObjects[DressupSkin.Placement.Hat][0] == null))
		{
			instantiatedObjects[DressupSkin.Placement.Hat][0].SetActive(value: true);
		}
	}

	public void NextHair()
	{
		editorHairIndex++;
		List<DressupSkin> list = DressupSkinList.instance.possibleSkins.FindAll((DressupSkin x) => x.placement == DressupSkin.Placement.Hair);
		editorHairIndex %= list.Count;
		int num = 0;
		while (true)
		{
			if (num < list.Count)
			{
				if (num >= editorHairIndex)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		SpawnItem(list[num]);
	}

	public void SpawnRandomHair(Skin.Gender gender = Skin.Gender.FEMALE)
	{
		List<DressupSkin> list = DressupSkinList.instance.possibleSkins.FindAll((DressupSkin x) => x.placement == DressupSkin.Placement.Hair && x.gender == gender);
		if (!getRandomHair)
		{
			bool bought = clothesLoader.GetSkinIndex(DressupSkin.Placement.Hair) < 0;
			SpawnItem(list[0], bought);
		}
		else
		{
			int index = UnityEngine.Random.Range(0, list.Count - 1);
			SpawnItem(list[index]);
		}
	}

	public void SpawnHair(int id)
	{
		List<DressupSkin> list = DressupSkinList.instance.possibleSkins.FindAll((DressupSkin x) => x.placement == DressupSkin.Placement.Hair);
		SpawnItem(list[id]);
	}

	private void SpawnShoes()
	{
		List<DressupSkin> list = DressupSkinList.instance.possibleSkins.FindAll((DressupSkin x) => x.placement == DressupSkin.Placement.Shoes);
		SpawnItem(list[0], bought: true);
		UpdateShoesMaterial();
	}
}
