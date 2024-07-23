using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBallCheckDistance : MonoBehaviour
{
    float distanceBetweenBoss;
    SnakeBoss snakeBoss;
    [SerializeField] float dist = 10;
    [SerializeField] GameObject energy;
    // Start is called before the first frame update
    void Start()
    {
        snakeBoss = GameObject.Find("SnajeBoss").GetComponent<SnakeBoss>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        distanceBetweenBoss = Vector3.Distance(transform.position, snakeBoss.gameObject.transform.position);
        if(distanceBetweenBoss >= dist)
        {
            snakeBoss.gameObject.transform.LookAt(transform);
            snakeBoss.getEnergyWhileChasing = true;
            Debug.Log(distanceBetweenBoss);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("BossDmg"))
        {
            energy.SetActive(false);
        }
    }
}
