using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Toggler))]
public class TogglerEditor : GenericEditor<Toggler>
{
    public override void CustomInspector(Toggler behaviour)
    {
        if (EditorGUILayout.LinkButton("Toggle"))
        {
            behaviour.Toggle();
        }
    }
}

