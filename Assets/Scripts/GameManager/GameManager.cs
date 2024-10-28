using Generation.ResourcesGeneration;
using UnityEngine;

namespace GameManager
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private TerrainGenerator _terrainGenerator;
        [SerializeField] private ResourcesGenerator _resourcesGenerator;
        
        private void Start()
        {
            InitGame();
        }

        private void InitGame()
        {
            _terrainGenerator.Init();
            Invoke(nameof(InitResources), 0.1f);
        }
        
        private void InitResources()
        {
            _resourcesGenerator.Init();
        }
    }
}
