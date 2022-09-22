// DecompilerFi decompiler from Assembly-CSharp.dll class: DropAmmo
using UnityEngine;

public class DropAmmo : MonoBehaviour
{
	public GameObject[] ammoPrefabs;

	public void Drop(Vector3 position)
	{
		int num = UnityEngine.Random.Range(1, ammoPrefabs.Length);
		int value = UnityEngine.Random.Range(3, 7);
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ammoPrefabs[Random.Range(0, ammoPrefabs.Length)]);
			gameObject.GetComponent<AmmoObject>().Init(value, position + UnityEngine.Random.insideUnitSphere, addRandomForce: true);
		}
	}
}
