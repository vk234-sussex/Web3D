using System;
using UnityEngine;

namespace Rendering.Camera
{
    [Serializable]
    public class CameraMode
    {
        public string modeName = "";
        public Vector3 position = Vector3.zero;
        public Vector3 eulerAngles = Vector3.zero;
        public bool orthographic = false;
        [Header("In Ortho mode, this is size")]
        public float fov = 60;
    }
}