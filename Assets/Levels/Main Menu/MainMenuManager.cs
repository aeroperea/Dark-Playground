using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Controllers")]
    public AnimatedCamera cameraController;
    public QuitGameBehaviour quitGame;
    [SerializeField] ScreenFader screenFader;

    [Header("Camera Targets")]
    public CameraTarget mainTarget;
    public CameraTarget faceTarget;
    public CameraTarget settingsTarget;
    public CameraTarget creditsTarget;

    Coroutine playGameCoroutine;
    Coroutine openSettingsCoroutine;

    void Awake()
    {
        if(screenFader == null)
        {
            GameObject screenFaderObj = GameObject.FindWithTag("Screen Fade");
            if(screenFaderObj != null)
                screenFader = screenFaderObj.GetComponent<ScreenFader>();
        }

    }

    void Start()
    {
        OpenMain();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMain();
        }
    }

    public void PlayGame()
    {
        if (playGameCoroutine == null)
        {
            playGameCoroutine = StartCoroutine(PlayGameCoroutine());
        }
    }

    public void OpenMain()
    {
        cameraController.SetTargetTransform(mainTarget.transform, mainTarget.smoothTime);
    }

    public void OpenSettings()
    {
        if (openSettingsCoroutine == null)
        {
            openSettingsCoroutine = StartCoroutine(OpenSettingsCoroutine());
        }
        else
        {
            Debug.LogError("tried to start open settings but coroutine was not null");
        }
    }

    public void ShowCredits()
    {
        cameraController.SetTargetTransform(creditsTarget.transform, creditsTarget.smoothTime);
    }

    public void QuitGame()
    {
        quitGame.StartCinematic();
    }

    IEnumerator PlayGameCoroutine()
    {
        cameraController.SetTargetTransform(faceTarget.transform, faceTarget.smoothTime);
        screenFader.FadeToBlack(faceTarget.smoothTime);
        yield return new WaitWhile(() => screenFader.isFading);
        yield return new WaitForSeconds(0.5f);
        playGameCoroutine = null;
        SceneManager.LoadScene(1);
        
    }

    IEnumerator OpenSettingsCoroutine()
    {
        cameraController.SetTargetTransform(settingsTarget.transform, settingsTarget.smoothTime);
        yield return null; //possibly implement animation afterwards

        openSettingsCoroutine = null;
    }
}


