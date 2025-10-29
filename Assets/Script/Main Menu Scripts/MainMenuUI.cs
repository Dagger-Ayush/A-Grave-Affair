using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject volumeMenu;

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "New Mainmenu")
        {
            TriggerVolume();
        }
        else
        {
            TriggerPauseMenu();
           
        }


    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void Play()
    {
        SceneManager.LoadScene("Proto Scene");
    }

    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void VolumePanel()
    {
        volumeMenu.SetActive(true);
        if(pauseMenu!=null) pauseMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(false);
    }

    public void VolumePanelExit()
    {
        volumeMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    private void TriggerPauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Case 1: Volume panel is active → go back to Pause Menu
            if (volumeMenu != null && volumeMenu.activeSelf)
            {
                volumeMenu.SetActive(false);
                pauseMenu.SetActive(true);
                Debug.Log("Closed Volume Panel → Returned to Pause Menu");
            }
            // Case 2: Pause menu is active → resume game
            else if (pauseMenu != null && pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                Debug.Log("Closed Pause Menu → Game Resumed");
            }
            // Case 3: No menu is open → open Pause Menu
            else
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                Debug.Log("Opened Pause Menu");
            }
        }
    }
    private void TriggerVolume()
    {
        // Only act if volume panel is active
        if (Input.GetKeyDown(KeyCode.Escape) && volumeMenu != null && volumeMenu.activeSelf)
        {
            volumeMenu.SetActive(false);
            if (mainMenu != null)
                mainMenu.SetActive(true);

            Debug.Log("Closed Volume Panel → Returned to Main Menu");
        }
    }

}
