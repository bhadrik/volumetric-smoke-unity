using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Point
{
    public static float gridSize;
    Vector3 position;
    float randomValue;
    bool isActive;

    public static float min = float.MaxValue;
    public static float max = 0;

    public Vector3 Position
    {
        get { return position; }
        set { position = value; }
    }

    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }
    public float RandomValue
    {
        get { return randomValue; }
        set { randomValue = value; }
    }

    public Point(Vector3 pos)
    {
        position = pos;
        //randomValue = UnityEngine.Random.Range(1, 135);
        randomValue = (Noise.PerlinNoise3D(position.x, position.y, position.z));
        min = randomValue < min ? randomValue : min;
        max = randomValue > max ? randomValue : max;
        //Debug.Log($"Noice: {randomValue} | Min:{min} Max:{max}");
        isActive = UnityEngine.Random.Range(0, 100) > 50;
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
    [SerializeField] float visibleSpectrum = 0;
    [Range(0, 1)]
    [SerializeField] float t = 0.5f;
    float preT;
    float previousVisibleSpectrum = 0;


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

        UpdateGrid();
        UpdateMesh();
    }

    private void OnValidate()
    {
        if (showGizmo)
        {
            Tools.current = Tool.None;
        }
        else
        {
            Tools.current = Tool.Move;
        }

        //Point.gridSize = cubeSize;

        //bool spectrumNotUpdated = previousVisibleSpectrum == visibleSpectrum;
        //bool tUpdate = preT != t;
        //if (spectrumNotUpdated && !tUpdate)
        //{
        //    UpdateGrid();
        //}
        //else
        //{
        //    previousVisibleSpectrum = visibleSpectrum;
        //}

        //UpdateMesh();
        //CreateMesh();
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
                    if (x == 0 || y == 0 || z == 0 || x == meshDimension.x - 1 || y == meshDimension.y - 1 || z == meshDimension.z - 1)
                    {
                        points[x, y, z].RandomValue = Point.max;
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
        //if (!Application.isPlaying)
        //    meshFilter.sharedMesh = generatedMesh;
        //else
        //    meshFilter.mesh = generatedMesh;

        meshFilter.sharedMesh = generatedMesh;
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
                    cubePoints.Add(points[x, y, z]);
                    cubePoints.Add(points[x + 1, y, z]);
                    cubePoints.Add(points[x, y + 1, z]);
                    cubePoints.Add(points[x + 1, y + 1, z]);
                    cubePoints.Add(points[x, y, z + 1]);
                    cubePoints.Add(points[x + 1, y, z + 1]);
                    cubePoints.Add(points[x, y + 1, z + 1]);
                    cubePoints.Add(points[x + 1, y + 1, z + 1]);

                    // Pattern
                    //(0, 0, 0)
                    //(1, 0, 0)
                    //(0, 1, 0)
                    //(1, 1, 0)
                    //(0, 0, 1)
                    //(1, 0, 1)
                    //(0, 1, 1)
                    //(1, 1, 1)

                    //for (int i = 0; i < cubePoints.Count; i++)
                    //{
                    //    Debug.Log($"<color=white>[Vertex]:{cubePoints[i].Position}</color>", gameObject);
                    //}


                    // Using learp to find vertex not the center method
                    //localVertices[0] = Lerp(0, 1);
                    //localVertices[1] = Lerp(1, 2);
                    //localVertices[2] = Lerp(2, 3);
                    //localVertices[3] = Lerp(3, 0);
                    //localVertices[4] = Lerp(4, 5);
                    //localVertices[5] = Lerp(5, 6);
                    //localVertices[6] = Lerp(6, 7);
                    //localVertices[7] = Lerp(7, 4);
                    //localVertices[8] = Lerp(0, 4);
                    //localVertices[9] = Lerp(1, 5);
                    //localVertices[10] = Lerp(2, 6);
                    //localVertices[11] = Lerp(3, 7);

                    //Finding mid point for the current selected cube so that
                    //we can create vertex at mid point
                    localVertices[0] = (cubePoints[Tables.EdgeVertices[0, 0]].Position + cubePoints[Tables.EdgeVertices[0, 1]].Position) / 2;
                    localVertices[1] = (cubePoints[Tables.EdgeVertices[1, 0]].Position + cubePoints[Tables.EdgeVertices[1, 1]].Position) / 2;
                    localVertices[2] = (cubePoints[Tables.EdgeVertices[2, 0]].Position + cubePoints[Tables.EdgeVertices[2, 1]].Position) / 2;
                    localVertices[3] = (cubePoints[Tables.EdgeVertices[3, 0]].Position + cubePoints[Tables.EdgeVertices[3, 1]].Position) / 2;
                    localVertices[4] = (cubePoints[Tables.EdgeVertices[4, 0]].Position + cubePoints[Tables.EdgeVertices[4, 1]].Position) / 2;
                    localVertices[5] = (cubePoints[Tables.EdgeVertices[5, 0]].Position + cubePoints[Tables.EdgeVertices[5, 1]].Position) / 2;
                    localVertices[6] = (cubePoints[Tables.EdgeVertices[6, 0]].Position + cubePoints[Tables.EdgeVertices[6, 1]].Position) / 2;
                    localVertices[7] = (cubePoints[Tables.EdgeVertices[7, 0]].Position + cubePoints[Tables.EdgeVertices[7, 1]].Position) / 2;
                    localVertices[8] = (cubePoints[Tables.EdgeVertices[8, 0]].Position + cubePoints[Tables.EdgeVertices[8, 1]].Position) / 2;
                    localVertices[9] = (cubePoints[Tables.EdgeVertices[9, 0]].Position + cubePoints[Tables.EdgeVertices[9, 1]].Position) / 2;
                    localVertices[10] = (cubePoints[Tables.EdgeVertices[10, 0]].Position + cubePoints[Tables.EdgeVertices[10, 1]].Position) / 2;
                    localVertices[11] = (cubePoints[Tables.EdgeVertices[11, 0]].Position + cubePoints[Tables.EdgeVertices[11, 1]].Position) / 2;

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

                    var verticesList = Tables.Triangles[cubeConfig];
                    //Debug.Log($"cubeConfig: {cubeConfig}");

                    int vertexIndex = vertices.Count - 1;

                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("{");
                    //for (int i = verticesList.Length-1; i >= 0; i--)
                    for (int i = 0; i < verticesList.Count; i++)
                    {
                        if (verticesList[i] == -1) break;

                        //sb.Append($"{verticesList[i]}, ");

                        // Scan the list which we just fetch from Table and add those points into vertex list
                        vertices.Add(localVertices[verticesList[i]]);
                        triengles.Add(++vertexIndex);
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
        float valA = cubePoints[a].RandomValue;
        float valB = cubePoints[b].RandomValue;

        //float diff = (valA - valB)/2;

        //t = (Point.max - Point.min) + diff;

        //t = Remap(t, Point.min, Point.max, 0, 1);

        //Debug.Log($"t={t}");


        //Vector3 pointA = cubePoints[a].Position;
        //Vector3 pointB = cubePoints[b].Position;

        Vector3 pointA = valA < valB ? cubePoints[a].Position : cubePoints[b].Position;
        Vector3 pointB = valA > valB ? cubePoints[a].Position : cubePoints[b].Position;

        return Vector3.Lerp(pointA, pointB, t);
    }

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


    [Header("Seperated--------")]
    [SerializeField] List<bool> selected = new List<bool>(new bool[] { true, true, true, true, true, true, true, true });


    [ContextMenu("Create")]
    private void CreateMesh()
    {
        List<Vector3> finalVertices = new List<Vector3>();
        List<int> finalTrieses = new List<int>();

        var cube = new List<Point>();

        cube.Add(new Point(new Vector3(0, 0, 0)));
        cube.Add(new Point(new Vector3(1, 0, 0)));
        cube.Add(new Point(new Vector3(0, 1, 0)));
        cube.Add(new Point(new Vector3(1, 1, 0)));
        cube.Add(new Point(new Vector3(0, 0, 1)));
        cube.Add(new Point(new Vector3(1, 0, 1)));
        cube.Add(new Point(new Vector3(0, 1, 1)));
        cube.Add(new Point(new Vector3(1, 1, 1)));

        var localVertices = new Vector3[12];

        localVertices[0] = (cube[Tables.EdgeVertices[0, 0]].Position + cube[Tables.EdgeVertices[0, 1]].Position) / 2;
        localVertices[1] = (cube[Tables.EdgeVertices[1, 0]].Position + cube[Tables.EdgeVertices[1, 1]].Position) / 2;
        localVertices[2] = (cube[Tables.EdgeVertices[2, 0]].Position + cube[Tables.EdgeVertices[2, 1]].Position) / 2;
        localVertices[3] = (cube[Tables.EdgeVertices[3, 0]].Position + cube[Tables.EdgeVertices[3, 1]].Position) / 2;
        localVertices[4] = (cube[Tables.EdgeVertices[4, 0]].Position + cube[Tables.EdgeVertices[4, 1]].Position) / 2;
        localVertices[5] = (cube[Tables.EdgeVertices[5, 0]].Position + cube[Tables.EdgeVertices[5, 1]].Position) / 2;
        localVertices[6] = (cube[Tables.EdgeVertices[6, 0]].Position + cube[Tables.EdgeVertices[6, 1]].Position) / 2;
        localVertices[7] = (cube[Tables.EdgeVertices[7, 0]].Position + cube[Tables.EdgeVertices[7, 1]].Position) / 2;
        localVertices[8] = (cube[Tables.EdgeVertices[8, 0]].Position + cube[Tables.EdgeVertices[8, 1]].Position) / 2;
        localVertices[9] = (cube[Tables.EdgeVertices[9, 0]].Position + cube[Tables.EdgeVertices[9, 1]].Position) / 2;
        localVertices[10] = (cube[Tables.EdgeVertices[10, 0]].Position + cube[Tables.EdgeVertices[10, 1]].Position) / 2;
        localVertices[11] = (cube[Tables.EdgeVertices[11, 0]].Position + cube[Tables.EdgeVertices[11, 1]].Position) / 2;

        int cubeConfig = 0;

        for (int i = 0; i < 8; i++)
        {
            if (selected[i])
            {
                cubeConfig |= 1 << i;
            }
        }

        Debug.Log($"<color=white>Cube config: {cubeConfig}</color>", gameObject);

        var verticesList = Tables.Triangles[cubeConfig];

        //int vertexIndex = finalVertices.Count - 1;
        int vertextIndex = 0;
        for (int i = 0; i < verticesList.Count; i++)
        {
            if (verticesList[i] == -1) break;

            Debug.Log($"<color=white>Vertex[{verticesList[i]}]: {localVertices[verticesList[i]]}</color>", gameObject);
            finalVertices.Add(localVertices[verticesList[i]]);
            finalTrieses.Add(vertextIndex++);
        }

        var localMesh = new Mesh();
        localMesh.vertices = finalVertices.ToArray();
        localMesh.triangles = finalTrieses.ToArray();
        localMesh.RecalculateBounds();
        localMesh.RecalculateNormals();

        GetComponent<MeshFilter>().sharedMesh = localMesh;
    }
}
