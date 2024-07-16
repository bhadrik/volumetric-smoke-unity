using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorTest))]
public class EditorTestEditor : Editor
{
    EditorTest obj;

    private void OnSceneGUI()
    {
        obj = target as EditorTest;

        if (obj.mesh == null) return;

        int i = 0;
        foreach (var item in obj.mesh.sharedMesh.vertices)
        {
            int index = i;
            Vector3 pos = item;
            if (Handles.Button(item, Quaternion.identity, 0.05f, 0.05f, Handles.SphereHandleCap))
            {
                Debug.Log($"<color=white>Clicked: {index}</color>");

                var lr = obj.GetComponent<LineRenderer>();

                lr.positionCount = lr.positionCount + 1;

                lr.SetPosition(lr.positionCount-1, pos);
            }

            i++;
        }
    }

    
}
