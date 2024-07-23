using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayer : MonoBehaviour
{
    Transform player;
    Rigidbody rb;
    [SerializeField] GameObject projectiles;
    [SerializeField] GameObject expo;
    MoveRight moveRight;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = gameObject.GetComponent<Rigidbody>();
        Destroy(gameObject, 2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movePos = (player.position - transform.position).normalized;
        
        rb.AddForce(movePos * Vector3.Distance(transform.position, player.position), ForceMode.Acceleration);
    }
    private void OnDestroy()
    {
        moveRight = projectiles.GetComponent<MoveRight>();
        moveRight.direction = new Vector3(0, 0, 1);
        Instantiate(expo, transform.position, transform.rotation);
        InsantiateProjectiles();
        moveRight.direction = new Vector3(0, 0, -1);
        InsantiateProjectiles();
        moveRight.direction = new Vector3(0, 1, 0);
        InsantiateProjectiles();
        moveRight.direction = new Vector3(0, -1, 0);
        InsantiateProjectiles();
    }
    private void OnCollisionEnter(Collision collision)
    {
        moveRight = projectiles.GetComponent<MoveRight>();
        moveRight.direction = new Vector3(0, 0, 1);
        Instantiate(expo, transform.position, transform.rotation);
        InsantiateProjectiles();
        moveRight.direction = new Vector3(0, 0, -1);
        InsantiateProjectiles();
        moveRight.direction = new Vector3(0, 1, 0);
        InsantiateProjectiles();
        moveRight.direction = new Vector3(0, -1, 0);
        InsantiateProjectiles();
        Destroy(gameObject);
    }
    void InsantiateProjectiles()
    {
        Instantiate(projectiles, transform.position, transform.rotation);
    }
}
