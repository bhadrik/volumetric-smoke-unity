using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
public class MeshGeneratorEditor : Editor
{
    MeshGenerator obj;

    int counter;

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

        //int counter = 0;

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
                        Handles.color = Color.Lerp(obj.inactivePointColor, obj.activePointColor, Remap(obj.points[x, y, z].RandomValue, Point.min, Point.max, 0, 1));
                    //Handles.color = obj.activePointColor;
                    else
                        Handles.color = obj.inactivePointColor;

                    //Handles.Label(previous, $"     :({x},{y},{z})");
                    counter++;

                    if (obj.manulySetPoint)
                    {
                        if (Handles.Button(previous, Quaternion.identity, 0.05f, 0.05f, Handles.SphereHandleCap))
                            TogglePoint(x, y, z);
                    }
                    else
                    {
                        Handles.SphereHandleCap(0, previous, Quaternion.identity, 0.05f, EventType.Repaint);
                    }
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

    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

}
