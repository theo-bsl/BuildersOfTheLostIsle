using System;
using UnityEngine;

[Serializable]
public struct WorldSettings
{
    [Range(1, 10)] public int worldSize;
    [Range(0, 5)] public float shapePower;
    public int shapeDistance;
    
    public float groundElevation;
    public float waterLevel;
    
    public GameObject dirtLandPrefab;
    public GameObject waterLandPrefab;
    
    public WorldSettings(int worldSize, float shapePower, int shapeDistance, float groundElevation, float waterLevel, GameObject dirtLandPrefab, GameObject waterLandPrefab)
    {
        this.worldSize = worldSize;
        this.shapePower = shapePower;
        this.shapeDistance = shapeDistance;
        
        this.groundElevation = groundElevation;
        this.waterLevel = waterLevel;
        
        this.dirtLandPrefab = dirtLandPrefab;
        this.waterLandPrefab = waterLandPrefab;
    }
}
