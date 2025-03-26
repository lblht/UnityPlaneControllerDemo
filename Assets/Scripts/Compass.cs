using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform arrow;

    void Start()
    {
        target = GameObject.Find("Island").transform;
    }

    void FixedUpdate()
    {
        if (target == null) { return; }
        Vector3 targetPosition = new Vector3(target.position.x, 0f, target.position.z);
        Vector3 compassPosition = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 directionToTarget = (targetPosition - compassPosition).normalized;
        Vector3 compassForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
        float angle = Vector3.SignedAngle(compassForward, directionToTarget, Vector3.up);
        arrow.localRotation = Quaternion.Euler(0, 0, -angle);
    }

}
