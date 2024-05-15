using UnityEngine;

public class UserRotator : MonoBehaviour
{
    public Vector3 rotationPoint = Vector3.zero;
    public float rotationSpeed = 1f;
    private float startTargetAnglesPerSecond;
    private Camera m_cam;

    private RotateForever m_RotateForever;

    private void Start()
    {
        m_cam = Camera.main;
        m_RotateForever = GetComponent<RotateForever>();
    }

    public void Update()
    {
        if (Input.GetMouseButton(0))
        {
            transform.RotateAround(rotationPoint, -m_cam.transform.up, Input.mousePositionDelta.x * rotationSpeed);
            transform.RotateAround(rotationPoint, m_cam.transform.right, Input.mousePositionDelta.y * rotationSpeed);
            m_RotateForever.lastInteractedTime = Time.time;
        }
    }
}

