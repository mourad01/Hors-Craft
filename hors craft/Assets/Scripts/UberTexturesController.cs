// DecompilerFi decompiler from Assembly-CSharp.dll class: UberTexturesController
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class UberTexturesController : MonoBehaviour
{
	private readonly string[] texturesFields = new string[3]
	{
		"_UpText",
		"_SideText",
		"_BottomText"
	};

	[SerializeField]
	private List<UberMainConfig> configs = new List<UberMainConfig>();

	[SerializeField]
	private List<UberConfig> singleConfigs = new List<UberConfig>();

	private Dictionary<Voxel, Material> materialsToRestore = new Dictionary<Voxel, Material>();

	private Dictionary<int, Material> _voxel2Material;

	private Dictionary<int, Sprite> _voxel2Sprite;

	private Dictionary<int, Material> volxel2Material
	{
		get
		{
			if (_voxel2Material == null)
			{
				InitDictionary();
			}
			return _voxel2Material;
		}
	}

	private Dictionary<int, Sprite> voxel2Sprite
	{
		get
		{
			if (_voxel2Sprite == null)
			{
				InitDictionary();
			}
			return _voxel2Sprite;
		}
	}

	public List<UberConfig> allConfigs
	{
		get
		{
			List<UberConfig> list = new List<UberConfig>(singleConfigs);
			list.AddRange(configs.SelectMany((UberMainConfig a) => a.uberConfigs));
			return list;
		}
	}

	private void InitDictionary()
	{
		_voxel2Material = new Dictionary<int, Material>();
		_voxel2Sprite = new Dictionary<int, Sprite>();
		allConfigs.ForEach(delegate(UberConfig a)
		{
			if (a.uberMaterial == null)
			{
				InitMaterial(a);
			}
			volxel2Material.AddOrReplace(a.voxelID, a.uberMaterial);
			if (a.customSprite != null)
			{
				voxel2Sprite.AddOrReplace(a.voxelID, a.customSprite);
			}
		});
	}

	public void SetupVoxels(Dictionary<int, Voxel> voxels)
	{
		foreach (KeyValuePair<int, Voxel> voxel in voxels)
		{
			if (volxel2Material.ContainsKey(voxel.Key))
			{
				materialsToRestore.Add(voxel.Value, voxel.Value.VMaterial);
				voxel.Value.VMaterial = volxel2Material[voxel.Key];
				if (voxel2Sprite.ContainsKey(voxel.Key))
				{
					voxel.Value.useCustomSprite = true;
					voxel.Value.voxelSprite = voxel2Sprite[voxel.Key];
				}
			}
		}
	}

	private void OnDestroy()
	{
		foreach (KeyValuePair<Voxel, Material> item in materialsToRestore)
		{
			item.Key.VMaterial = item.Value;
			item.Key.useCustomSprite = false;
			item.Key.voxelSprite = null;
		}
		allConfigs.ForEach(delegate(UberConfig a)
		{
			if (a.dirtyMaterial && a.uberMaterial != null)
			{
				UnityEngine.Object.DestroyImmediate(a.uberMaterial);
				a.uberMaterial = null;
			}
		});
	}

	private void InitMaterial(UberConfig config)
	{
		config.uberMaterial = new Material(config.uberShader);
		config.dirtyMaterial = true;
		if (config.uberTexture.Length == 1)
		{
			config.uberMaterial.SetTexture("_MainTex2", config.uberTexture[0]);
			config.uberMaterial.SetColor("_Tint", config.tint);
		}
		else
		{
			for (int i = 0; i < config.uberTexture.Length; i++)
			{
				config.uberMaterial.SetTexture(texturesFields[i], config.uberTexture[i]);
			}
		}
		config.uberMaterial.SetFloat("_BlockSize2", config.blockSize);
		config.uberMaterial.SetTexture("_Normals", config.uberNormal);
		config.uberMaterial.SetTexture("_EdgeNormals", config.edgeTexture);
	}
}
