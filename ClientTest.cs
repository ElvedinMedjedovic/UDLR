using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class ClientTest : NetworkBehaviour
{
    [SerializeField] GameObject clientCanvas;
    [SerializeField] NetworkObject networkObject;
    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        networkObject = gameObject.GetComponent<NetworkObject>();
        networkObject.ChangeOwnership(NetworkManager.Singleton.LocalClientId);
        GameObject InstantiatedCanvas = Instantiate(clientCanvas, transform.position, transform.rotation);
        ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[1];
        InstantiatedCanvas.GetComponent<NetworkObject>().SpawnWithOwnership(clientId);



        
        
    }
   

    // Update is called once per frame
    void Update()
    {
        Debug.Log(IsOwner);
    }
}
