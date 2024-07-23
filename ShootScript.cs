using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootScript : NetworkBehaviour
{
    [SerializeField] Rigidbody projectile;
    [SerializeField] Transform[] firePoints;
    
    [SerializeField] GameObject shootHostBtn;
    [SerializeField] GameObject shootClientBtn;


    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        //DontDestroyOnLoad(gameObject);
        ChangeOwnershipServerRpc();
        if (IsHost)
        {
            shootHostBtn.SetActive(true);
            shootClientBtn.SetActive(false);
        }
        if(IsClient && !IsServer)
        {
            shootHostBtn.SetActive(false);
            shootClientBtn.SetActive(true);
            Debug.Log("c;");
        }
    }
    [ServerRpc]
    void ChangeOwnershipServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().RemoveOwnership();
    }
   

    // Update is called once per frame
    void FixedUpdate()
    {
        //if (IsHost)
        //{

            
        //    if (Input.GetButtonDown("F"))
        //    {
        //        ShootForward();
        //    }
        //    if (Input.GetKeyDown(KeyCode.DownArrow))
        //    {
        //        ShootBackward();
        //    }

        //}
        //else if (IsClient)
        //{

            
        //    if (Input.GetKeyDown(KeyCode.G))
        //    {
        //        ShootUpServerRpc();
        //    }
        //    if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    {
        //        ShootDownServerRpc();
        //    }

        //}
    }
    [ServerRpc(RequireOwnership = false)]
   public void ShootUpServerRpc()
    {
        ShootUp();
    }
    [ServerRpc(RequireOwnership = false)]
   public void ShootDownServerRpc()
    {
        ShootDown();
    }
    public void ShootForward()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[3].position, transform.rotation) as Rigidbody;
        
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        instantiatedProjectile.velocity = -transform.right * 20;
        Debug.Log("shot");
    }
    public void ShootBackward()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[2].position, transform.rotation) as Rigidbody;
        
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        instantiatedProjectile.velocity = transform.right * 20;
        Debug.Log("shot");
    }
    void ShootUp()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[0].position, transform.rotation) as Rigidbody;
        
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        instantiatedProjectile.velocity = transform.up * 20;
        Debug.Log("shot");
    }
    void ShootDown()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[1].position, transform.rotation) as Rigidbody;
        
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        instantiatedProjectile.velocity = -transform.up * 20;
        Debug.Log("shot");
    }
}
