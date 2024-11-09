using System;
using System.Collections.Generic;
using System.Linq;
using Calcul;
using Resources;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Generation.ResourcesGeneration
{
    public class ResourcesGenerator
    {
        private int _totalResources;
        private int _nbForests  = 0;
        private int _nbRocks  = 0;
        private int _nbBushes = 0;
        
        private GameObject _treesHolder;
        private GameObject _rocksHolder;
        private GameObject _bushesHolder;
        
        public ResourcesGenerator(Transform holdersParent)
        {
            InitHolders(holdersParent);
        }

        private void InitHolders(Transform holdersParent)
        {
            _treesHolder  = new GameObject("Trees");
            _rocksHolder  = new GameObject("Rocks");
            _bushesHolder = new GameObject("Bushes");
        
            _treesHolder.transform.SetParent(holdersParent);
            _rocksHolder.transform.SetParent(holdersParent);
            _bushesHolder.transform.SetParent(holdersParent);
        }

        private (int, int, int) VerifyResourcesPoucentage(float pourcentTrees, float pourcentRocks, float pourcentBushes)
        {
            float totalPercentage = pourcentTrees + pourcentRocks + pourcentBushes;

            // Normaliser les pourcentages si la somme dépasse 100%
            if (totalPercentage > 100f)
            {
                float normalizationFactor = 100f / totalPercentage;
                pourcentTrees  = (int)(pourcentTrees  * normalizationFactor);
                pourcentRocks  = (int)(pourcentRocks  * normalizationFactor);
                pourcentBushes = (int)(pourcentBushes * normalizationFactor);
            }
            
            return ((int)pourcentTrees, (int)pourcentRocks, (int)pourcentBushes);
        }

        private int SetNbMaxResources(int maxResources, int maxGlobalResources)
        {
            return Mathf.Min(Random.Range(maxResources / 2, maxResources), maxGlobalResources);
        }
    
        private void SetNbResources(int nbSpawnPoints, ResourcesGenerationSettings settings)
        {
            (_nbForests, _nbRocks, _nbBushes) = VerifyResourcesPoucentage(settings.pourcentForests, settings.pourcentRocks, settings.pourcentBushes);
            
            settings.nbMaxResources = (int)MathF.Min(settings.nbMaxResources, nbSpawnPoints);

            // Dictionnaire des limites maximales de chaque type de ressource
            var resourceLimits = new Dictionary<ResourceType, int>
            {
                { ResourceType.Wood, settings.pourcentForests  * settings.nbMaxResources / 100 },
                { ResourceType.Iron, settings.pourcentRocks  * settings.nbMaxResources / 100 },
                { ResourceType.Food, settings.pourcentBushes * settings.nbMaxResources / 100 },
            };

            int nbResourcesAvailable = settings.nbMaxResources;
            var resourceValues = new Dictionary<ResourceType, int>();

            // Distribution des ressources dans un ordre aléatoire
            foreach (var resource in resourceLimits.Keys.OrderBy(x => Random.value))
            {
                int allocatedResources = SetNbMaxResources(resourceLimits[resource], nbResourcesAvailable);
                resourceValues[resource] = allocatedResources;
                nbResourcesAvailable -= allocatedResources;
            }

            // Assignation des valeurs calculées aux variables correspondantes
            _nbForests = resourceValues[ResourceType.Wood];
            _nbRocks = resourceValues[ResourceType.Iron];
            _nbBushes = resourceValues[ResourceType.Food];
            _totalResources = _nbForests + _nbRocks + _nbBushes;
        }
        
        private void SetRandomOrientation(ref Vector3 orientation)
        {
            orientation.y = Random.Range(0, 360);
        }

        private bool IsInResourceZone(Vector2 position, Vector2 center, float shapeDistance, float shapePower)
        {
            float dist = Calculate.Distance(position, center, shapePower);
            return dist < shapeDistance;
        }

        private void SetResourcePosition(ref Vector3 resourcePosition, Vector2 spawnPoint, Vector2 offset, LayerMask groundLayerMask)
        {
            resourcePosition.Set(spawnPoint.x + offset.x, 0, spawnPoint.y + offset.y);
            resourcePosition.y = Calculate.GetHeight(resourcePosition, groundLayerMask);
        }

        public Dictionary<ResourceType, List<Transform>> GenerateAllResources(ResourcesGenerationSettings settings)
        {
            Dictionary<ResourceType, List<Transform>> resources = new();
            
            List<Vector2> spawnPoints = PoissonDiscSampling.GeneratePoints(settings.distanceBetweenResources, new Vector2(settings.resourcesZoneSize, settings.resourcesZoneSize), settings.rejectionSamples, true);
            spawnPoints = spawnPoints.OrderBy(x => Random.value).ToList();
            
            SetNbResources(spawnPoints.Count, settings);
            
            Vector2 offset = new Vector2(settings.offset, settings.offset);

            int index = 0;
            
            resources[ResourceType.Wood] = GenerateResource(ResourceType.Wood, _nbForests, spawnPoints, ref index, offset, _treesHolder.transform,  settings);
            resources[ResourceType.Iron] = GenerateResource(ResourceType.Iron, _nbRocks,   spawnPoints, ref index, offset, _rocksHolder.transform,  settings);
            resources[ResourceType.Food] = GenerateResource(ResourceType.Food, _nbBushes,  spawnPoints, ref index, offset, _bushesHolder.transform, settings);
            
            return resources;
        }

        private List<Transform> GenerateResource(ResourceType resourceType, int nbResources, List<Vector2> spawnPoints, ref int spawnPointsIndex, Vector2 positionOffset, Transform resourcesHolder, ResourcesGenerationSettings settings)
        {
            List<Transform> resources = new List<Transform>();
            Vector2 worldCenter = new Vector2(settings.resourcesZoneSize / 2f + settings.offset, settings.resourcesZoneSize / 2f + settings.offset);
            Vector3 resourcePosition = Vector3.zero;
            Vector3 orientation = Vector3.zero;
            
            for (int i = 0; i < nbResources; i++, spawnPointsIndex++)
            {
                SetResourcePosition(ref resourcePosition, spawnPoints[spawnPointsIndex], positionOffset, settings.groundLayerMask);
                SetRandomOrientation(ref orientation);
                
                if (!IsInResourceZone(spawnPoints[spawnPointsIndex] + positionOffset, worldCenter, settings.shapeDistance, settings.shapePower))
                {
                    bool isResourceSpawned = resources.Count > 0;
                    int remainingResources = nbResources - i;
                    bool enoughSpawnPoints = _totalResources + 1 < spawnPoints.Count && _totalResources + 1 < settings.nbMaxResources;
                    
                    if ((!isResourceSpawned && remainingResources == 0) || enoughSpawnPoints) // condition du if pas forcement bonne
                    {
                        i--;
                        continue;
                    }
                }
                
                GameObject resource = Object.Instantiate(settings.GetResourcePrefab(resourceType), resourcePosition, Quaternion.Euler(orientation), resourcesHolder);

                if (resource.TryGetComponent<Forest>(out var forest))
                {
                    forest.SetSettings(settings);
                }
                    
                resources.Add(resource.transform);
            }
            
            return resources;
        }
    }
}
