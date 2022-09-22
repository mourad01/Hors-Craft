// DecompilerFi decompiler from Assembly-CSharp.dll class: States.BlueprintsShopStateConnector
using Common.Managers.States.UI;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace States
{
	public class BlueprintsShopStateConnector : UIConnector
	{
		[Serializable]
		public class Tab
		{
			public string name;

			public Button button;

			public ScrollableListMediator mediator;

			public Image topImage;

			public Image selectedImage;
		}

		public List<Tab> tabs;

		public Button returnButton;
	}
}
