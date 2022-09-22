// DecompilerFi decompiler from Assembly-CSharp.dll class: States.DefaultUniversalPopup
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class DefaultUniversalPopup : MonoBehaviour
	{
		public Button returnButton;

		public List<Button> actionButtons;

		public List<GameObject> objects;

		public Button GetActionButton(string name)
		{
			return (from b in actionButtons
				where b.name.ToLower() == name.ToLower()
				select b).FirstOrDefault();
		}

		public GameObject GetObject(string name)
		{
			return (from b in objects
				where b.name.ToLower() == name.ToLower()
				select b).FirstOrDefault();
		}
	}
}
