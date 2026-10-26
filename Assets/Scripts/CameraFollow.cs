using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;
    public float cameraHeight = 4f;
    public float distanceToTarget = 8f;
    public float followSpeed = 5f;

    void LateUpdate() {
        // camera's target position
        Vector3 targetPosition = new Vector3( 0f, cameraHeight, target.position.z - distanceToTarget);

        // move to the target 
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

}