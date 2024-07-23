using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBalls : MonoBehaviour
{
    public static int balls;
    [SerializeField] List<GameObject> energy = new List<GameObject>();
    int followIndex;
    Animator vineAnimator;
    // Start is called before the first frame update
    void Start()
    {
        vineAnimator = GameObject.Find("Vines").GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "BossDmg")
        {
            energy[balls].SetActive(false);
            balls++;
            SnakeBoss snakeBoss = collision.gameObject.GetComponent<SnakeBoss>();
            snakeBoss.GrowSnake();
            snakeBoss.energyBalls = energy[balls].transform;
            if(balls == 9)
            {
                snakeBoss.ateAllBalls = true;
                snakeBoss.linePhase = true;
                snakeBoss.gameObject.transform.LookAt(snakeBoss.followPoints[followIndex].transform);
                vineAnimator.SetBool("HasAte", true);
                Debug.Log(balls + "Has");
            }
            Debug.Log(balls);
            energy[balls].SetActive(true);
        }
    }
}
