using System;
using UnityEngine;

[Serializable]
public struct MeshSettings
{
    [Range(2, 255)] public int gridDensity;
    [Range(2, 100)] public int meshSize;
    [Range(1, 20)]  public float heightMultiplier;

    public MeshSettings(int gridDensity, int meshSize, float heightMultiplier)
    {
        this.gridDensity = gridDensity;
        this.meshSize = meshSize;
        this.heightMultiplier = heightMultiplier;
    }
}