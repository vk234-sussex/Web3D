using UnityEngine;
using UnityEditor;

public abstract class GenericEditor<T> : Editor where T : MonoBehaviour
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CustomInspector(target as T);
    }

    public abstract void CustomInspector(T behaviour);
}

