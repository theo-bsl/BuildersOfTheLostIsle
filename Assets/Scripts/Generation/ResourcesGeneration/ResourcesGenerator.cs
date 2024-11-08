using System;
using System.Collections.Generic;
using System.Linq;
using Calcul;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Generation.ResourcesGeneration
{
    public class ResourcesGenerator : Singleton<ResourcesGenerator>
    {
        public bool _refresh;
        
        [Header("Options")]
        [SerializeField] private LayerMask _groundLayerMask = default;
        
        [Header("Global Generation Settings")]
        [SerializeField] private float _distanceBetweenResources = 1;
        [SerializeField] private int _rejectionSamples = 30;
        
        [Header("Forest Generation Settings")]
        [SerializeField] private float _distanceBetweenTrees = 1;
        [SerializeField] private Vector2 _forestSize = new Vector2(5, 5);
        
        [Header("Max Resources")]
        [SerializeField] private int _nbMaxResources = default;
        [SerializeField, Range(1, 100)] private int _pourcentTrees  = default;
        [SerializeField, Range(1, 100)] private int _pourcentRocks  = default;
        [SerializeField, Range(1, 100)] private int _pourcentBushes = default;
    
        [Header("Resources Prefabs")]
        [SerializeField] private List<GameObject> _treesPrefabs;
        [SerializeField] private List<GameObject> _rocksPrefabs;
        [SerializeField] private List<GameObject> _bushesPrefabs;
    
        private Transform _transform;
        private int _resourcesZoneSize;
        private Vector2 _resourcesZoneCenter;
        private int _maxDistance;
        private int _totalResources;
        private int _nbForests  = 0;
        private int _nbRocks  = 0;
        private int _nbBushes = 0;
        
        private GameObject _treesHolder;
        private GameObject _rocksHolder;
        private GameObject _bushesHolder;
        
        private readonly Dictionary<ResourceType, List<GameObject>> _resources = new();
        
        private List<Vector2> _spawnPoints = new List<Vector2>();

        protected override void Awake()
        {
            base.Awake();
            _transform = transform;
        }

        public void Init()
        {
            _resourcesZoneSize = TerrainGenerator.Instance.GroundSize;
            _maxDistance = TerrainGenerator.Instance.ShapeDistance;
            
            InitPosition();
        
            InitHolders();
            InitResourcesList();
            
            GenerateAllResources();
        }

        private void Update()
        {
            if (!_refresh) return;
            _refresh = false;
            
            _nbMaxResources = 150;
        
            ClearResources();
            VerifyResourcesPoucentage();
            SetNbResources(_nbMaxResources);
            GenerateAllResources();
        }

        private void ClearResources()
        {
            foreach (var resources in _resources.Values)
            {
                foreach (var resource in resources)
                {
                    Destroy(resource);
                }
                resources.Clear();
            }
        }

        private void InitPosition()
        {
            int worldSize = TerrainGenerator.Instance.WorldSize;
            _resourcesZoneCenter = new Vector2(worldSize / 2f, worldSize / 2f);
            _transform.position = new Vector3(worldSize / 2f, 0, worldSize / 2f);
        }

        private void InitHolders()
        {
            _treesHolder  = new GameObject("Trees");
            _rocksHolder  = new GameObject("Rocks");
            _bushesHolder = new GameObject("Bushes");
        
            _treesHolder.transform.SetParent(_transform);
            _rocksHolder.transform.SetParent(_transform);
            _bushesHolder.transform.SetParent(_transform);
        }
        
        private void InitResourcesList()
        {
            _resources.Add(ResourceType.Wood, new List<GameObject>());
            _resources.Add(ResourceType.Iron, new List<GameObject>());
            _resources.Add(ResourceType.Food, new List<GameObject>());
        }

        private void VerifyResourcesPoucentage()
        {
            float totalPercentage = _pourcentTrees + _pourcentRocks + _pourcentBushes;

            // Normaliser les pourcentages si la somme dépasse 100%
            if (totalPercentage > 100f)
            {
                float normalizationFactor = 100f / totalPercentage;
                _pourcentTrees  = (int)(_pourcentTrees  * normalizationFactor);
                _pourcentRocks  = (int)(_pourcentRocks  * normalizationFactor);
                _pourcentBushes = (int)(_pourcentBushes * normalizationFactor);
            }
        }

        private int SetNbMaxResources(int maxResources, int maxGlobalResources)
        {
            return Mathf.Min(Random.Range(maxResources / 2, maxResources), maxGlobalResources);
        }
    
        private void SetNbResources(int nbSpawnPoints)
        {
            VerifyResourcesPoucentage();
            
            _nbMaxResources = (int)MathF.Min(_nbMaxResources, nbSpawnPoints);

            // Dictionnaire des limites maximales de chaque type de ressource
            var resourceLimits = new Dictionary<ResourceType, int>
            {
                { ResourceType.Wood,  _pourcentTrees  * _nbMaxResources / 100 },
                { ResourceType.Iron,  _pourcentRocks  * _nbMaxResources / 100 },
                { ResourceType.Food, _pourcentBushes * _nbMaxResources / 100 },
            };

            int nbResourcesAvailable = _nbMaxResources;
            var resourceValues = new Dictionary<ResourceType, int>();

            // Distribution des ressources dans un ordre aléatoire
            foreach (var resource in resourceLimits.Keys.OrderBy(x => Random.value))
            {
                int allocatedResources = SetNbMaxResources(resourceLimits[resource], nbResourcesAvailable);
                resourceValues[resource] = allocatedResources;
                nbResourcesAvailable -= allocatedResources;
            }

            // Assignation des valeurs calculées aux variables correspondantes
            _nbForests = resourceValues[ResourceType.Trees];
            _nbRocks = resourceValues[ResourceType.Rocks];
            _nbBushes = resourceValues[ResourceType.Bushes];
            _totalResources = _nbForests + _nbRocks + _nbBushes;
        }
        
        private void SetRandomOrientation(ref Vector3 orientation)
        {
            orientation.y = Random.Range(0, 360);
        }

        private bool IsInResourceZone(Vector2 position)
        {
            float dist = Calculate.Distance(position, _resourcesZoneCenter, TerrainGenerator.Instance.ShapePower);
            return dist < _maxDistance;
        }

        private void SetResourcePosition(ref Vector3 resourcePosition, Vector2 spawnPoint, Vector2 offset)
        {
            resourcePosition.Set(spawnPoint.x + offset.x, 0, spawnPoint.y + offset.y);
            resourcePosition.y = Calculate.GetHeight(resourcePosition, _groundLayerMask);
        }

        private void GenerateAllResources()
        {
            _spawnPoints = PoissonDiscSampling.GeneratePoints(_distanceBetweenResources, new Vector2(_resourcesZoneSize, _resourcesZoneSize), _rejectionSamples, true);
            _spawnPoints = _spawnPoints.OrderBy(x => Random.value).ToList();
            
            SetNbResources(_spawnPoints.Count);
        
            Vector3 resourcePosition = Vector3.zero;
            Vector3 orientation = Vector3.zero;
            
            Vector2 offset = new Vector2(_transform.position.x - _resourcesZoneSize / 2f, _transform.position.z - _resourcesZoneSize / 2f);

            int index = 0;
            
            GenerateResource(ResourceType.Trees,  _nbForests, _spawnPoints, ref index, offset, _treesPrefabs,  _treesHolder.transform);
            GenerateResource(ResourceType.Rocks,  _nbRocks,   _spawnPoints, ref index, offset, _rocksPrefabs,  _rocksHolder.transform);
            GenerateResource(ResourceType.Bushes, _nbBushes,  _spawnPoints, ref index, offset, _bushesPrefabs, _bushesHolder.transform);
        }

        private void GenerateResource(ResourceType resourceType, int nbResources, List<Vector2> spawnPoints, ref int spawnPointsIndex, Vector2 positionOffset, List<GameObject> resourcePrefabs, Transform resourcesHolder)
        {
            Vector3 resourcePosition = Vector3.zero;
            Vector3 orientation = Vector3.zero;
            
            for (int i = 0; i < nbResources; i++, spawnPointsIndex++)
            {
                SetResourcePosition(ref resourcePosition, _spawnPoints[spawnPointsIndex], positionOffset);
                SetRandomOrientation(ref orientation);
                
                if (!IsInResourceZone(_spawnPoints[spawnPointsIndex] + positionOffset))
                {
                    Debug.Log($"{resourceType} out of zone");
                    Debug.Log($"At {resourcePosition}");
                    
                    bool isResourceSpawned = _resources[resourceType].Count > 0;
                    int remainingResources = nbResources - i;
                    bool enoughSpawnPoints = _totalResources + 1 < _spawnPoints.Count && _totalResources + 1 < _nbMaxResources;
                    
                    if ((!isResourceSpawned && remainingResources == 0) || enoughSpawnPoints) // condition du if pas forcement bonne
                    {
                        i--;
                        continue;
                    }

                    Debug.LogError($"No more {resourceType}");
                }
                
                GameObject resource = Instantiate(resourcePrefabs[Random.Range(0, resourcePrefabs.Count)], resourcePosition, Quaternion.Euler(orientation), resourcesHolder);
                _resources[resourceType].Add(resource);
            }
        }
        
        private void GenerateForest(Vector3 forestPosition)
        {
            List<Vector2> spawnPoints = PoissonDiscSampling.GeneratePoints(_distanceBetweenTrees, _forestSize, _rejectionSamples);
            Vector3 resourcesPosition = Vector3.zero;
            Vector3 orientation = Vector3.zero;
            
            foreach (Vector2 point in spawnPoints)
            {
                resourcesPosition.Set(point.x + forestPosition.x, 0, point.y + forestPosition.z);
                resourcesPosition.y = Calculate.GetHeight(resourcesPosition, _groundLayerMask);
                SetRandomOrientation(ref orientation);
                
                _resources[ResourceType.Wood].Add(Instantiate(_treesPrefabs[Random.Range(0, _treesPrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), _treesHolder.transform));
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(_resourcesZoneSize, 1, _resourcesZoneSize));

            foreach (var point in _spawnPoints)
            {
                Gizmos.DrawSphere(new Vector3(point.x + transform.position.x - _resourcesZoneSize/2f, 0, point.y  + transform.position.z - _resourcesZoneSize/2f), _distanceBetweenResources/10f);
            }
        }
        
        public Dictionary<ResourceType, List<GameObject>> Resources => _resources;
    }
}
