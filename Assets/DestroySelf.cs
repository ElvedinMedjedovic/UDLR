using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DestroySelf : NetworkBehaviour
{
    [SerializeField] float destroyTime = 1;
    NetworkObject networkObject;
    // Start is called before the first frame update
    void Start()
    {
        networkObject = gameObject.GetComponent<NetworkObject>();
        Destroy(gameObject, destroyTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
    private void OnDestroy()
    {
        networkObject.Despawn();
    }
}
