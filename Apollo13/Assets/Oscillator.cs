using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVecor = new Vector3 (10f, 10f, 10f);
    [SerializeField] Quaternion rotationVector = new Quaternion();
    [SerializeField] float period = 4;

    [Range(0,-1)]
    [SerializeField]
    float movementFactor;

    
    Vector3 startingPos;
    Quaternion startingRot;
    void Start()
    {
        startingPos = transform.position;
        startingRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon) { return; }
        float cycles = Time.time / period;

        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f -0.5f;
        Vector3 offset = movementVecor * movementFactor;
        transform.position = startingPos + offset;

        /*
        //update rotation        
        startingRot.x = Time.time * 30 ;         
        transform.rotation = startingRot ;
        print(startingRot.x); 
        */
    }
}
