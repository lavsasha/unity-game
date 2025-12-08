using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;    
    public Vector3 offset = new Vector3(0, 6, -10);
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothPosition;
        transform.LookAt(target);
    }
}
