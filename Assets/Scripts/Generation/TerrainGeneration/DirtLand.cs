using System.Collections.Generic;
using Calcul;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class DirtLand : MonoBehaviour
{
    [Header("Chunk Settings")]
                   private float _worldSize;
    [Range(0, 5)]  private float _shapePower;
                   private float _shapeDistance;
    [Range(0, 15)] private float _groundElevation;

    [Header("Mesh Settings")]
    [Range(2, 255)] private int     _gridDensity;
    [Range(2, 100)] private int     _meshSize;
    [Range(1, 15)]  private float   _heightMultiplier;

    [Header("Perlin Noise Settings")]
                   private long    _seed;
    [Range(0, 1)]  private float   _frequency;
    [Range(1, 10)] private int     _octaveCount;
    [Range(0, 1)]  private float   _persistence;
                   private float   _lacunarity;
                   private Vector2 _perlinOffset;
    
    private float Islandify(Vector2 vertexPosition, float elevation, float worldSize, int meshSize, float shapeDistance = 0f, float shapePower = 0f)
    {
        Vector2 worldCenter = Vector2.one * (worldSize * meshSize / 2);

        float dist = Calculate.Distance(vertexPosition, worldCenter, shapePower);
        
        dist = -Calculate.Remap(dist, shapeDistance, worldSize * meshSize / 2, 0, 1) + 1;

        return -Mathf.Cos(Mathf.PI * dist) * elevation;
    }

    private Mesh InitMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        return mesh;
    }

    private List<Vector3> GenerateGroundVertices()
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 vertex = Vector3.zero;
        Vector2 worldPosVertex = Vector2.zero;
        
        float step = (float)_meshSize / (_gridDensity - 1);
        float height = 0f;
        
        for (int x = 0; x < _gridDensity; x++)
        {
            for (int z = 0; z < _gridDensity; z++)
            {
                height = Calculate.PerlinNoise(x + _perlinOffset.x, z + _perlinOffset.y, _frequency, _octaveCount, _persistence, _lacunarity, _seed);

                worldPosVertex.Set(x * step + _perlinOffset.x / (_gridDensity - 1) * _meshSize, 
                                   z * step + _perlinOffset.y / (_gridDensity - 1) * _meshSize);
                
                height += Islandify(worldPosVertex, _groundElevation, _worldSize, _meshSize, _shapeDistance, _shapePower);

                vertex.Set(x * step, height * _heightMultiplier, z * step);
                vertices.Add(vertex);
            }
        }

        return vertices;
    }

    private List<int> GenerateGroundTriangles()
    {
        List<int> triangles = new List<int>();
        int cornerIndex = 0;

        for (int i = 0; i < _gridDensity - 1; i++)
        {
            for (int j = 0; j < _gridDensity - 1; j++)
            {
                cornerIndex = i + j * _gridDensity;

                triangles.Add(cornerIndex);
                triangles.Add(cornerIndex + 1);
                triangles.Add(cornerIndex + 1 + _gridDensity);

                triangles.Add(cornerIndex);
                triangles.Add(cornerIndex + 1 + _gridDensity);
                triangles.Add(cornerIndex + _gridDensity);
            }
        }

        return triangles;
    }

    public void GenerateMesh()
    {
        Mesh mesh = InitMesh();

        List<Vector3> vertices = GenerateGroundVertices();
        List<int> triangles = GenerateGroundTriangles();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
    }

    public void CompleteMeshGeneration()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
    
    public void SetChunkSettings(float worldSize, float shapePower, float shapeDistance, float groundElevation)
    {
        _worldSize       = worldSize;
        _shapePower      = shapePower;
        _shapeDistance   = shapeDistance;
        _groundElevation = groundElevation;
    }
    
    public void SetMeshSettings(MeshSettings meshSettings)
    {
        _gridDensity      = meshSettings.gridDensity;
        _meshSize         = meshSettings.meshSize;
        _heightMultiplier = meshSettings.heightMultiplier;
    }

    public void SetPerlinNoiseSettings(PerlinNoiseSettings perlinNoiseSettings)
    {
        _frequency    = perlinNoiseSettings.frequency;
        _octaveCount  = perlinNoiseSettings.octaves;
        _persistence  = perlinNoiseSettings.persistence;
        _lacunarity   = perlinNoiseSettings.lacunarity;
        _seed         = perlinNoiseSettings.seed;
        _perlinOffset = perlinNoiseSettings.offset;
    }
}
