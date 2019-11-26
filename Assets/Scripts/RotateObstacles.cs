using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DisallowMultipleComponent]
public class RotateObstacles : MonoBehaviour
{

    [SerializeField] Vector3 rotateVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f; // time of one full circle

    //todo remove from inspector later
    float rotateFactor; //0 for not moved, 1 for fully moved
    Vector3 startingPos;

    // Use this for initialization
    void Start()
    {
        startingPos = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {

        if (period <= Mathf.Epsilon) //Epsilon is the smallest float
        {
            return;
        }
        float cycles = Time.time / period; //grows continually from 0
        const float tau = Mathf.PI * 2; //about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); //goes from -1 to +1

        rotateFactor = rawSinWave / 2f + 0.5f; // goes from 0 to 1
        Vector3 offset = rotateVector * rotateFactor;
        transform.eulerAngles = startingPos + offset;
    }
}

