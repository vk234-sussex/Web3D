using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RotateForever))]
public class RotateForeverEditor : Editor
{
    private RotateForever targetCasted;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        targetCasted = target as RotateForever;
        if (EditorGUILayout.LinkButton("Enable Rotation"))
        {
            targetCasted.EnableAutoRotate();
        }

        if (EditorGUILayout.LinkButton("Disable Rotation"))
        {
            targetCasted.DisableAutoRotate();
        }

        if (EditorGUILayout.LinkButton("Toggle Rotation"))
        {
            targetCasted.ToggleAutoRotate();
        }
    }
}

