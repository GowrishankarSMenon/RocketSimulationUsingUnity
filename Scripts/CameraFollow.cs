using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform rocket;
    public Vector3 offset;
    public float smoothTime = 0.3f; // Time for the smooth damping

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        Vector3 desiredPosition = rocket.position + offset;

        // Smoothly move the camera towards the desired position using SmoothDamp
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

        // Make the camera look at the rocket
        transform.LookAt(rocket.position);
    }
}
