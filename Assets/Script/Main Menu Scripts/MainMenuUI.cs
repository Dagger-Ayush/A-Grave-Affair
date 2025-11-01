using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    [Header("Menu References")]
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject volumeMenu;
    public GameObject creditsVideoPlayer;

    public static bool IsPaused { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        var vp = creditsVideoPlayer != null ? creditsVideoPlayer.GetComponent<VideoPlayer>() : null;
        if (vp != null)
            vp.loopPointReached += HandleCreditsVideoEnd;
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "Mainmenu")
        {
            TriggerVolume();
        }
        else
        {
            TriggerPauseMenu();
        }

        // Allow Escape key to exit credits video early
        if (creditsVideoPlayer != null && creditsVideoPlayer.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCreditsVideo();
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
        StartCoroutine(LoadGameScene());
    }

    private System.Collections.IEnumerator LoadGameScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Proto Scene");

        while (!asyncLoad.isDone)
            yield return null;

        // Hide all menus after load
        if (volumeMenu != null) volumeMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(false);

        // Reset pause state fully
        Time.timeScale = 1f;
        IsPaused = false;

        if (PlayerInteract.Instance != null)
            PlayerInteract.Instance.OnPauseStateChanged(false);
    }

    // 🔹 NEW — fully integrated pause handling
    public void Continue()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        IsPaused = false;

        if (PlayerInteract.Instance != null)
            PlayerInteract.Instance.OnPauseStateChanged(false);

        Debug.Log("Game Continued");
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        IsPaused = true;

        if (PlayerInteract.Instance != null)
            PlayerInteract.Instance.OnPauseStateChanged(true);

        Debug.Log("Game Paused");
    }

    public void VolumePanel()
    {
        volumeMenu.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
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
            // Case 1: Volume panel active → go back to Pause Menu
            if (volumeMenu != null && volumeMenu.activeSelf)
            {
                volumeMenu.SetActive(false);
                pauseMenu.SetActive(true);
                Debug.Log("Closed Volume Panel → Returned to Pause Menu");
            }
            // Case 2: Pause menu active → resume game
            else if (pauseMenu != null && pauseMenu.activeSelf)
            {
                Continue();
            }
            // Case 3: No menu → open Pause Menu
            else
            {
                Pause();
            }
        }
    }

    private void TriggerVolume()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && volumeMenu != null && volumeMenu.activeSelf)
        {
            volumeMenu.SetActive(false);
            if (mainMenu != null)
                mainMenu.SetActive(true);

            Debug.Log("Closed Volume Panel → Returned to Main Menu");
        }
    }

    public void ShowCreditsVideo()
    {
        if (creditsVideoPlayer != null)
        {
            creditsVideoPlayer.SetActive(true);
            var vp = creditsVideoPlayer.GetComponent<VideoPlayer>();
            if (vp != null)
                vp.Play();
        }
    }

    public void OnCreditsButtonClicked()
    {
        ShowCreditsVideo();
        if (mainMenu != null) mainMenu.SetActive(false);
    }

    void HandleCreditsVideoEnd(VideoPlayer vp)
    {
        creditsVideoPlayer.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    public void ExitCreditsVideo()
    {
        var vp = creditsVideoPlayer.GetComponent<VideoPlayer>();
        if (vp != null && vp.isPlaying)
            vp.Stop();

        creditsVideoPlayer.SetActive(false);
        if (mainMenu != null)
            mainMenu.SetActive(true);

        Debug.Log("Credits video exited early and main menu shown.");
    }
}
