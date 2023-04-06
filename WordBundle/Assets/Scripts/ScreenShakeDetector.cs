using UnityEngine;

/// <summary>
/// Detects screen shakes on mobile to shuffle letters
/// </summary>
public class ScreenShakeDetector : MonoBehaviour
{
    [SerializeField]
    private float shakeThreshold;
    // Time in between detecting shakes
    [SerializeField]
    private float waitTime;

    private GameManager gameManager;
    private float currTime;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    private void Start()
    {
        currTime = 0;
        gameManager = FindObjectOfType<GameManager>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
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
