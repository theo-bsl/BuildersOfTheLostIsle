using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Resources;
using Random = UnityEngine.Random;

namespace Generation.ResourcesGeneration
{
    public class ResourcesGenerator : MonoBehaviour
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
        private int _nbTrees  = 0;
        private int _nbRocks  = 0;
        private int _nbBushes = 0;
        
        private GameObject _treesHolder;
        private GameObject _rocksHolder;
        private GameObject _bushesHolder;
        
        private readonly Dictionary<ResourceType, List<GameObject>> _resources = new Dictionary<ResourceType, List<GameObject>>();
    
        
        private List<Vector2> _spawnPoints = new List<Vector2>();

        private void Awake()
        {
            _transform = transform;
        }

        public void Init()
        {
            _resourcesZoneSize = TerrainGenerator.Instance.GroundSize;
            
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
            _resources.Add(ResourceType.Trees, new List<GameObject>());
            _resources.Add(ResourceType.Rocks, new List<GameObject>());
            _resources.Add(ResourceType.Bushes, new List<GameObject>());
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
                { ResourceType.Trees,  _pourcentTrees  * _nbMaxResources / 100 },
                { ResourceType.Rocks,  _pourcentRocks  * _nbMaxResources / 100 },
                { ResourceType.Bushes, _pourcentBushes * _nbMaxResources / 100 },
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
            _nbTrees = resourceValues[ResourceType.Trees];
            _nbRocks = resourceValues[ResourceType.Rocks];
            _nbBushes = resourceValues[ResourceType.Bushes];
        }
        
        private void SetRandomOrientation(ref Vector3 orientation)
        {
            orientation.y = Random.Range(0, 360);
        }

        private void GenerateAllResources()
        {
            _spawnPoints = PoissonDiscSampling.GeneratePoints(_distanceBetweenResources, new Vector2(_resourcesZoneSize, _resourcesZoneSize), _rejectionSamples, true);
            _spawnPoints = _spawnPoints.OrderBy(x => Random.value).ToList();
            
            SetNbResources(_spawnPoints.Count);
        
            Vector3 resourcesPosition = Vector3.zero;
            Vector3 orientation = Vector3.zero;
            
            float offsetX = _transform.position.x - _resourcesZoneSize / 2f;
            float offsetZ = _transform.position.z - _resourcesZoneSize / 2f;

            int index = 0;
            
            for (int i = 0; i < _nbTrees; i++, index++) 
            {
                resourcesPosition.Set(_spawnPoints[index].x + offsetX, 0, _spawnPoints[index].y + offsetZ);
                resourcesPosition.y = GetResourceHeight(resourcesPosition);
                SetRandomOrientation(ref orientation);
                
                GenerateForest(resourcesPosition);
                /*GameObject tree = Instantiate(_treesPrefabs[Random.Range(0, _treesPrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), _treesHolder.transform);
                _resources[ResourceType.Trees].Add(tree);*/
            }
            
            for (int i = 0; i < _nbRocks; i++, index++)
            {
                resourcesPosition.Set(_spawnPoints[index].x + offsetX, 0, _spawnPoints[index].y + offsetZ);
                resourcesPosition.y = GetResourceHeight(resourcesPosition);
                SetRandomOrientation(ref orientation);
                
                GameObject rock = Instantiate(_rocksPrefabs[Random.Range(0, _rocksPrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), _rocksHolder.transform);
                _resources[ResourceType.Rocks].Add(rock);
            }

            for (int i = 0; i < _nbBushes; i++, index++)
            {
                resourcesPosition.Set(_spawnPoints[index].x + offsetX, 0, _spawnPoints[index].y + offsetZ);
                resourcesPosition.y = GetResourceHeight(resourcesPosition);
                SetRandomOrientation(ref orientation);
                
                GameObject bush = Instantiate(_bushesPrefabs[Random.Range(0, _bushesPrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), _bushesHolder.transform);
                _resources[ResourceType.Bushes].Add(bush);
            }
        }

        private void GenerateResource(int nbResources, ref int spawnPointsIndex, ResourceType resourceType, ref List<GameObject> resourcePrefabs, ref Vector3 resourcesPosition, ref Vector3 orientation, Transform resourcesHolder)
        {
            for (int i = 0; i < nbResources; i++, spawnPointsIndex++)
            {
                resourcesPosition.Set(_spawnPoints[spawnPointsIndex].x, 0, _spawnPoints[spawnPointsIndex].y);
                resourcesPosition.y = GetResourceHeight(resourcesPosition);
                SetRandomOrientation(ref orientation);
                
                GameObject resource = Instantiate(resourcePrefabs[Random.Range(0, resourcePrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), resourcesHolder);
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
                resourcesPosition.y = GetResourceHeight(resourcesPosition);
                SetRandomOrientation(ref orientation);
                
                _resources[ResourceType.Trees].Add(Instantiate(_treesPrefabs[Random.Range(0, _treesPrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), _treesHolder.transform));
            }
        }
        
        private float GetResourceHeight(Vector3 position)
        {
            if (Physics.Raycast(position + Vector3.up * 100, Vector3.down, out RaycastHit hit, 200, _groundLayerMask.value))
            {
                return hit.point.y;
            }

            return 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(_resourcesZoneSize, 1, _resourcesZoneSize));

            foreach (var point in _spawnPoints)
            {
                Gizmos.DrawSphere(new Vector3(point.x + transform.position.x - _resourcesZoneSize/2f, 0, point.y  + transform.position.z - _resourcesZoneSize/2f), _distanceBetweenResources/10f);
            }
        }
    }
}
