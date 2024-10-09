using UnityEngine;

public class CreditScene : MonoBehaviour
{
    float startTime = 0;
    float progress = 0;
    public Transform creditsTransform;  // Transform containing the credits
    public float duration = 50f;     // Speed at which credits scroll
    public Transform startPosition;     // Start position for credits
    public Transform endPosition;       // End position for credits
    public AudioSource backgroundMusic; // Audio source for the background music

    private bool isScrolling = false;
    private bool isResetting = false;

    void Start()
    {
        // Ensure the credits are at the start position initially
        creditsTransform.position = startPosition.position;
    }

    // Public function to start the credits
    public void StartCredits()
    {
        startTime = Time.time;
        // Ensure the background music is playing
        if (backgroundMusic != null)
        {
            backgroundMusic.Play();
        }

        // Move credits to the start position and begin scrolling
        creditsTransform.position = startPosition.position;
        isScrolling = true;
        isResetting = false;
    }

    // Public function to stop the credits and reset
    public void ExitCredits()
    {
        progress = 0;
    
        // Stop the background music
        if (backgroundMusic != null)
        {
            backgroundMusic.Stop();
        }

        // Set flag to reset credits to the start position
        isScrolling = false;
        isResetting = true;
    }

    void Update()
    {
        // Handle scrolling of the credits
        if (isScrolling)
        {
            progress += Time.deltaTime / duration;
            creditsTransform.position = Vector3.Lerp(startPosition.position, endPosition.position, progress);

            // Check if the credits have reached the end position
            if (Vector3.Distance(creditsTransform.position, endPosition.position) < 0.1f)
            {
                // Ensure credits are exactly at the end position
                creditsTransform.position = endPosition.position;
                isScrolling = false;
            }
        }

        // Handle resetting of the credits
        if (isResetting)
        {
            // Move the credits back to the start position
            creditsTransform.position = Vector3.Lerp(creditsTransform.position, startPosition.position, Time.deltaTime * duration);

            // Check if credits are back to the start position
            if (Vector3.Distance(creditsTransform.position, startPosition.position) < 0.1f)
            {
                // Ensure credits are exactly at the start position
                creditsTransform.position = startPosition.position;
                isResetting = false;
            }
        }
    }
}
