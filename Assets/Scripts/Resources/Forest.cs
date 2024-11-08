using System.Collections.Generic;
using Calcul;
using UnityEngine;

namespace Resources
{
    public class Forest : MonoBehaviour
    {
        [Header("Forest Settings")]
        [SerializeField] private float _distanceBetweenTrees;
        [SerializeField] private float _minDistanceBetweenTrees;
        [SerializeField] private float _maxDistanceBetweenTrees;
        [SerializeField] private Vector2 _forestSize;
        [SerializeField] private int _rejectionSamples;
        [SerializeField] private LayerMask _groundLayerMask;
        [SerializeField] private List<GameObject> _treesPrefabs;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            GenerateForest();
        }
    
        private void GenerateForest()
        {
            _distanceBetweenTrees = Random.Range(_minDistanceBetweenTrees, _maxDistanceBetweenTrees);
            List<Vector2> spawnPoints = PoissonDiscSampling.GeneratePoints(_distanceBetweenTrees, _forestSize, _rejectionSamples);
            Vector3 resourcesPosition = Vector3.zero;
            Vector3 orientation = Vector3.zero;
            
            foreach (Vector2 point in spawnPoints)
            {
                resourcesPosition.Set(point.x + _transform.position.x, 0, point.y + _transform.position.z);
                resourcesPosition.y = Calculate.GetHeight(resourcesPosition, _groundLayerMask);
                orientation.y = Random.Range(0, 360);
                
                Instantiate(_treesPrefabs[Random.Range(0, _treesPrefabs.Count)], resourcesPosition, Quaternion.Euler(orientation), _transform);
            }
        }
    }
}
