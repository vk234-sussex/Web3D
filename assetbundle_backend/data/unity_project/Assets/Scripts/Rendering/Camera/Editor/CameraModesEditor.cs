using UnityEngine;
using System.Collections;
using Rendering.Camera;
using UnityEditor;

namespace Rendering.Camera
{
    [CustomEditor(typeof(CameraModes))]
    public class CameraModedEditor : GenericEditor<CameraModes>
    {

        public override void CustomInspector(CameraModes behaviour)
        {
            for (var i = 0; i < behaviour.Modes.Count; i++)
            {
                if(EditorGUILayout.LinkButton(behaviour.Modes[i].modeName))
                {
                    if (Application.isPlaying) behaviour.ApplyMode(behaviour.Modes[i].modeName);
                    return;
                }
            }
        }
    }
}
