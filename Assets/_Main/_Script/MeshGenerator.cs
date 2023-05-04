using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

[Serializable]
public class Point
{
    public static float gridSize;
    Vector3 position;
    int randomValue;
    bool isActive;

    static float min = 500;
    static float max = 0;

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
        set { randomValue = value; }
    }

    public Point(Vector3 pos)
    {
        position = pos;
        //randomValue = UnityEngine.Random.Range(1, 135);
        randomValue = (int)(Noise.PerlinNoise3D(position.x, position.y, position.z));
        if(randomValue < min) min = randomValue;
        if(randomValue > max) max = randomValue;
        //Debug.Log($"Noice: {randomValue} | Min:{min} Max:{max}");
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

    [SerializeField] int seed;
    [Range(5, 60)]
    [SerializeField] int visibleSpectrum = 0;
    [Range(0, 1)]
    [SerializeField] float t = 0.5f;
    float preT;
    int previousVisibleSpectrum = 0;


    [SerializeField] public Vector3 meshDimension = Vector3.one * 2;
    [SerializeField] public float cubeSize = 1;

    List<Vector3> vertices;
    List<int> triengles;

    Mesh generatedMesh;
    MeshFilter meshFilter;
    List<Point> cubePoints;

    public Point[,,] points;


    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        vertices = new List<Vector3>();
        triengles = new List<int>();
    }

    private void OnValidate()
    {
        Point.gridSize = cubeSize;

        bool spectrumNotUpdated = previousVisibleSpectrum == visibleSpectrum;
        bool tUpdate = preT != t;
        if (spectrumNotUpdated && !tUpdate)
        {
            UpdateGrid();
        }
        else
        {
            previousVisibleSpectrum = visibleSpectrum;
        }

        UpdateMesh();
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
                    if(x == 0 || y ==0 || z == 0 || x == meshDimension.x-1 || y == meshDimension.y-1 || z == meshDimension.z-1)
                    {
                        points[x, y, z].RandomValue = 27;
                    }
                }
            }
        }

        preT = t;
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
        cubePoints = new List<Point>();

        for (int z = 0; z < points.GetLength(2) - 1; z++)
        {
            for (int y = 0; y < points.GetLength(1) - 1; y++)
            {
                for (int x = 0; x < points.GetLength(0) - 1; x++)
                {
                    int cubeConfig = 0;

                    cubePoints.Clear();

                    Vector3[] localVertices = new Vector3[12];

                    // cubePoints represents the current selected cube's 8 points
                    cubePoints.Add(points[x, y, z + 1]);
                    cubePoints.Add(points[x, y, z]);
                    cubePoints.Add(points[x + 1, y, z]);
                    cubePoints.Add(points[x + 1, y, z + 1]);
                    cubePoints.Add(points[x, y + 1, z + 1]);
                    cubePoints.Add(points[x, y + 1, z]);
                    cubePoints.Add(points[x + 1, y + 1, z]);
                    cubePoints.Add(points[x + 1, y + 1, z + 1]);

                    /*
                    * Using learp to find vertex not the center method
                    */
                    localVertices[0] = Lerp(0, 1);
                    localVertices[1] = Lerp(1, 2);
                    localVertices[2] = Lerp(2, 3);
                    localVertices[3] = Lerp(3, 0);
                    localVertices[4] = Lerp(4, 5);
                    localVertices[5] = Lerp(5, 6);
                    localVertices[6] = Lerp(6, 7);
                    localVertices[7] = Lerp(7, 4);
                    localVertices[8] = Lerp(0, 4);
                    localVertices[9] = Lerp(1, 5);
                    localVertices[10] = Lerp(2, 6);
                    localVertices[11] = Lerp(3, 7);
                    
                    /*  
                     *  Finding mid point for the current selected cube so that
                     *  we can create vertex at mid point
                     */
                    //localVertices[0] = (cubePoints[0].Position + cubePoints[1].Position)/2;
                    //localVertices[1] = (cubePoints[1].Position + cubePoints[2].Position)/2;
                    //localVertices[2] = (cubePoints[2].Position + cubePoints[3].Position)/2;
                    //localVertices[3] = (cubePoints[3].Position + cubePoints[0].Position)/2;
                    //localVertices[4] = (cubePoints[4].Position + cubePoints[5].Position)/2;
                    //localVertices[5] = (cubePoints[5].Position + cubePoints[6].Position)/2;
                    //localVertices[6] = (cubePoints[6].Position + cubePoints[7].Position)/2;
                    //localVertices[7] = (cubePoints[7].Position + cubePoints[4].Position)/2;
                    //localVertices[8] = (cubePoints[0].Position + cubePoints[4].Position)/2;
                    //localVertices[9] = (cubePoints[1].Position + cubePoints[5].Position)/2;
                    //localVertices[10] = (cubePoints[2].Position + cubePoints[6].Position)/2;
                    //localVertices[11] = (cubePoints[3].Position + cubePoints[7].Position)/2;

                    for (int i = 0; i < 8; i++)
                    {
                        if (!manulySetPoint)
                        {
                            if (cubePoints[i].RandomValue < visibleSpectrum)
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
                    //Debug.Log($"cubeConfig: {cubeConfig}");

                    int vertexIndex = vertices.Count-1;

                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("{");
                    //for (int i = verticesList.Length-1; i >= 0; i--)
                    for (int i = 0; i < verticesList.Length; i++)
                    {
                        //sb.Append($"{verticesList[i]}, ");
                        if (verticesList[i] != -1)
                        {
                            // Scan the list which we just fetch from Table and add those points into vertex list
                            vertices.Add(localVertices[verticesList[i]]);
                            triengles.Add(++vertexIndex);
                        }
                    }
                    //sb.Append("}");
                    //Debug.Log(sb);
                }
            }
        }

        localMesh.vertices = vertices.ToArray();
        localMesh.triangles = triengles.ToArray();
        localMesh.RecalculateBounds();
        localMesh.RecalculateNormals();

        return localMesh;
    }

    Vector3 Lerp(int a, int b)
    {
        int valA = cubePoints[a].RandomValue;
        int valB = cubePoints[b].RandomValue;

        t = (float)valA / (float)valB;
        t = Remap(t, 0.08333f, 12, 0, 1);

        Debug.Log($" = " + t);
        return Vector3.Lerp(cubePoints[a].Position, cubePoints[b].Position, t);
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
