using UnityEngine;

public class FollowScript : MonoBehaviour
{
    public Transform target; // The object to follow
    public float followSpeed = 5f; // Speed of following

    private Vector3 previousPosition;
    private Quaternion previousRotation;

    void Start()
    {
        // Initialize previous position and rotation
        previousPosition = target.position;
        previousRotation = target.rotation;
    }

    void Update()
    {
        // Calculate new position and rotation with a slight delay
        Vector3 newPosition = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, target.rotation, followSpeed * Time.deltaTime);

        // Apply new position and rotation
        transform.position = newPosition;
        transform.rotation = newRotation;

        // Update previous position and rotation
        previousPosition = target.position;
        previousRotation = target.rotation;
    }
}