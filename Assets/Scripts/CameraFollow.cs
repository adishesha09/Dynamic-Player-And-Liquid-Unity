using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target; // the player
    public Vector3 offset = new Vector3(0, 5f, -10f);
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (!target) return;

        // Desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate to the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        // Always look at the target
        transform.LookAt(target);
    }
}
