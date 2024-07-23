using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [SerializeField] float limitYN;
    [SerializeField] float limitYP;
    private bool gameOver;
    [SerializeField] private GameObject gameOverScrn;
    private float verticalInput;
    private float horizontalInput;
    private float speed = 7;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (transform.position.y > limitYP)
        {
            transform.position = new Vector3(transform.position.x, limitYP, transform.position.z);
        }
        else if (transform.position.y < limitYN)
        {
            transform.position = new Vector3(transform.position.x, limitYN, transform.position.z);
        }
        transform.Translate(Vector3.right * speed * Time.fixedDeltaTime * horizontalInput);
        transform.Translate(Vector3.up * speed * Time.fixedDeltaTime * verticalInput);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameOver = true;
            gameOverScrn.SetActive(true);
        }
    }
}
