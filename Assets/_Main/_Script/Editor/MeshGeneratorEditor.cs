using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    MeshGenerator obj;
    


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Update Grid"))
        {
            var obj = (MeshGenerator)target;
            obj.UpdateGrid();
        }
        
        if (GUILayout.Button("Update Mesh"))
        {
            var obj = (MeshGenerator)target;
            obj.UpdateMesh();
        }
    }


    private void OnSceneGUI()
    {
        obj = target as MeshGenerator;

        if (!obj.showGizmo) return;

        if (obj.points == null || obj.points.Length <= 0 || obj.points[0, 0, 0].Position == null)
        {
            obj.UpdateGrid();
            return;
        }

        Vector3 previous = obj.points[0, 0, 0].Position;

        int counter = 0;

        for (int x = 0; x < obj.points.GetLength(0); x++)
        {
            for (int y = 0; y < obj.points.GetLength(1); y++)
            {
                for (int z = 0; z < obj.points.GetLength(2); z++)
                {
                    //Gizmos.color = obj.lineColor;
                    //Gizmos.DrawLine(previous, obj.points[x, y, z].Position);
                    previous = obj.points[x, y, z].Position;

                    if (obj.points[x, y, z].IsActive)
                        Handles.color = obj.activePointColor;
                    else
                        Handles.color = obj.inactivePointColor;

                    //Handles.Label(previous, $"     {counter++}:({x},{y},{z})");

                    if (obj.manulySetPoint && Handles.Button(previous, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                        TogglePoint(x, y, z);
                }
            }
        }
    }

    void TogglePoint(int x, int y, int z)
    {
        //Debug.Log($"Toggle: {x} {y} {z}");
        obj.points[x, y, z].IsActive = !obj.points[x, y, z].IsActive;
        obj.UpdateMesh();
    }
}
