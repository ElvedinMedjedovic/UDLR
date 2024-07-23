using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MagmaGolem : NetworkBehaviour
{
     Animator animator;
    bool isThrowing;
    WaitForSeconds timeLimit = new WaitForSeconds(5);
    WaitForSeconds lavaTime = new WaitForSeconds(3);
    WaitForSeconds spawnTime = new WaitForSeconds(0.59f);
    [SerializeField] Rigidbody projectilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float speed = 150;
    [SerializeField] GameObject rock;
    [SerializeField] ParticleSystem lava;
    [SerializeField] Transform player;
    [SerializeField] ParticleSystem DeathLava;
    [SerializeField] ParticleSystem DeathFlame;
    [SerializeField] RectTransform bossHp;
    NetworkVariable<float> bossHealth = new NetworkVariable<float>(500f);
    NetworkObject networkObject;
    bool justThrew;
    float timePassed;
    float timeMax = 4;
    bool defeat;
    float defeatPassTime;
    float defeatMaxTime = 7;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        //ThrowRock();
        rock.SetActive(false);
        //LavaAttack();
        ChangeOwnershipServerRpc();
        InvokeRepeating("DefeatAttacks", 2, 1.5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsServer) return;
        timePassed += Time.fixedDeltaTime;
        if (defeat)
        {
            defeatPassTime += Time.fixedDeltaTime;
            if(defeatPassTime >= defeatMaxTime)
            {
                player.GetComponent<PlayerController>().LevelPassed();
            }
            
        }
        if (defeat) return;
        if(timePassed >= timeMax)
        {
            if (!justThrew)
            {
                StartCoroutine(ThrowRockTime());
            }
            else if (justThrew)
            {
                StartCoroutine(lavaAttackTime());
            }
            timePassed = 0f;
            
        }
        

    }
    void DefeatAttacks()
    {
        if (defeat)
        {
            Vector3 randomPos = new Vector3(UnityEngine.Random.Range(-20, 50), UnityEngine.Random.Range(10, 20), 0);

            Rigidbody instantiatedProjectile = Instantiate(projectilePrefab, randomPos, transform.rotation) as Rigidbody;
            instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
            instantiatedProjectile.velocity = -transform.up * speed;
            DeathLava.Stop();
            //DeathFlame.Stop();

        }

    }
    IEnumerator ThrowRockTime()
    {
        
            yield return timeLimit;
        ThrowRockServerRpc();
            
        
        
    }
    [ServerRpc]
    void ChangeOwnershipServerRpc()
    {
        networkObject = gameObject.GetComponent<NetworkObject>();
        NetworkObject.RemoveOwnership();
    }
    [ServerRpc(RequireOwnership = false)]
    private void ThrowRockServerRpc()
    {
        ChangeAnimServerRpc(true);
        rock.SetActive(true);
        StartCoroutine(SpawnRock());
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeAnimServerRpc(bool anim)
    {
        animator.SetBool("isThrowing", anim);
    }
    
    IEnumerator SpawnRock()
    {
        yield return spawnTime;
        Rigidbody instantiatedProjectile = Instantiate(projectilePrefab, firePoint.position, transform.rotation) as Rigidbody;
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        rock.SetActive(false);
        Vector3 throwDirection = (player.position - transform.position).normalized;
        instantiatedProjectile.velocity = throwDirection * speed; // Use transform.forward for simplicity
        yield return spawnTime;
        ChangeAnimServerRpc(false);
        justThrew = true;
    }
    [ServerRpc(RequireOwnership = false)]
    void ChangeAnimLavaServerRpc(bool lava)
    {
        animator.SetBool("Lava", lava);
    }
    [ServerRpc(RequireOwnership = false)]
    void LavaAttackServerRpc()
    {
        ChangeAnimLavaServerRpc(true);
        StartParticleClientRpc();
        StartCoroutine(LavaStop());

    }
    IEnumerator lavaAttackTime()
    {
        if(justThrew)
        {
            yield return timeLimit;
            LavaAttackServerRpc();
            
        }
        
    }
    [ClientRpc]
    void StartParticleClientRpc()
    {
        lava.Play();
    }
    [ClientRpc]
    void StopParticleClientRpc()
    {
        lava.Stop();
    }
    IEnumerator LavaStop()
    {
        yield return lavaTime;
        StopParticleClientRpc();
        justThrew = false;
        ChangeAnimLavaServerRpc(false);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Bullet"))
        {
            bossHealth.Value -= 5;
            bossHp.sizeDelta = new Vector2(bossHealth.Value, 42);
            Debug.Log(bossHealth.Value);
            if(bossHealth.Value <= 0)
            {
                defeat = true;
                DeathLava.Play();
                DeathFlame.Play();
            }
            
        }
    }

}
