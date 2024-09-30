using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    bool paused = false;
    public GameObject pauseMenu;
    public GameObject settings;

    // Start is called before the first frame update
    void Start()
    {
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        paused = true;
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        AudioSource[] allAudios = FindObjectsOfType<AudioSource>();

        foreach(AudioSource audio in allAudios)
        {
            audio.Pause();
        }
    }

    public void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        paused = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false);

            AudioSource[] allAudios = FindObjectsOfType<AudioSource>();

        foreach(AudioSource audio in allAudios)
        {
            audio.UnPause();
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // public void OpenMainMenu()
    // {
    //     SceneManager.LoadScene(0);
    // }

    public void ResetCurrentScene()
    {
        // get the current active scene's index
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        
        // reload the scene by index
        SceneManager.LoadScene(sceneIndex);
    }

    public void OpenSettings()
    {
        if(settings != null)
        {
            settings.SetActive(true);
        }
        print("clicked settings");
    }

    public void Reset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TogglePause()
    {
        if(paused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

}
