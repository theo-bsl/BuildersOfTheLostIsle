using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        private WorldGenerator _worldGenerator;

        [SerializeField]             private WorldSettings _worldSettings;
        [Header(""), SerializeField] private MeshSettings _meshSettings;
        [Header(""), SerializeField] private PerlinNoiseSettings _perlinNoiseSettings;
        [Header(""), SerializeField] private ResourcesGenerationSettings _resourcesGenerationSettings;
        
        private Dictionary<ResourceType, List<Transform>> _resources;

        private void Awake()
        {
            _resourcesGenerationSettings.SetWorldSettings(_worldSettings.worldSize * _meshSettings.meshSize, _worldSettings.shapeDistance, _worldSettings.shapePower);
            
            _worldGenerator = new WorldGenerator(transform, _worldSettings, _meshSettings, _perlinNoiseSettings, _resourcesGenerationSettings);
        }

        private void Start()
        {
            StartCoroutine(GenerateWorld());
        }
        
        private IEnumerator GenerateWorld()
        {
            _worldGenerator.GenerateTerrain();
            yield return new WaitForSeconds(0.1f);
            _resources = _worldGenerator.GenerateResources();
        }
        
        public Dictionary<ResourceType, List<Transform>> Resources => _resources;
    }
}
