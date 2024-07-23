using Unity.Netcode;
using UnityEngine;

public class FollowPlayer : NetworkBehaviour
{
    public Transform target;
    [SerializeField] private Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.1f;
    private Vector3 lastTargetPosition;

    void Start()
    {
        if (target != null)
        {
            lastTargetPosition = target.position;
        }
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 targetPosition = Vector3.Lerp(lastTargetPosition, target.position, 0.5f) + offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            lastTargetPosition = target.position;
        }
    }
}