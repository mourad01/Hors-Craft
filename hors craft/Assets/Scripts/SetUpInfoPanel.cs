// DecompilerFi decompiler from Assembly-CSharp.dll class: SetUpInfoPanel
using UnityEngine;

public class SetUpInfoPanel : MonoBehaviour
{
	public bool showPortrait = true;

	public bool showName = true;

	public GameObject portraitPrefab;

	public string defaultName;

	public string translationName;

	private void Awake()
	{
		GenericTutorial component = GetComponent<GenericTutorial>();
		component.defaultConfigInfoPaneAfterSpawn = SetSpriteAndName;
	}

	private void SetSpriteAndName(GameObject infoPanel)
	{
		if (showPortrait)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(portraitPrefab, infoPanel.transform.FindChildRecursively("Panel"), worldPositionStays: false);
			gameObject.transform.SetSiblingIndex(1);
		}
		if (showName)
		{
			TranslateText component = infoPanel.transform.FindChildRecursively("Name").GetComponent<TranslateText>();
			component.defaultText = defaultName;
			component.translationKey = translationName;
			component.ForceRefresh();
		}
		else
		{
			infoPanel.transform.FindChildRecursively("PortraitFrame").gameObject.SetActive(value: false);
		}
	}
}
