using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform rocket;

    public float smoothSpeed = 0.125f;
    [SerializeField] Vector3 offset;

    void FixedUpdate()
    {
        Vector3 desiredPosition = rocket.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        //transform.LookAt(rocket);
    }

}
