using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float mouseSensitivity = 4f;
    public float smoothTime = .5f;
    public float pitchMax;
    public float yawMax;

    private float yaw;
    private float pitch;

    private void Awake()
    {
        yaw = transform.eulerAngles.y;
        pitch = transform.eulerAngles.x;
    }

    private void Update()
    {
        if (Input.GetButton("Fire2"))
        {
            yaw += mouseSensitivity * Input.GetAxis("Mouse X");
            pitch -= mouseSensitivity * Input.GetAxis("Mouse Y");
            Debug.Log("yaw"+yaw);
            Debug.Log("pitch" + pitch);

            yaw = Mathf.Clamp(yaw, -yawMax, yawMax);
            pitch = Mathf.Clamp(pitch, -pitchMax, pitchMax);
            transform.eulerAngles = new Vector3(pitch, yaw, 0);
        }
    }
}
