using UnityEngine;
using System.Collections;

public class Animegirlcutscene : MonoBehaviour
{
    public Quest grandmaPictureQuest;

    // Audio Components
    [Header("Audio Components")]
    [SerializeField] AudioEnvelopeSpeaking animeGirlVoice;

    // Animator Components
    [Header("Animator Components")]
    Animator playerAnimator;

    // Transform Components
    [Header("Transform Components")]
    [SerializeField] Transform animeGirl;
    [SerializeField] Transform playerStartPlacement;
    Transform playerT;

    // Camera Components
    [Header("Camera Settings")]
   Camera mainCamera;
    [Tooltip("Secondary camera used for specific sequences.")]
    public Camera camera1;
    public Camera camera2;
    [Tooltip("Animated camera component attached to the secondary camera.")]
    public AnimatedCamera animatedCamera;
    [Header("Anime Girl Targets")]
    public Transform agTarget1;

    [Header("Camera Targets")]
    public Transform cTarget1;

    [Header("Player Targets")]
    public Transform pTarget1;

    // Player Components
    [Header("Player Components")]
    ThirdPersonController playerController;

    // Missing Variables
    [SerializeField] private float moveDuration = 3f; // How long the player takes to move
    
    [SerializeField] private Transform wideAnglePlacement; // The position for the wide-angle camera placement
    [SerializeField] private Transform wideAnglePan; // The target for wide-angle camera pan

    void Awake()
    {
        playerT = GameManager.Instance.playerT;
        playerController = GameManager.Instance.playerController;
        playerAnimator = GameManager.Instance.playerAnimator;

        mainCamera = Camera.main;
        animeGirlVoice = animeGirl.gameObject.GetComponent<AudioEnvelopeSpeaking>();

        animatedCamera = camera1.gameObject.GetComponent<AnimatedCamera>();
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(false);
    }

    public void StartCutscene()
    {
        StartCoroutine(CutScene());
    }

    public void EndCutscene()
    {
        // Implement end cutscene logic here if needed
    }

    IEnumerator CutScene()
    {
        // Turn on the cutscene camera
        mainCamera.gameObject.SetActive(false);
        camera1.gameObject.SetActive(true);

        // Player walks up
        playerT.position = playerStartPlacement.position;
        playerT.rotation = playerStartPlacement.rotation;
        playerController.DeactivateControls();

        // Animated camera pans to something
        animatedCamera.SetTargetTransform(cTarget1);

        // Wait for the player to reach the pTarget1
        float startTime = Time.time;
        Vector3 startPosition = playerT.position;
        float speed = 0f;
        while (Time.time < startTime + moveDuration)
        {
            Vector3 previousPosition = playerT.position;
            playerT.position = Vector3.Lerp(startPosition, pTarget1.position, (Time.time - startTime) / moveDuration);
            speed = Vector3.Distance(playerT.position, previousPosition) / Time.deltaTime;
            playerAnimator.SetFloat("speedZ", speed);
            yield return null;
        }

        // Stop walking animation
        playerAnimator.SetFloat("speedZ", 0);

        // Camera switches to close up of the anime girl face
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(true);

        // Anime girl turns around and looks at the player
        Quaternion initialRotation = animeGirl.rotation;
        Quaternion targetRotation = agTarget1.rotation;
        float rotationSpeed = 1f;
        float rotationProgress = 0f;
        while (rotationProgress < 1f)
        {
            rotationProgress += Time.deltaTime * rotationSpeed;
            animeGirl.rotation = Quaternion.Lerp(initialRotation, targetRotation, rotationProgress);
            yield return null;
        }

        // Wide angle camera shot of anime girl
        camera1.transform.position = wideAnglePlacement.position;
        camera1.gameObject.SetActive(true);
        camera2.gameObject.SetActive(false);
        animatedCamera.SetTargetTransform(wideAnglePan);
        yield return new WaitWhile(() => animatedCamera.isAnimating);
    

        // Player voice line
        GameManager.Instance.playerVoiceLineController.PlayVoiceLine("who are you");
        float lineDuration = GameManager.Instance.playerVoiceLineController.GetVoiceLineLength("who are you");
        yield return new WaitForSeconds(lineDuration);

        // Anime girl responds
        animeGirlVoice.PlayVoiceLine("I Am An Alien");
        lineDuration = animeGirlVoice.GetVoiceLineLength("I Am An Alien");
        yield return new WaitForSeconds(lineDuration);

        // Player's awkward response
        GameManager.Instance.playerVoiceLineController.PlayVoiceLine("uh ok");
        lineDuration = GameManager.Instance.playerVoiceLineController.GetVoiceLineLength("uh ok");
        yield return new WaitForSeconds(lineDuration);

        // Add quest
        QuestManager.Instance.AddQuest(grandmaPictureQuest);
    }
}
