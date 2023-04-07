using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Point
{
    Vector3 position;
    int randomValue;
    bool isActive;

    public Vector3 Position { 
        get{ return position; }
        set{ position = value; }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }
    public int RandomValue
    {
        get { return randomValue; }
    }

    public Point(Vector3 pos)
    {
        position = pos;
        randomValue = UnityEngine.Random.Range(0, 135);
        isActive = false;
        //isActive = UnityEngine.Random.Range(0, 11) < 5 ? true : false;
    }
}

public class MeshGenerator : MonoBehaviour
{
    [Header("Edior")]
    [SerializeField] public bool showGizmo = true;
    [SerializeField] public bool manulySetPoint = false;

    [SerializeField] public GameObject textMesh;
    [SerializeField] public UnityEngine.Color lineColor;
    [SerializeField] public UnityEngine.Color activePointColor;
    [SerializeField] public UnityEngine.Color inactivePointColor;
    [SerializeField] public UnityEngine.Color textColor;

    [Range(-1, 135)]
    [SerializeField] int visibleSpecteram = 0;
    int previousVisi = 0;


    [SerializeField] public Vector3 meshDimension = Vector3.one * 2;
    [SerializeField] public float cubeSize = 1;

    List<Vector3> vertices;
    List<int> triengles;

    Mesh generatedMesh;
    MeshFilter meshFilter;

    public Point[,,] points;


    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        vertices = new List<Vector3>();
        triengles = new List<int>();
    }

    private void OnValidate()
    {
        UpdateGrid();
        UpdateMesh();

        //bool isGridUpdate = visibleSpecteram != previousVisi || ;

        //if (isGridUpdate)
        //{
        //    previousVisi= visibleSpecteram;

        //    UpdateMesh();
        //}
    }

    public void UpdateGrid()
    {
        points = new Point[(uint)meshDimension.x, (uint)meshDimension.y, (uint)meshDimension.z];

        for (int x = 0; x < meshDimension.x; x++)
        {
            for (int y = 0; y < meshDimension.y; y++)
            {
                for (int z = 0; z < meshDimension.z; z++)
                {
                    points[x, y, z] = new Point(new Vector3(x * cubeSize, y * cubeSize, z * cubeSize));
                }
            }
        }
    }

    public void UpdateMesh()
    {
        // Initialize for non playing mode
        if (!Application.isPlaying)
        {
            meshFilter = GetComponent<MeshFilter>();
            vertices = new List<Vector3>();
            triengles = new List<int>();
        }

        // it
        generatedMesh = GenerateMesh();

        // Set generated mesh
        if (!Application.isPlaying)
            meshFilter.sharedMesh = generatedMesh;
        else
            meshFilter.mesh = generatedMesh;
    }

    Mesh GenerateMesh()
    {
        Mesh localMesh = new Mesh();


        for (int z = 0; z < points.GetLength(2) - 1; z++)
        {
            for (int y = 0; y < points.GetLength(1) - 1; y++)
            {
                for (int x = 0; x < points.GetLength(0) - 1; x++)
                {
                    int cubeConfig = 0;
                    Point[] cubePoints = new Point[8];
                    Vector3[] localVertices = new Vector3[12];

                    // cubePoints represents the current selected cube's 8 points
                    cubePoints[0] = points[x, y, z + 1];
                    cubePoints[1] = points[x, y, z];
                    cubePoints[2] = points[x + 1, y, z];
                    cubePoints[3] = points[x + 1, y, z + 1];
                    cubePoints[4] = points[x, y + 1, z + 1];
                    cubePoints[5] = points[x, y + 1, z];
                    cubePoints[6] = points[x + 1, y + 1, z];
                    cubePoints[7] = points[x + 1, y + 1, z + 1];


                    // Finding mid point for the current selected cube so that
                    // we can create vertex at mid point
                    localVertices[0] = (cubePoints[0].Position + cubePoints[1].Position)/2;
                    localVertices[1] = (cubePoints[1].Position + cubePoints[2].Position)/2;
                    localVertices[2] = (cubePoints[2].Position + cubePoints[3].Position)/2;
                    localVertices[3] = (cubePoints[3].Position + cubePoints[0].Position)/2;
                    localVertices[4] = (cubePoints[4].Position + cubePoints[5].Position)/2;
                    localVertices[5] = (cubePoints[5].Position + cubePoints[6].Position)/2;
                    localVertices[6] = (cubePoints[6].Position + cubePoints[7].Position)/2;
                    localVertices[7] = (cubePoints[7].Position + cubePoints[4].Position)/2;
                    localVertices[8] = (cubePoints[0].Position + cubePoints[4].Position)/2;
                    localVertices[9] = (cubePoints[1].Position + cubePoints[5].Position)/2;
                    localVertices[10] = (cubePoints[2].Position + cubePoints[6].Position)/2;
                    localVertices[11] = (cubePoints[3].Position + cubePoints[7].Position)/2;

                    for (int i = 0; i < 8; i++)
                    {
                        if (!manulySetPoint)
                        {
                            if (cubePoints[i].RandomValue < visibleSpecteram)
                            {
                                cubePoints[i].IsActive = true;
                                cubeConfig |= 1 << i;
                            }
                            else
                                cubePoints[i].IsActive = false;
                        }
                        else if (cubePoints[i].IsActive)
                        {
                            cubeConfig |= 1 << i;
                        }
                    }

                    var verticesList = Tables.triangleTable[cubeConfig];

                    int vertexIndex = vertices.Count-1;

                    for (int i = 0; i < verticesList.Length; i++)
                    {
                        if (verticesList[i] != -1)
                        {
                            // Scan the list which we just fetch from Table and add those points into vertex list
                            vertices.Add(localVertices[verticesList[i]]);
                            triengles.Add(++vertexIndex);
                        }
                    }
                }
            }
        }

        localMesh.vertices = vertices.ToArray();
        localMesh.triangles = triengles.ToArray();
        localMesh.RecalculateBounds();
        localMesh.RecalculateNormals();

        return localMesh;
    }
}
