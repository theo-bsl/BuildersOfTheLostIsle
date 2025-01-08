using System.Collections.Generic;
using Generation.ResourcesGeneration;
using UnityEngine;

public class WorldGenerator
{
    private static TerrainGenerator _terrainGenerator;
    private static ResourcesGenerator _resourcesGenerator;
    
    private WorldSettings _worldSettings;
    private MeshSettings _meshSettings;
    private PerlinNoiseSettings _perlinNoiseSettings;
    private ResourcesGenerationSettings _resourcesGenerationSettings;

    private Vector3 _worldCenter;
    
    public WorldGenerator(Transform transform, WorldSettings worldSettings, MeshSettings meshSettings, PerlinNoiseSettings perlinNoiseSettings, ResourcesGenerationSettings resourcesGenerationSettings)
    {
        _terrainGenerator = new TerrainGenerator(transform);
        _resourcesGenerator = new ResourcesGenerator(transform);
        
        _worldSettings = worldSettings;
        _meshSettings = meshSettings;
        _perlinNoiseSettings = perlinNoiseSettings;
        _resourcesGenerationSettings = resourcesGenerationSettings;
        
        _worldCenter = new Vector3(worldSettings.worldSize * meshSettings.meshSize / 2f, 0, worldSettings.worldSize * meshSettings.meshSize / 2f);
    }

    public void GenerateTerrain()
    {
        _terrainGenerator.GenerateTerrain(_worldSettings, _meshSettings, _perlinNoiseSettings);
    }
    
    public Dictionary<ResourceType, List<Transform>> GenerateResources()
    {
        return _resourcesGenerator.GenerateAllResources(_resourcesGenerationSettings);
    }

    public Vector3 WorldCenter => _worldCenter;
}
