using System.Collections.Generic;
using Calcul;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class DirtLand : MonoBehaviour
{
    [Header("Option")]
    private bool _islandify   = default;
    private bool _shapeIsland = default;
    private bool _makeLayer   = default;
    
    [Header("Chunk Settings")]
                   private float _worldSize       = default;
    [Range(0, 5)]  private float _shapePower      = default;
                   private float _shapeDistance   = default;
    [Range(0, 15)] private float _groundElevation = default;

    [Header("Mesh Settings")]
    [Range(2, 255)] private int     _gridDensity      = default;
    [Range(2, 10)]  private int     _gridNbLayer      = default;
    [Range(2, 100)] private int     _meshSize         = default;
    [Range(1, 15)]  private float   _heightMultiplier = default;

    [Header("Perlin Noise Settings")]
                   private long    _seed           = default;
    [Range(0, 1)]  private float   _frequency      = default;
    [Range(1, 10)] private int     _octaveCount    = default;
    [Range(0, 1)]  private float   _persistence    = default;
                   private float   _lacunarity     = default;
                   private Vector3 _perlinOffset = Vector3.zero;
    
    private float Islandify(Vector2 vertexPosition, float elevation, float worldSize, int meshSize, bool shapeIsland = false, float shapeDistance = 0f, float shapePower = 0f)
    {
        Vector2 worldCenter = Vector2.one * (worldSize * meshSize / 2);

        //float dist = shapeIsland ? ShapeIsland(vertexPosition, worldCenter, shapePower) : Vector2.Distance(vertexPosition, worldCenter);
        float dist = shapeIsland ? Calculate.Distance(vertexPosition, worldCenter, shapePower) : Vector2.Distance(vertexPosition, worldCenter);
        
        dist = -Calculate.Remap(dist, shapeDistance, worldSize * meshSize / 2, 0, 1) + 1;

        return -Mathf.Cos(Mathf.PI * dist) * elevation;
    }

    private float MakeLayer(float height, int nbLayer)
    {
        height *= nbLayer;
        int heightInt = (int)height;
        return (float)heightInt / nbLayer;
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
                height = Calculate.PerlinNoise(x + _perlinOffset.x, z + _perlinOffset.z, _frequency, _octaveCount, _persistence, _lacunarity, _seed);

                worldPosVertex.Set(x * step + _perlinOffset.x / (_gridDensity - 1) * _meshSize, 
                                   z * step + _perlinOffset.z / (_gridDensity - 1) * _meshSize);
                
                height += _islandify ? Islandify(worldPosVertex, _groundElevation, _worldSize, _meshSize, _shapeIsland, _shapeDistance, _shapePower) : 0;
                height = _makeLayer ? MakeLayer(height, _gridNbLayer) : height;

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
    
    public void SetMeshSettings(int gridDensity, int gridNbLayer, int meshSize, float heightMultiplier)
    {
        _gridDensity      = gridDensity;
        _gridNbLayer      = gridNbLayer;
        _meshSize         = meshSize;
        _heightMultiplier = heightMultiplier;
    }

    public void SetPerlinNoiseSettings(float frequency, int octaveCount, float persistence, float lacunarity, long seed, Vector3 perlinOffset)
    {
        _frequency    = frequency;
        _octaveCount  = octaveCount;
        _persistence  = persistence;
        _lacunarity   = lacunarity;
        _seed         = seed;
        _perlinOffset = perlinOffset;
    }

    public void SetOption(bool islandify, bool shapeIsland, bool makeLayer)
    {
        _islandify   = islandify;
        _shapeIsland = shapeIsland;
        _makeLayer   = makeLayer;
    }
}
