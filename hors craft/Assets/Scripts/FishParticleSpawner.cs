// DecompilerFi decompiler from Assembly-CSharp.dll class: FishParticleSpawner
using UnityEngine;

public class FishParticleSpawner : MonoBehaviour
{
	[SerializeField]
	private Mesh[] fishMeshes;

	public GameObject fishParticleSystemPrefab;

	private ParticleSystemRenderer pSystemRenderer;

	private void Awake()
	{
		pSystemRenderer = fishParticleSystemPrefab.GetComponent<ParticleSystemRenderer>();
		Mesh[] array = new Mesh[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = fishMeshes[Random.Range(0, fishMeshes.Length)];
		}
		pSystemRenderer.SetMeshes(array);
		GameObject gameObject = UnityEngine.Object.Instantiate(fishParticleSystemPrefab);
		gameObject.transform.SetParent(base.transform);
	}
}
