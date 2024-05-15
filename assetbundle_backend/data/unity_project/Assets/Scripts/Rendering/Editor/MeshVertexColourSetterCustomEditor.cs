using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshVertexColourSetter))]
public class MeshVertexColourSetterCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (EditorGUILayout.LinkButton("Reset!"))
        {
            (target as MeshVertexColourSetter).Reset();
        }
    }
}

