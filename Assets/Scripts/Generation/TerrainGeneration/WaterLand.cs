using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class WaterLand : MonoBehaviour
{
    [Range(2, 255)] private int _gridDensity;
    [Range(2, 100)] private float _meshSize;

    private float _waterLevel;

    private Mesh InitMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

        return mesh;
    }

    private List<Vector3> GenerateWaterVertices()
    {
        List<Vector3> vertices = new List<Vector3>();
        Vector3 vertex = Vector3.zero;

        float step = _meshSize / (_gridDensity - 1);

        for (int x = 0; x < _gridDensity; x++)
        {
            for (int z  = 0; z < _gridDensity; z++)
            {
                vertex.Set(x * step, _waterLevel, z * step);
                vertices.Add(vertex);
            }
        }

        return vertices;
    }

    private List<int> GenerateWaterTriangles()
    {
        List<int> triangles = new List<int>();

        int vertexIndex = 0;

        for (int x = 0; x < _gridDensity - 1; x++)
        {
            for (int z = 0; z < _gridDensity - 1; z++)
            {
                vertexIndex = x + z * _gridDensity;

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 1 + _gridDensity);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1 + _gridDensity);
                triangles.Add(vertexIndex + _gridDensity);
            }
        }

        return triangles;
    }

    public void GenerateMesh()
    {
        Mesh mesh = InitMesh();

        List<Vector3> vertices = GenerateWaterVertices();
        List<int> triangles = GenerateWaterTriangles();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    public void SetChunkSettings(int gridSize, float meshSize)
    {
        _gridDensity = gridSize;
        _meshSize = meshSize;
    }
    
    public void SetWaterSettings(float waterLevel)
    {
        _waterLevel = waterLevel;
    }
}
