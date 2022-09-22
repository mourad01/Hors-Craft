// DecompilerFi decompiler from Assembly-CSharp.dll class: States.OnVoxelTextModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class OnVoxelTextModule : GameplayModule
	{
		public Text text;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.VOXEL_TEXT
		};

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			VoxelTextContext voxelTextContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<VoxelTextContext>(Fact.VOXEL_TEXT).FirstOrDefault();
			if (voxelTextContext != null && voxelTextContext.hasToShow)
			{
				text.rectTransform.localPosition = voxelTextContext.screenPosition;
				text.transform.localScale = voxelTextContext.scale;
				text.text = voxelTextContext.text;
				text.gameObject.SetActive(value: true);
			}
			else
			{
				text.gameObject.SetActive(value: false);
			}
		}
	}
}
