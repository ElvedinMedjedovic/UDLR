using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPrediction : MonoBehaviour
{
    float timer;
    int currentTick;
    float minTimeBetweenTicks;

    const float tickRate = 30f;
    // Start is called before the first frame update
    void Start()
    {
        minTimeBetweenTicks = 1f / tickRate;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        while(timer >= minTimeBetweenTicks)
        {
            timer -= minTimeBetweenTicks;
            HandleTick();
            currentTick++;
        }
    }

    private void HandleTick()
    {
        
    }
}
