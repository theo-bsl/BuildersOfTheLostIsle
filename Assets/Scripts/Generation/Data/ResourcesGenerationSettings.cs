using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct ResourcesGenerationSettings
{
    public LayerMask groundLayerMask;
    
    public int resourcesZoneSize;
    public float distanceBetweenResources;
    public int rejectionSamples;
    
    public Vector2 forestSize;
    public float minDistanceBetweenTrees;
    public float maxDistanceBetweenTrees;

    public int nbMaxResources;
    public int pourcentForests;
    public int pourcentRocks;
    public int pourcentBushes;
    
    public List<GameObject> forestsPrefabs;
    public List<GameObject> rocksPrefabs;
    public List<GameObject> bushesPrefabs;
    
    [HideInInspector] public float shapeDistance;
    [HideInInspector] public float shapePower;
    [HideInInspector] public int offset;
    
    public ResourcesGenerationSettings(LayerMask groundLayerMask, int resourcesZoneSize, float distanceBetweenResources, int rejectionSamples, Vector2 forestSize, float minDistanceBetweenTrees, float maxDistanceBetweenTrees, int nbMaxResources, int pourcentForests, int pourcentRocks, int pourcentBushes, List<GameObject> forestsPrefabs, List<GameObject> rocksPrefabs, List<GameObject> bushesPrefabs)
    {
        this.groundLayerMask = groundLayerMask;
        
        this.resourcesZoneSize = resourcesZoneSize;
        this.distanceBetweenResources = distanceBetweenResources;
        this.rejectionSamples = rejectionSamples;
        
        this.forestSize = forestSize;
        this.minDistanceBetweenTrees = minDistanceBetweenTrees;
        this.maxDistanceBetweenTrees = maxDistanceBetweenTrees;
        
        this.nbMaxResources = nbMaxResources;
        this.pourcentForests = pourcentForests;
        this.pourcentRocks = pourcentRocks;
        this.pourcentBushes = pourcentBushes;
        
        this.forestsPrefabs = forestsPrefabs;
        this.rocksPrefabs = rocksPrefabs;
        this.bushesPrefabs = bushesPrefabs;
        
        shapeDistance = 0;
        shapePower = 0;
        offset = 0;
    }

    public void SetWorldSettings(int WorldSize, float ShapeDistance, float ShapePower)
    {
        shapeDistance = ShapeDistance;
        shapePower = ShapePower;
        offset = (WorldSize - resourcesZoneSize) / 2;
    }

    public GameObject GetResourcePrefab(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Wood:
                return forestsPrefabs[Random.Range(0, forestsPrefabs.Count)];
            case ResourceType.Iron:
                return rocksPrefabs[Random.Range(0, rocksPrefabs.Count)];
            case ResourceType.Food:
                return bushesPrefabs[Random.Range(0, bushesPrefabs.Count)];
            default:
                return null;
        }
    }
}
