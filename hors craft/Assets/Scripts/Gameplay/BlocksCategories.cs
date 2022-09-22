// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.BlocksCategories
using Common.Gameplay;
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;
using UnityToolbag;

namespace Gameplay
{
	public class BlocksCategories : MonoBehaviourSingleton<BlocksCategories>
	{
		[Serializable]
		public class Category
		{
			public string name;

			public Sprite icon;

			public Reward reward;

			[Reorderable]
			public List<GameObject> voxels;
		}

		public List<Category> categories;

		public override void Init()
		{
			ConfigModule module = Manager.Get<ModelManager>().configSettings;
			if (categories != null)
			{
				categories.ForEach(delegate(Category cat)
				{
					Reward reward = cat.reward;
					Reward reward2 = (Reward)ScriptableObject.CreateInstance(reward.GetType());
					reward2.amount = module.GetExpAmountForCategory(cat.name);
					cat.reward = reward2;
				});
			}
		}

		public List<GameObject> GetVoxels(string cat)
		{
			return categories.FirstOrDefault((Category c) => c.name == cat)?.voxels.ToList();
		}

		public Category GetCategory(Voxel voxel)
		{
			return categories.FirstOrDefault((Category cat) => cat.voxels.Any((GameObject vox) => vox.GetComponent<Voxel>() == voxel));
		}
	}
}
