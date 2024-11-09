using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct PerlinNoiseSettings
{
    public int seed;
    [Range(0, 0.5f)] public float frequency;
    [Range(1, 10)]   public int octaves;
    [Range(0, 1)]    public float persistence;
    public float lacunarity;
    public Vector2 offset;
    
    public PerlinNoiseSettings(int seed, float frequency, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        this.seed = seed;
        this.frequency = frequency;
        this.octaves = octaves;
        this.persistence = persistence;
        this.lacunarity = lacunarity;
        this.offset = offset;
    }
    
    public void SetOffset(Vector2 offset)
    {
        this.offset = offset;
    }
}
