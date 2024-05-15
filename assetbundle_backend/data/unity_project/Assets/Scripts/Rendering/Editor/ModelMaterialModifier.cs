using UnityEditor;

namespace Rendering.Editor
{
    [CustomEditor(typeof(ModelMaterialModifier))]
    public class ModelMaterialModifierEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (EditorGUILayout.LinkButton("Wireframe!"))
            {
                (target as ModelMaterialModifier)?.SetWireframe();
            }
            if (EditorGUILayout.LinkButton("Lit!"))
            {
                (target as ModelMaterialModifier)?.SetLit();
            }
        }
    }
}

