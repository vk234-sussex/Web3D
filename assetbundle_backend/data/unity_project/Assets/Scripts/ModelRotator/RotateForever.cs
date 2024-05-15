using UnityEngine;

public class RotateForever : MonoBehaviour
{
    public float targetAnglesPerSecond = 15f;
    public float anglesPerSecond = 15f;
    public float changeSpeed = 10f;
    public Vector2 defaultXzAngle;
    public float delayBetweenInteractions = 5f;

    public float lastInteractedTime { get; set; }

    private float _startAnglesPerSecond;
    private bool AutoRotate = true;

    private void Start()
    {
        lastInteractedTime = int.MinValue;
        _startAnglesPerSecond = targetAnglesPerSecond;
    }

    // Update is called once per frame
    void Update()
    {
        if (!AutoRotate) return;
        if (Time.time - lastInteractedTime <= delayBetweenInteractions) return;

        anglesPerSecond = Mathf.Lerp(anglesPerSecond, targetAnglesPerSecond, Time.deltaTime * changeSpeed);
        Quaternion targetAngle = Quaternion.Euler(new Vector3(defaultXzAngle.x, transform.eulerAngles.y + Time.deltaTime * anglesPerSecond, defaultXzAngle.y));
        Quaternion actualAngle = Quaternion.Euler(new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Time.deltaTime * anglesPerSecond, transform.eulerAngles.z));
        transform.rotation = Quaternion.Slerp(actualAngle, targetAngle, Time.deltaTime * anglesPerSecond / 5f);
        transform.position = Vector3.Lerp(transform.position, Vector3.zero, Time.deltaTime * anglesPerSecond / 5f);
    }

    public void DisableAutoRotate()
    {
        targetAnglesPerSecond = 0f;
    }

    public void EnableAutoRotate()
    {
        targetAnglesPerSecond = _startAnglesPerSecond;
    }

    public void ToggleAutoRotate()
    {
        if (targetAnglesPerSecond == 0f)
        {
            EnableAutoRotate();
            return;
        }
        DisableAutoRotate();
    }
}
