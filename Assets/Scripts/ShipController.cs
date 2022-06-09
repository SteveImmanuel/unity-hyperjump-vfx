using UnityEngine;

public class ShipController : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float smoothTime = .5f;

    private float yaw;
    private float pitch;
    private float roll;
    private Quaternion targetRotation;

    private void Awake()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        pitch = Input.GetAxisRaw("Vertical") * movementSpeed;
        roll = -Input.GetAxisRaw("Horizontal") * movementSpeed;
        yaw = Input.GetAxisRaw("Elevation") * movementSpeed;

        targetRotation *= Quaternion.Euler(new Vector3(pitch, yaw, roll));
        transform.rotation =  Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
    }
}
