// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressFragment
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ProgressFragment : Fragment
	{
		public Slider levelSlider;

		public Text levelText;

		public Text levelNumber;

		public Button rankingButton;

		public Button achievementsButton;

		public List<CurrencySlot> currencySlots = new List<CurrencySlot>();

		public GameObject progressStatExample;

		public override void Init(FragmentStartParameter parameter)
		{
			base.Init(parameter);
		}

		public override void UpdateFragment()
		{
			base.UpdateFragment();
			GetComponent<ProgressFragmentBehaviour>().UpdateFragment();
		}

		public void AddProgressStat(string text, string value, int below = -1)
		{
			if (!(progressStatExample == null))
			{
				GameObject gameObject = Object.Instantiate(progressStatExample);
				gameObject.transform.SetParent(progressStatExample.transform.parent, worldPositionStays: false);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(value: true);
				Text[] componentsInChildren = gameObject.GetComponentsInChildren<Text>();
				componentsInChildren[0].text = text;
				componentsInChildren[1].text = value;
				if (below > -1)
				{
					gameObject.transform.SetSiblingIndex(progressStatExample.transform.GetSiblingIndex() + 1 + below);
				}
				else
				{
					gameObject.transform.SetSiblingIndex(progressStatExample.transform.GetSiblingIndex() + 1);
				}
			}
		}

		public void AddProgressStat(Sprite icon, string value)
		{
			if (!(progressStatExample == null))
			{
				GameObject gameObject = Object.Instantiate(progressStatExample);
				gameObject.transform.SetParent(progressStatExample.transform.parent, worldPositionStays: false);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(value: true);
				Image componentInChildren = gameObject.GetComponentInChildren<Image>();
				componentInChildren.sprite = icon;
				Text componentInChildren2 = gameObject.GetComponentInChildren<Text>();
				componentInChildren2.text = value;
			}
		}

		public void ClearStats()
		{
			if (progressStatExample == null)
			{
				return;
			}
			Transform parent = progressStatExample.transform.parent;
			for (int num = parent.childCount - 1; num >= 0; num--)
			{
				if (!(parent.GetChild(num).gameObject == progressStatExample) && !(parent.GetChild(num).gameObject.GetComponent<PleaseDoNotKillMeImNotStat>() != null))
				{
					UnityEngine.Object.DestroyImmediate(parent.GetChild(num).gameObject);
				}
			}
		}
	}
}
