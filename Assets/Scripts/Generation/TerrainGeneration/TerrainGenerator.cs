using System.Collections.Generic;
using Calcul;
using UnityEngine;

public class TerrainGenerator : Singleton<TerrainGenerator> 
{
    [Header("World Options")]
    [SerializeField] private bool _islandify   = default;
    [SerializeField] private bool _shapeIsland = default;
    [SerializeField] private bool _makeLayer   = default;
    [SerializeField] private bool _makeWater   = default;
    
    [Header("World Settings")]
    [SerializeField]               private long  _seed            = default;
    [SerializeField, Range(2, 10)] private int   _worldSize       = default;
    [SerializeField, Range(0, 5)]  private float _shapePower      = default;
    [SerializeField]               private int   _shapeDistance   = default;
    [SerializeField]               private float _groundElevation = default;
    
    [Header("")][Header("Mesh Settings")]
    [SerializeField, Range(2, 255)] private int   _gridDensity      = default;
    [SerializeField, Range(2, 15)]  private int   _gridNbLayer      = default;
    [SerializeField, Range(2, 100)] private int   _meshSize         = default;
    [SerializeField, Range(1, 20)]  private float _heightMultiplier = default;

    [Header("")][Header("Perlin Noise Settings")]
    [SerializeField, Range(0, 0.5f)]  private float _frequency   = default;
    [SerializeField, Range(1, 10)]    private int   _octaveCount = default;
    [SerializeField, Range(0, 1)]     private float _persistence = default;
    [SerializeField]                  private float _lacunarity  = default;

    [Header("")][Header("Water Settings")]
    [SerializeField, Range(0, 10)] private float _waterLevel = default;

    [Header("")][Header("Option")]
    [SerializeField] private bool _refresh = default;
    [SerializeField] private bool _alwaysRefresh = default;
    
    [Header("")][Header("Lands Prefab")]
    [SerializeField] private GameObject _dirtLandPrefab  = default;
    [SerializeField] private GameObject _waterLandPrefab = default;

    private Transform _transform;
    private GameObject _dirtLandsHolder;
    private GameObject _waterLandsHolder;
    private readonly List<DirtLand> _dirtLands  = new List<DirtLand>();
    private readonly List<WaterLand> _waterLands = new List<WaterLand>();

    private new void Awake()
    {
        base.Awake();
        
        _transform = transform;
    }

    public void Init()
    {
        InitHolder();
        InitSeed();
        
        GenerateTerrain();
        CompleteTerrainGeneration();
    }

    private void Update()
    {
        if (!_refresh && !_alwaysRefresh) return;
        _refresh = false;

        ClearDirtLands();
        ClearWaterLands();

        GenerateTerrain();
        CompleteTerrainGeneration();
    }
    
    private void InitHolder()
    {
        _dirtLandsHolder = new GameObject("DirtLands");
        _dirtLandsHolder.transform.SetParent(_transform);
        
        _waterLandsHolder = new GameObject("WaterLands");
        _waterLandsHolder.transform.SetParent(_transform);
    }

    private void InitSeed()
    {
        //_seed = DateTime.Now.Ticks;
        _seed = PlayerPrefs.GetInt("seed", 0);
    }
    
    private GameObject InstantiateLand(GameObject landPrefab, Transform parent, Vector3 position, string chunkName)
    {
        GameObject land = Instantiate(landPrefab, parent);
        land.name = chunkName;
        land.transform.position = position;

        return land;
    }

    private void GenerateTerrain()
    {
        ClearDirtLands();
        ClearWaterLands();

        Vector3 chunkPosition = Vector3.zero;
        Vector2 chunkPivotPosition = Vector2.zero;
        Vector2 chunkOppositePivotPosition = Vector2.zero;
        Vector3 perlinOffset = Vector3.zero;
        Vector2 worldCenter = new Vector2(_worldSize * _meshSize / 2, _worldSize * _meshSize / 2);

        for (int x = 0; x < _worldSize; x++)
        {
            for (int z = 0; z < _worldSize; z++)
            {
                chunkPosition.Set(x * _meshSize, 0, z * _meshSize);
                chunkPivotPosition.Set(x * _meshSize, z * _meshSize);
                chunkOppositePivotPosition.Set(x * _meshSize + _meshSize, z * _meshSize + _meshSize);
                perlinOffset.Set(x * (_gridDensity - 1), 0, z * (_gridDensity - 1));

                DirtLand dirtLand = InstantiateLand(_dirtLandPrefab, _dirtLandsHolder.transform, chunkPosition, $"DirtLand{x}{z}").GetComponent<DirtLand>();

                dirtLand.SetChunkSettings(_worldSize, _shapePower, _shapeDistance, _groundElevation);
                dirtLand.SetMeshSettings(_gridDensity, _gridNbLayer, _meshSize, _heightMultiplier);
                dirtLand.SetPerlinNoiseSettings(_frequency, _octaveCount, _persistence, _lacunarity, _seed, perlinOffset);
                dirtLand.SetOption(_islandify, _shapeIsland, _makeLayer);
                dirtLand.GenerateMesh();

                _dirtLands.Add(dirtLand);
                
                float distanceToPivot = Calculate.Distance(chunkPivotPosition, worldCenter, _shapePower);
                float distanceToOpposite = Calculate.Distance(chunkOppositePivotPosition, worldCenter, _shapePower);
                
                bool makeWater = _makeWater && (distanceToPivot > _shapeDistance || distanceToOpposite > _shapeDistance);

                if (!makeWater) continue;
                WaterLand waterLand = InstantiateLand(_waterLandPrefab, _waterLandsHolder.transform, chunkPosition, $"WaterLand{x}{z}").GetComponent<WaterLand>();
                waterLand.SetChunkSettings(_gridDensity, _meshSize);
                waterLand.SetWaterSettings(_waterLevel);
                waterLand.GenerateMesh();
                _waterLands.Add(waterLand);
            }
        }
    }

    private void CompleteTerrainGeneration()
    {
        foreach (var dirtLand in _dirtLands)
        {
            dirtLand.CompleteMeshGeneration();
        }
    }

    private void ClearDirtLands()
    {
        for (int i = 0; i < _dirtLands.Count; i++)
        {
            Destroy(_dirtLands[i].gameObject);
        }
        _dirtLands.Clear();
    }

    private void ClearWaterLands()
    {
        for (int i = 0; i < _waterLands.Count; i++)
        {
            Destroy(_waterLands[i].gameObject);
        }
        _waterLands.Clear();
    }
    
    public int GroundSize => _worldSize * _meshSize - _shapeDistance;
    public int WorldSize => _worldSize * _meshSize;
}
