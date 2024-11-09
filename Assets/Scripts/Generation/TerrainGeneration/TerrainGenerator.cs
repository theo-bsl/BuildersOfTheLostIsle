using System.Collections.Generic;
using Calcul;
using UnityEngine;

public class TerrainGenerator
{
    private GameObject _dirtLandsHolder;
    private GameObject _waterLandsHolder;
    private readonly List<DirtLand> _dirtLands  = new List<DirtLand>();
    private readonly List<WaterLand> _waterLands = new List<WaterLand>();

    public TerrainGenerator(Transform holdersParent)
    {
        InitHolder(holdersParent);
    }
    
    private void InitHolder(Transform holdersParent)
    {
        _dirtLandsHolder = new GameObject("DirtLands");
        _dirtLandsHolder.transform.SetParent(holdersParent);
        
        _waterLandsHolder = new GameObject("WaterLands");
        _waterLandsHolder.transform.SetParent(holdersParent);
    }
    
    private GameObject InstantiateLand(GameObject landPrefab, Transform parent, Vector3 position, string chunkName)
    {
        GameObject land = Object.Instantiate(landPrefab, parent);
        land.name = chunkName;
        land.transform.position = position;

        return land;
    }

    public void GenerateTerrain(WorldSettings worldSettings, MeshSettings meshSettings, PerlinNoiseSettings perlinNoiseSettings)
    {
        Vector3 chunkPosition = Vector3.zero;
        Vector2 chunkPivotPosition = Vector2.zero;
        Vector2 chunkOppositePivotPosition = Vector2.zero;
        Vector2 worldCenter = new Vector2(worldSettings.worldSize * meshSettings.meshSize / 2f, worldSettings.worldSize * meshSettings.meshSize / 2f);

        for (int x = 0; x < worldSettings.worldSize; x++)
        {
            for (int z = 0; z < worldSettings.worldSize; z++)
            {
                chunkPosition.Set(x * meshSettings.meshSize, 0, z * meshSettings.meshSize);
                chunkPivotPosition.Set(x * meshSettings.meshSize, z * meshSettings.meshSize);
                chunkOppositePivotPosition.Set(x * meshSettings.meshSize + meshSettings.meshSize, z * meshSettings.meshSize + meshSettings.meshSize);
                perlinNoiseSettings.offset.Set(x * (meshSettings.gridDensity - 1), z * (meshSettings.gridDensity - 1));

                DirtLand dirtLand = InstantiateLand(worldSettings.dirtLandPrefab, _dirtLandsHolder.transform, chunkPosition, $"DirtLand{x}{z}").GetComponent<DirtLand>();

                dirtLand.SetChunkSettings(worldSettings.worldSize, worldSettings.shapePower, worldSettings.shapeDistance, worldSettings.groundElevation);
                dirtLand.SetMeshSettings(meshSettings);
                dirtLand.SetPerlinNoiseSettings(perlinNoiseSettings);
                dirtLand.GenerateMesh();

                _dirtLands.Add(dirtLand);
                
                float distanceToPivot = Calculate.Distance(chunkPivotPosition, worldCenter, worldSettings.shapePower);
                float distanceToOpposite = Calculate.Distance(chunkOppositePivotPosition, worldCenter, worldSettings.shapePower);
                
                bool makeWater = (distanceToPivot > worldSettings.shapeDistance || distanceToOpposite > worldSettings.shapeDistance);

                if (!makeWater) continue;
                WaterLand waterLand = InstantiateLand(worldSettings.waterLandPrefab, _waterLandsHolder.transform, chunkPosition, $"WaterLand{x}{z}").GetComponent<WaterLand>();
                waterLand.SetChunkSettings(meshSettings.gridDensity, meshSettings.meshSize);
                waterLand.SetWaterSettings(worldSettings.waterLevel);
                waterLand.GenerateMesh();
                _waterLands.Add(waterLand);
            }
        }
        
        CompleteTerrainGeneration();
    }

    private void CompleteTerrainGeneration()
    {
        foreach (var dirtLand in _dirtLands)
        {
            dirtLand.CompleteMeshGeneration();
        }
    }
}
