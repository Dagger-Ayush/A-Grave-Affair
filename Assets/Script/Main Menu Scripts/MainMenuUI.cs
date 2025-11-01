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

    private bool escapePressedLastFrame = false;
    private bool isSceneLoading = false;
    private bool canPlay = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(EnablePlayAfterDelay(0.25f));

        var vp = creditsVideoPlayer != null ? creditsVideoPlayer.GetComponent<VideoPlayer>() : null;
        if (vp != null)
            vp.loopPointReached += HandleCreditsVideoEnd;
    }

    private System.Collections.IEnumerator EnablePlayAfterDelay(float delay)
    {
        canPlay = false;
        yield return new WaitForSeconds(delay);
        canPlay = true;
        Debug.Log("Main menu ready — Play button now enabled.");
    }

    void Update()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        bool escapePressed = Input.GetKey(KeyCode.Escape);

        // Only trigger ESC logic once per press
        if (escapePressed && !escapePressedLastFrame)
        {
            if (creditsVideoPlayer != null && creditsVideoPlayer.activeSelf)
            {
                ExitCreditsVideo();
            }
            else if (currentScene.name == "Mainmenu")
            {
                TriggerVolume();
            }
            else
            {
                TriggerPauseMenu();
            }
        }

        escapePressedLastFrame = escapePressed;
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    // 🔹 Play button — now waits 0.25 seconds before being usable
    public void Play()
    {
        if (!canPlay)
        {
            Debug.LogWarning("Please wait a moment — main menu still initializing.");
            return;
        }

        if (isSceneLoading)
        {
            Debug.Log("Already loading scene — please wait.");
            return;
        }

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name != "Mainmenu")
        {
            Debug.LogWarning("Play can only be used in Mainmenu scene.");
            return;
        }

        StartCoroutine(LoadGameScene());
    }

    private System.Collections.IEnumerator LoadGameScene()
    {
        isSceneLoading = true;
        Debug.Log("Starting game scene load...");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Proto Scene");
        asyncLoad.allowSceneActivation = true;

        while (!asyncLoad.isDone)
            yield return null;

        // Hide all menus after load
        if (volumeMenu != null) volumeMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(false);

        // Reset pause state
        Time.timeScale = 1f;
        IsPaused = false;

        if (PlayerInteract.Instance != null)
            PlayerInteract.Instance.OnPauseStateChanged(false);

        isSceneLoading = false;
        Debug.Log("Game scene loaded successfully.");
    }

    // 🔹 Continue Game
    public void Continue()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        IsPaused = false;

        if (PlayerInteract.Instance != null)
            PlayerInteract.Instance.OnPauseStateChanged(false);

        Debug.Log("Game Continued");
    }

    // 🔹 Pause Game
    public void Pause()
    {
        if (pauseMenu != null) pauseMenu.SetActive(true);
        Time.timeScale = 0.0f;
        IsPaused = true;

        if (PlayerInteract.Instance != null)
            PlayerInteract.Instance.OnPauseStateChanged(true);

        Debug.Log("Game Paused");
    }

    public void VolumePanel()
    {
        if (volumeMenu != null) volumeMenu.SetActive(true);
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(false);
    }

    public void VolumePanelExit()
    {
        if (volumeMenu != null) volumeMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
        if (mainMenu != null) mainMenu.SetActive(true);
    }

    // ✅ Clean pause logic (no double press)
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
                return;
            }

            // Case 2: Already paused → resume
            if (IsPaused)
            {
                Continue();
            }
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
