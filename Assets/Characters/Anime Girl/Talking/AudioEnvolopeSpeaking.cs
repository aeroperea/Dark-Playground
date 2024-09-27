using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class AudioEnvelopeSpeaking : MonoBehaviour
{
    public Transform targetTransform;
    public float maxHeightOffset = 15f; // Offset from the initial Y position
    private AudioSource audioSource;
    private float initialYPosition;

    // Array to hold voice clips and serialize it for inspector assignment
    [SerializeField] private AudioClip[] voiceClips;

    // Dictionary to hold voice lines
    private Dictionary<string, AudioClip> voiceLineDictionary;

    [Header("Testing")]
    public AudioClip testAudio;
    public bool testing = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Initialize voice lines when the script starts
        InitializeVoiceLines();

        if (testing && testAudio != null)
        {
            PlayAudio(testAudio);
        }
    }

    // Function to initialize voice lines dictionary from voiceClips array
    private void InitializeVoiceLines()
    {
        voiceLineDictionary = new Dictionary<string, AudioClip>();
        foreach (AudioClip clip in voiceClips)
        {
            if (clip != null && !voiceLineDictionary.ContainsKey(clip.name))
            {
                voiceLineDictionary.Add(clip.name, clip);
            }
        }
    }

    // Function to play a voice line using its name (key)
    public void PlayVoiceLine(string clipName)
    {
        if (voiceLineDictionary != null && voiceLineDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            PlayAudio(clip);
        }
        else
        {
            Debug.LogWarning($"Voice line with ID '{clipName}' not found.");
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        if (targetTransform == null)
        {
            Debug.LogError("Target Transform is not assigned.");
            return;
        }

        if (clip == null)
        {
            Debug.LogError("AudioClip is null.");
            return;
        }

        initialYPosition = targetTransform.position.y; // Set initial Y position at the start of analysis
        audioSource.clip = clip;
        audioSource.Play();
        StartCoroutine(AnalyzeAudioCoroutine(clip.length, clip));
    }

    private IEnumerator AnalyzeAudioCoroutine(float duration, AudioClip clip)
    {
        float[] clipSampleData = new float[1024];
        float startTime = Time.time;

        while (Time.time - startTime < duration)
        {
            audioSource.GetOutputData(clipSampleData, 0);
            float currentAverageVolume = GetCurrentAverageVolume(clipSampleData);

            // Map the volume to the transform's position, starting from initial Y position
            float mappedHeight = Mathf.Lerp(initialYPosition, initialYPosition + maxHeightOffset, currentAverageVolume);
            Vector3 currentPosition = targetTransform.position;
            currentPosition.y = mappedHeight;
            targetTransform.position = currentPosition;

            yield return null;
        }

        audioSource.Stop();
    }

    float GetCurrentAverageVolume(float[] data)
    {
        float total = 0;
        foreach (float datum in data)
        {
            total += Mathf.Abs(datum);
        }
        return total / 1024; // Average
    }

    public float GetVoiceLineLength(string clipName)
    {
        if(voiceLineDictionary.TryGetValue(clipName, out AudioClip clip))
        {
            return clip.length;
        }else
        {
            Debug.LogWarning($"Voice Line with ID'{clipName}' notfound.");
            return 0;
        }
    }
}