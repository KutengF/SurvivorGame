using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The player transform to follow
    public float smoothSpeed = 0.125f; // Smooth speed for the camera movement
    public Vector3 offset; // Offset position of the camera

    void FixedUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
