using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class PlayerController : NetworkBehaviour
{





    //nonPredict
    [SerializeField] float speed = 7;
    [SerializeField] float clientSpeed = 7;
    [SerializeField] GameObject gameOverScrn;
    [SerializeField] GameObject levelPassScrn;
    [SerializeField] ParticleSystem Flames;
    [SerializeField] Rigidbody projectile;
    float horizontalInput;
    float verticalInput;
    NetworkVariable<bool> gameOver = new NetworkVariable<bool>(false);
    [SerializeField] float limitYN;
    [SerializeField] float limitYP;
    [SerializeField] float limitXN = -9.88f;
    [SerializeField] float limitXP;
    float health = 5;
    bool isOnFire;
    bool isPressedDown1;
    bool isPressedDown2;
    bool isPressedDown3;
    bool isPressedDown4;
    Rigidbody rb;
    float timePassed;
    [SerializeField] Transform[] firePoints;
    //[SerializeField] Rigidbody rb;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        RemoveOwnershipServerRpc();
        
    }

    

   
    [ServerRpc(RequireOwnership = false)]
    void RemoveOwnershipServerRpc()
    {
        NetworkObject networkObject = gameObject.GetComponent<NetworkObject>();
        networkObject.RemoveOwnership();
    }

    public override void OnNetworkSpawn()
    {
        // Debug log to check initial position
        Debug.Log($"Initial position: {transform.position}");
        
        // Set the initial position for both host and client
        if (IsServer)
        {
            // Set the position for the host
            transform.position = new Vector3(0.00f, 0.98f, 0.00f);
        }
        else if (IsClient)
        {
            // Set the position for the client (if needed)
            transform.position = new Vector3(0.00f, 0.98f, 0.00f);
        }
    }

    private void Update()
    {


        if (IsHost)
        {
            horizontalInput = Input.GetAxis("Horizontal");


        }
        else if (IsClient)
        {
            verticalInput = Input.GetAxis("Vertical");


        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        rb.velocity = Vector3.zero;

        if (isOnFire)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= 2)
            {
                health -= 1;
                if (health <= 0)
                {
                    gameOver.Value = true;
                    gameOverScrn.SetActive(true);
                }
                isOnFire = false;
                timePassed = 0;

            }
            Flames.Play();
        }
        else
        {
            Flames.Stop();
            timePassed = 0;
        }

        if (transform.position.y > limitYP)
        {
            transform.position = new Vector3(transform.position.x, limitYP, transform.position.z);
        }
        else if (transform.position.y < limitYN)
        {
            transform.position = new Vector3(transform.position.x, limitYN, transform.position.z);
        }
        else if (transform.position.x < limitXN && SceneManager.GetActiveScene().buildIndex == 11)
        {
            transform.position = new Vector3(limitXN, transform.position.y, transform.position.z);
        }
        if (gameOver.Value == true)
        {
            gameOverScrn.SetActive(true);
            return;
        }

        if (IsHost)
        {

            transform.Translate(Vector3.right * speed * Time.fixedDeltaTime * horizontalInput);
            if (Input.GetKeyDown(KeyCode.W))
            {
                ShootForward();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ShootBackward();
            }

        }
        else if (IsClient)
        {

            MoveServerRpc(verticalInput);
            if (Input.GetKeyDown(KeyCode.D))
            {
                ShootUpServerRpc();
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                ShootDownServerRpc();
            }

        }
        if (isPressedDown1)
        {
            MoveUpServerRpc();

        }
        else if (isPressedDown2)
        {
            MoveDownServerRpc();

        }
        else if (isPressedDown3)
        {
            MoveRight();

        }
        else if (isPressedDown4)
        {
            MoveLeft();

        }

        if (!IsOwner) return;
        

    }
    public void MoveRight()
    {
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime);
        Debug.Log("r");
    }
    public void MoveLeft()
    {
        transform.Translate(Vector3.left * speed * Time.fixedDeltaTime);
    }
    [ServerRpc(RequireOwnership = false)]
    public void MoveUpServerRpc()
    {
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime);
    }
    [ServerRpc(RequireOwnership = false)]
    public void MoveDownServerRpc()
    {
        transform.Translate(Vector3.down * speed * Time.fixedDeltaTime);
    }
    public void LevelPassed()
    {
        levelPassScrn.SetActive(true);
    }
    [ServerRpc(RequireOwnership = false)]
    void MoveServerRpc(float verticalInput)
    {

        transform.Translate(Vector3.up * clientSpeed * Time.fixedDeltaTime * verticalInput);
    }
    [ServerRpc(RequireOwnership = false)]
    void ShootUpServerRpc()
    {
        ShootUp();
    }
    [ServerRpc(RequireOwnership = false)]
    void ShootDownServerRpc()
    {
        ShootDown();
    }
    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver.Value = true;
            gameOverScrn.SetActive(true);
            Debug.Log(gameOver);

        }
        else if (collision.gameObject.CompareTag("BossDmg"))
        {
            health -= 1;
            if (health <= 0)
            {
                gameOver.Value = true;
                gameOverScrn.SetActive(true);
                Debug.Log(gameOver);
            }
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            levelPassed();
        }
    }
    public void levelPassed()
    {
        gameOver.Value = true;
        levelPassScrn.SetActive(true);
        gameOverScrn.SetActive(true);
    }
    private void OnParticleCollision(GameObject other)
    {
        isOnFire = true;
        Debug.Log(isOnFire);
    }
    void ShootForward()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[2].position, transform.rotation) as Rigidbody;
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        Debug.Log("shot");
    }
    void ShootBackward()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[3].position, transform.rotation) as Rigidbody;
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        Debug.Log("shot");
    }
    void ShootUp()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[0].position, transform.rotation) as Rigidbody;
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        Debug.Log("shot");
    }
    void ShootDown()
    {
        Rigidbody instantiatedProjectile = Instantiate(projectile, firePoints[1].position, transform.rotation) as Rigidbody;
        instantiatedProjectile.gameObject.GetComponent<NetworkObject>().Spawn();
        Debug.Log("shot");
    }
    public void OnButtonDown1()
    {
        isPressedDown1 = true;
    }
    public void OnButtonUp1()
    {
        isPressedDown1 = false;
    }
    public void OnButtonDown2()
    {
        isPressedDown2 = true;
    }
    public void OnButtonUp2()
    {
        isPressedDown2 = false;
    }
    public void OnButtonDown3()
    {
        isPressedDown3 = true;
    }
    public void OnButtonUp3()
    {
        isPressedDown3 = false;
    }
    public void OnButtonDown4()
    {
        isPressedDown4 = true;
    }
    public void OnButtonUp4()
    {
        isPressedDown4 = false;
    }
}