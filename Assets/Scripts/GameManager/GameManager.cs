using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [FormerlySerializedAs("_cameraController")] [SerializeField] private CameraController _player;

        [Header(""), SerializeField] private WorldSettings _worldSettings;
        [Header(""), SerializeField] private MeshSettings _meshSettings;
        [Header(""), SerializeField] private PerlinNoiseSettings _perlinNoiseSettings;
        [Header(""), SerializeField] private ResourcesGenerationSettings _resourcesGenerationSettings;
        
        private WorldGenerator _worldGenerator;
        private Dictionary<ResourceType, List<Transform>> _resources;

        private void Awake()
        {
            _resourcesGenerationSettings.SetWorldSettings(_worldSettings.worldSize * _meshSettings.meshSize, _worldSettings.shapeDistance, _worldSettings.shapePower);
            
            _worldGenerator = new WorldGenerator(transform, _worldSettings, _meshSettings, _perlinNoiseSettings, _resourcesGenerationSettings);
        }

        private void Start()
        {
            SwitchCursor();
            
            StartCoroutine(GenerateWorld());

            _player.transform.position = _worldGenerator.WorldCenter;
        }
        
        private IEnumerator GenerateWorld()
        {
            _worldGenerator.GenerateTerrain();
            yield return new WaitForSeconds(0.1f);
            _resources = _worldGenerator.GenerateResources();
        }

        private void SwitchCursor()
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        public Dictionary<ResourceType, List<Transform>> Resources => _resources;
    }
}
