using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeDetector : MonoBehaviour
{
    [SerializeField]
    private float shakeThreshold;
    // Time in between detecting shakes
    [SerializeField]
    private float waitTime;

    private GameManager gameManager;
    private float currTime;

    /**
     * Called at the beginning of the scene starting up
     */
    private void Start()
    {
        currTime = 0;
        gameManager = FindObjectOfType<GameManager>();
    }

    /** 
     * Update is called once per frame
     */
    private void Update()
    {
        if (currTime > 0)
        {
            currTime -= Time.deltaTime;
        }

        if (currTime <= 0 && Input.acceleration.magnitude > shakeThreshold)
        {
            gameManager.ShuffleLetters();
            currTime = waitTime;
        }
    }
}
