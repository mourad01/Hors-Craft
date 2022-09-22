// DecompilerFi decompiler from Assembly-CSharp.dll class: Mobs.MechEnemy
using Gameplay;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Mobs
{
	public class MechEnemy : ShootingEnemy, IFormChangeListener
	{
		private Coroutine updateColliders;

		public void OnFormChange()
		{
			FormChanger componentInChildren = GetComponentInChildren<FormChanger>();
			if (componentInChildren.GetPart("weapon").currentlyMountedPart != null)
			{
				shootStartTransforms = componentInChildren.GetPart("weapon").currentlyMountedPart.transform.FindChildrenRecursively("shootPlace").ToList();
				Weapon[] componentsInChildren = GetComponentsInChildren<Weapon>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					UnityEngine.Object.Destroy(componentsInChildren[i]);
				}
			}
			Health componentInChildren2 = GetComponentInChildren<Health>();
			Renderer[] componentsInChildren2 = GetComponentsInChildren<Renderer>();
			componentsInChildren2 = (componentInChildren2.renderers = (from r in componentsInChildren2
				where !(r is ParticleSystemRenderer)
				select r).ToArray());
			if (updateColliders == null)
			{
				updateColliders = StartCoroutine(UpdateColliders());
			}
		}

		private IEnumerator UpdateColliders()
		{
			yield return new WaitForEndOfFrame();
			AutoConfigureTransforms.AutoconfigureBoxCollider(this);
			AutoConfigureTransforms.AutoconfigureTransforms(this);
			updateColliders = null;
		}
	}
}
