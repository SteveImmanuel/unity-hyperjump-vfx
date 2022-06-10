using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 4f;
    public float pitchMax;
    public float yawMax;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        yaw = transform.localEulerAngles.y;
        pitch = transform.localEulerAngles.x;
    }

    private void Update()
    {
        yaw += mouseSensitivity * Input.GetAxis("Mouse X");
        pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, -yawMax, yawMax);
        pitch = Mathf.Clamp(pitch, -pitchMax, pitchMax);
        transform.localEulerAngles = new Vector3(pitch, yaw, 0);
    }
}
