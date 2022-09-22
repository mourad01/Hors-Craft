// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadingStateConnector
using Common.Managers.States.UI;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class LoadingStateConnector : UIConnector
	{
		public Image logoImage;

		public GameObject cubePrefab;

		private GameObject cubeInstance;

		public void InitCube(Voxel loadingBlock)
		{
			if (cubePrefab != null && cubeInstance == null)
			{
				cubeInstance = Object.Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
				LoadingCube component = cubeInstance.GetComponent<LoadingCube>();
				component.InitCube(loadingBlock);
			}
		}

		public void DestroyCube()
		{
			if (cubeInstance != null)
			{
				cubeInstance.SetActive(value: false);
			}
		}
	}
}
