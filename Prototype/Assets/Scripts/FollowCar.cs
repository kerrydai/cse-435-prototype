using UnityEngine;

public class FollowCar : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float followSpeed = 5f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPos = target.position + target.TransformDirection(offset);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * followSpeed);
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}
