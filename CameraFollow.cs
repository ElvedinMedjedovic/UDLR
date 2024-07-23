using Unity.Netcode;
using UnityEngine;

public class CameraFollow : NetworkBehaviour
{
    public Transform target;
    public Transform clientTarget;
    [SerializeField] private Vector3 offset;
    private Vector3 velocity = Vector3.zero;
    private float smoothTime = 0.3f;
    private Vector3 lastTargetPosition;

    void Start()
    {
        if (target != null)
        {
            lastTargetPosition = clientTarget.position;
        }
    }

    //void FixedUpdate()
    //{
    //    if(!IsHost)
    //    {
    //        if (clientTarget != null)
    //        {

    //            Vector3 targetPosition = Vector3.Lerp(lastTargetPosition, clientTarget.position, 0.5f) + offset;
    //            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    //            lastTargetPosition = clientTarget.position;
    //        }
    //    }
        
    //}
    private void LateUpdate()
    {
        
        transform.position = target.position + offset;
        
        
    }
}
