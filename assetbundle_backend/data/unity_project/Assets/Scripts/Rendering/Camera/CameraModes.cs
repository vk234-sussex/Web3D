using UnityEngine;
using System.Collections.Generic;

namespace Rendering.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
	public class CameraModes : MonoBehaviour
	{
        [SerializeField] private List<CameraMode> modes;
        [SerializeField] private float transitionSpeed = 5f;

		private CameraMode defaultMode;
        private UnityEngine.Camera m_Camera;

        private CameraMode activeMode;

        private void Start()
        {
            m_Camera = GetComponent<UnityEngine.Camera>();
            defaultMode = new CameraMode();
            if (m_Camera.orthographic)
            {
                defaultMode.orthographic = true;
                defaultMode.fov = m_Camera.orthographicSize;
            }
            else
            {
                defaultMode.fov = m_Camera.fieldOfView;
            }
            defaultMode.position = transform.position;
            defaultMode.eulerAngles = transform.rotation.eulerAngles;
            defaultMode.modeName = "default";
            activeMode = defaultMode;
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, activeMode.position, Time.deltaTime * transitionSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(activeMode.eulerAngles), Time.deltaTime * transitionSpeed);
            m_Camera.orthographic = activeMode.orthographic;
            if (m_Camera.orthographic)
            {
                m_Camera.orthographicSize = Mathf.Lerp(m_Camera.orthographicSize, activeMode.fov, Time.deltaTime * transitionSpeed);
            }
            else
            {
                m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, activeMode.fov, Time.deltaTime * transitionSpeed);
            }
        }

        public void ApplyMode(string id)
        {
            foreach (var mode in modes)
            {
                if (mode.modeName == id)
                {
                    activeMode = mode;
                    return;
                }
            }
            Debug.LogWarning($"could not find mode with id {id} - using default");
            activeMode = defaultMode;
        }

        public List<CameraMode> Modes => modes;
    }
}

