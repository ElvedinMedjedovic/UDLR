using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBoss : MonoBehaviour
{

    // Settings
    public float MoveSpeed = 5;
    public float SteerSpeed = 180;
    public float BodySpeed = 5;
    public int Gap = 20;

    public float radius = 1;
    public float maxDistance = 15;
    public bool ateAllBalls;
    public bool linePhase;
    public bool getEnergyWhileChasing;

    // References
    public GameObject BodyPrefab;
    public Transform energyBalls;
    Rigidbody rb;
    GameObject player;

    // Lists
    private List<GameObject> BodyParts = new List<GameObject>();
    public List<GameObject> followPoints = new List<GameObject>();  
    private List<Vector3> PositionsHistory = new List<Vector3>();
    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        GrowSnake();
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector3.zero;
        // Move forward
        transform.position += transform.forward * MoveSpeed * Time.deltaTime;
        if (!ateAllBalls)
        {
            transform.LookAt(energyBalls);
        }
        else if (linePhase)
        {
            transform.LookAt(followPoints[i].transform);
            if (i == 23)
            {
                linePhase = false;
            }
        }else if(ateAllBalls && !linePhase && !getEnergyWhileChasing)
        {
            transform.LookAt(player.transform);
        }
        
        
        // Steer
        //float steerDirection = Input.GetAxis("Horizontal"); // Returns value -1, 0, or 1
        //transform.Rotate(Vector3.up * steerDirection * SteerSpeed * Time.deltaTime);

        // Store position history
        PositionsHistory.Insert(0, transform.position);

        // Move body parts
        int index = 0;
        foreach (var body in BodyParts)
        {
            Vector3 point = PositionsHistory[Mathf.Clamp(index * Gap, 0, PositionsHistory.Count - 1)];

            // Move body towards the point along the snakes path
            Vector3 moveDirection = point - body.transform.position;
            body.transform.position += moveDirection * BodySpeed * Time.deltaTime;

            // Rotate body towards the point along the snakes path
            body.transform.LookAt(point);

            index++;
        }
        
        
        
        
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == followPoints[i])
        {
            i++;
            transform.LookAt(followPoints[i].transform);
            Debug.Log("tocg");
            //i++;
        }
        //foreach (GameObject gameObject in followPoints)
        //{
            
        //}s
        
    }
    public void GrowSnake()
    {
        // Instantiate body instance and
        // add it to the list
        GameObject body = Instantiate(BodyPrefab);
        BodyParts.Add(body);
    }
}