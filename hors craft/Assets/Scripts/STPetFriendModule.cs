// DecompilerFi decompiler from Assembly-CSharp.dll class: STPetFriendModule
using Common.Model;
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "STPetFriendModule", menuName = "SaveTransformsModule/STPetFriendModule")]
public class STPetFriendModule : STPetModule
{
	public override GameObject Spawn(Settings settings)
	{
		GameObject go = base.Spawn(settings);
		PettableFriend friend = go.GetComponent<PettableFriend>();
		if (friend == null && alive.Count() == 0)
		{
			friend = go.AddComponent<PettableFriend>();
			PettableFriend pettableFriend = friend;
			pettableFriend.OnAwake = (Action)Delegate.Combine(pettableFriend.OnAwake, (Action)delegate
			{
				friend.pettable.ForceTamed();
				OverrideLogic(go);
			});
		}
		return go;
	}

	private void OverrideLogic(GameObject go)
	{
		Pettable component = go.GetComponent<Pettable>();
		if (component != null)
		{
			component.FollowPlayerMode();
		}
	}

	public override GameObject GetPrefab(string name)
	{
		return PetsList.petNameToGameObject[name];
	}

	public override bool ContainsPrefabFor(string name)
	{
		return PetsList.petNameToGameObject.ContainsKey(name);
	}

	public void ClearAllFreezed()
	{
		freezed.Clear();
	}
}
