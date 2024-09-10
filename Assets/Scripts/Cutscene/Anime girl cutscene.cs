using UnityEngine;
using System.Collections;
public class Animegirlcutscene : MonoBehaviour
{
    // Audio Components
    [Header("Audio Components")]
    [SerializeField] AudioEnvelopeSpeaking animeGirlVoice;

    // Animator Components
    [Header("Animator Components")]
    [SerializeField] Animator playerAnimator;

    // Transform Components
    [Header("Transform Components")]
    [SerializeField] Transform animeGirl;
    [SerializeField] Transform playerStartPlacement;
    Transform playerT;

    // Camera Components
    [Header("Camera Settings")]
    [SerializeField] Camera mainCamera;
    [Tooltip("Secondary camera used for specific sequences.")]
    public Camera cutsceneCamera;
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
    [SerializeField] ThirdPersonController playerController;
    void Awake()
    {
        playerT = GameManager.Instance.playerT;
        playerController = GameManager.Instance.playerController;
        playerAnimator = GameManager.Instance.playerAnimator;

        mainCamera = Camera.main;
        animeGirlVoice = animeGirl.gameObject.GetComponent<AudioEnvelopeSpeaking>();

        animatedCamera = cutsceneCamera.gameObject.GetComponent<AnimatedCamera>();
    }
    public void StartCutscene()
    {
        StartCoroutine(CutScene());
    }
    public void EndCutscene()
    {

    }

    IEnumerator CutScene()
    {
        // //turn on the cutscene camera
        // mainCamera.gameObject.SetActive(false);
        // cutsceneCamera.gameObject.SetActive(true);

        // //player walks up
        // playerT.position = playerStartPlacement.position;
        // playerT.rotation = playerStartPlacement.rotation;
        // playerController.DeactivateControls();
        // //animated camera pans to something
        // animatedCamera.SetTargetTransform(cTarget1);

        // //wauit for player to get there and animate him
        // float startTime = Time.time;
        // Vector3 startPosition = playerT.position;
        // float speed = 0f;
        // while (Time.time < startTime + moveDuration)
        // {
        //     Vector3 previousPosition = playerT.position;
        //     playerT.position = Vector3.Lerp(startPosition, destination.position, (Time.time - startTime) / moveDuration);
        //     speed = Vector3.Distance(playerT.position, previousPosition) / Time.deltaTime;
        //     playerAnimator.SetFloat("speedZ", speed);
        //     yield return null;
        // }
        // //stop doing the walk u dummy
        // playerAnimator.SetFloat("speedZ", 0);

        // //camera switches to other cam which is a close up of the anime girl face turning around 

        // // player turns around to look at dandid... wth i mean anime girl turns around and looks at player
        // Quaternion initialRotation = animeGirl.rotation;
        // Quaternion targetRotation = Quaternion.Euler(0, initialRotation.eulerAngles.y + 180, 0);
        // float rotationSpeed = 1f; // Speed of rotation
        // float rotationProgress = 0f; // Progress from 0 to 1
        // while (rotationProgress < 1f)
        // {
        //     rotationProgress += Time.deltaTime * rotationSpeed;
        //     animeGirl.rotation = Quaternion.Lerp(initialRotation, targetRotation, rotationProgress);
        //     yield return null;
        // }

        // //camera switch to wide angle of anime girl using that original camera we just used before by placing it at the wide angle camera location and rotation

        // //anime girl talks and says stufff using the audio evenelope thingy
        

        // // player guy talks too using his regular player voice thing

        // //they talk some more different camera angle and some panning using animated camera component

        // //a quest is given to get that pictujre



    yield return null;


    }
}
