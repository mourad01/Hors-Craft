// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CraftingResourcesFragment
using Common.Managers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CraftingResourcesFragment : Fragment
	{
		public GameObject resourcesPrefab;

		public GameObject resourcesListParent;

		public List<GameObject> resourcesList;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
			InitResources(Manager.Get<CraftingManager>().GetResourcesList(getOnlyVisibleOnList: true));
		}

		private void InitResources(List<Resource> currentResources)
		{
			if (resourcesList == null || resourcesList.Count == 0)
			{
				foreach (Resource currentResource in currentResources)
				{
					if (currentResource.id >= 0)
					{
						GameObject gameObject = Object.Instantiate(resourcesPrefab, resourcesListParent.transform);
						gameObject.SetActive(value: true);
						Sprite image = Manager.Get<CraftingManager>().GetResourceDefinition(currentResource.id).GetImage();
						gameObject.GetComponent<CraftItem>().Init(currentResource.id, Singleton<PlayerData>.get.playerItems.GetResourcesCount(currentResource.id), CraftableStatus.Undefined, image, "x{0}", resource: true);
						gameObject.transform.localScale = Vector3.one;
						gameObject.transform.SetAsLastSibling();
						resourcesList.Add(gameObject);
						gameObject.GetComponent<Button>().onClick.AddListener(delegate
						{
							Manager.Get<ToastManager>().ShowToast(Manager.Get<TranslationsManager>().GetText("crafting.yourResources", "This are your resources"));
						});
						gameObject.GetComponent<CraftItem>().EnableNotification(Manager.Get<QuestManager>().IsThisResourceNotified(currentResource.id));
					}
				}
			}
			else
			{
				UpdateResourcesList();
			}
		}

		public override void UpdateFragment()
		{
			UpdateResourcesList();
		}

		public void UpdateResourcesList()
		{
			foreach (GameObject resources in resourcesList)
			{
				CraftItem component = resources.GetComponent<CraftItem>();
				if (!(component == null))
				{
					int resourcesCount = Singleton<PlayerData>.get.playerItems.GetResourcesCount(component.id);
					component.Reinitialize(resourcesCount, CraftableStatus.Undefined, resource: true);
				}
			}
		}
	}
}
