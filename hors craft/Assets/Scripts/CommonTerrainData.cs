// DecompilerFi decompiler from Assembly-CSharp.dll class: CommonTerrainData
using System;
using Uniblocks;

[Serializable]
public class CommonTerrainData
{
	public AnimationCurveSerializer terrainHeightRamp;

	public bool generateStreets;

	public float scaleWorldInX;

	public float scaleWorldInZ;

	public CommonTerrainGenerator.FlowersConfig flowers;

	public CommonTerrainGenerator.SpecialFlowersConfig specialFlowers;

	public CommonTerrainGenerator.TreesConfig trees;

	public CommonTerrainGenerator.AnimalsSpawnerConfig animals;

	public CommonTerrainGenerator.OtherIDsConfig otherIDs;

	public CommonTerrainGenerator.StreetsConfig streets;

	public int waterHeight;

	public float perlinTerrainScaleX;

	public float perlinTerrainScaleZ;

	public bool fastWater = true;

	public CommonTerrainData(CommonTerrainGenerator generator)
	{
		terrainHeightRamp = new AnimationCurveSerializer(generator.terrainHeightRamp);
		generateStreets = generator.generateStreets;
		scaleWorldInX = generator.scaleWorldInX;
		scaleWorldInZ = generator.scaleWorldInZ;
		flowers = generator.flowers;
		specialFlowers = generator.specialFlowers;
		trees = generator.trees;
		animals = generator.animals;
		otherIDs = generator.otherIDs;
		streets = generator.streets;
		waterHeight = generator.waterHeight;
		perlinTerrainScaleX = generator.perlinTerrainScaleX;
		perlinTerrainScaleZ = generator.perlinTerrainScaleZ;
		fastWater = generator.fastWater;
	}

	public void ApplyToCommonGenerator(CommonTerrainGenerator generator)
	{
		generator.terrainHeightRamp = terrainHeightRamp.Deserialize();
		generator.generateStreets = generateStreets;
		generator.scaleWorldInX = scaleWorldInX;
		generator.scaleWorldInZ = scaleWorldInZ;
		generator.flowers = flowers;
		generator.specialFlowers = specialFlowers;
		generator.trees = trees;
		generator.animals = animals;
		generator.otherIDs = otherIDs;
		generator.streets = streets;
		generator.waterHeight = waterHeight;
		generator.perlinTerrainScaleX = perlinTerrainScaleX;
		generator.perlinTerrainScaleZ = perlinTerrainScaleZ;
		generator.fastWater = fastWater;
	}
}
