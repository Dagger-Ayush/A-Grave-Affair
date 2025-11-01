using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;
    private const string mixerPath = "Audio/MainAudioMixer";

    [Header("Audio References")]
    public AudioSource backgroundAudio;
    public AudioMixer audioMixer;
    public AudioMixer BackgroundaudioMixer;

    [Header("Audio clips")]
    public AudioClip MainMenu;
    public AudioClip protoScene;
    public AudioClip meetingWithGreg;
    public AudioClip Phase_1;
    public AudioClip Phase_2;
    public AudioClip Phase_3;
    public AudioClip Phase_4;
    public AudioClip Phase_5;
    public AudioClip Phase_6;
    public AudioClip Phase_7;

    [Header("UI")]
    public Slider masterSlider;
    public Slider backgroundSlider;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;
    
    private Coroutine fadeCoroutine;

    // ============================================
    // 🧠 NEW: Music continuity between MotelLobby & NancyRoom
    // ============================================
    private const string MUSIC_TIME_KEY = "SavedMotelNancyMusicTime";
    private const string MUSIC_CLIP_KEY = "SavedMotelNancyMusicClip";
    private const string SCENE_KEY = "SavedSceneName";
    private float lastSavedTime = 0f;

    public float volumeBackground = 0.25f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioMixer == null)
            audioMixer = Resources.Load<AudioMixer>(mixerPath);

        if (backgroundAudio == null)
            backgroundAudio = gameObject.AddComponent<AudioSource>();

        audioSource = gameObject.AddComponent<AudioSource>();

        backgroundAudio.loop = true;
        backgroundAudio.playOnAwake = false;
    }

    private void Start()
    {
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedBackground = PlayerPrefs.GetFloat("BackgroundMixer", 0.75f);

        SetMasterVolume(savedMaster);
        SetBackgroundVolume(savedBackground);
        SetupSliders(savedMaster, savedBackground);

        TrySetSceneBackgroundStart(SceneManager.GetActiveScene().name, true);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupSliders(
            PlayerPrefs.GetFloat("MasterVolume", 0.75f),
            PlayerPrefs.GetFloat("BackgroundMixer", 0.75f)
        );

        string lastScene = PlayerPrefs.GetString(SCENE_KEY, "");
        bool isBetweenMotelAndNancy =
            (scene.name == "Motel_Lobby" || lastScene == "Nancy Room");

        if (isBetweenMotelAndNancy)
        {
            float savedTime = PlayerPrefs.GetFloat(MUSIC_TIME_KEY, 0f);
            string savedClipName = PlayerPrefs.GetString(MUSIC_CLIP_KEY, "");

            StartCoroutine(RestoreBackgroundPosition(savedTime, savedClipName));
            // Continue from previous timestamp
            TrySetSceneBackgroundStart(scene.name, false);
           
        }
        else
        {
            TrySetSceneBackgroundStart(scene.name, true);
        }
    }

    private void SetupSliders(float savedMaster, float savedBackground)
    {
        if (backgroundSlider != null)
        {
            backgroundSlider.maxValue = volumeBackground; // use your variable
            backgroundSlider.value = Mathf.Min(savedBackground, volumeBackground);
            backgroundSlider.onValueChanged.RemoveAllListeners();
            backgroundSlider.onValueChanged.AddListener(SetBackgroundVolume);
        }

        if (masterSlider != null)
        {
            masterSlider.value = savedMaster;
            masterSlider.onValueChanged.RemoveAllListeners();
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }
    }


    public void SetMasterVolume(float value)
    {
        if (audioMixer == null) return;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetBackgroundVolume(float value)
    {
        if (BackgroundaudioMixer == null) return;

        // 🔒 Clamp using volumeBackground as the max limit
        float clampedValue = Mathf.Clamp(value, 0.0001f, volumeBackground);

        BackgroundaudioMixer.SetFloat("BackgroundMixer", Mathf.Log10(clampedValue) * 20);
        PlayerPrefs.SetFloat("BackgroundMixer", clampedValue);

        // ✅ Keep volume consistent after scene load
        backgroundAudio.volume = clampedValue;
    }

    // ============================================
    // 🎙 Dialog Audio
    // ============================================
    public void PlayDialogLine(DialogManager dialog, int index)
    {
        Stop();
        if (index < 0 || index >= dialog.dialogAudio.Length) return;

        DialogAudio data = dialog.dialogAudio[index];
        PlayAudioClip(data);
    }

    public void PlayDialogBigLine(DialogAudio dialog)
    {
        PlayAudioClip(dialog);
    }

    private void PlayAudioClip(DialogAudio dialog)
    {
        if (dialog == null || dialog.clip == null) return;

        audioSource.clip = dialog.clip;
        audioSource.volume = dialog.volume;
        audioSource.pitch = dialog.pitch;
        audioSource.loop = dialog.loop;
        audioSource.mute = dialog.isMute;

        if (audioMixer != null)
        {
            var groups = audioMixer.FindMatchingGroups("Master");
            if (groups.Length > 0)
                audioSource.outputAudioMixerGroup = groups[0];
        }

        audioSource.Play();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    // ============================================
    // 🎵 Background Crossfade System
    // ============================================
    public void SetBackgroundAudio(AudioClip newClip)
    {
        if (backgroundAudio == null || newClip == null) return;

        if (backgroundAudio.clip == newClip && backgroundAudio.isPlaying)
            return;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeBackgroundMusic(newClip));
    }

    private IEnumerator FadeBackgroundMusic(AudioClip newClip)
    {
        float startVolume = backgroundAudio.volume;
        float targetVolume = PlayerPrefs.GetFloat("BackgroundMixer", 0.75f);

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            backgroundAudio.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        backgroundAudio.volume = 0f;
        backgroundAudio.Stop();

        backgroundAudio.clip = newClip;
        backgroundAudio.loop = true;
        backgroundAudio.Play();

        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            backgroundAudio.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        backgroundAudio.volume = targetVolume;
    }

    private void TrySetSceneBackgroundStart(string sceneName, bool restartIfNew)
    {
        AudioClip clipToPlay = null;

        if (sceneName == "Mainmenu")
            clipToPlay = MainMenu;
        else if (sceneName == "Proto Scene")
            clipToPlay = protoScene;
        else if (sceneName == "Motel_Lobby")
            clipToPlay = Phase_1;
        else if (sceneName == "Nancy Room")
            clipToPlay = Phase_4;
        else if (sceneName == "Outside_Motel")
            clipToPlay = meetingWithGreg;

        if (clipToPlay == null) return;

        if (restartIfNew || backgroundAudio.clip != clipToPlay)
            SetBackgroundAudio(clipToPlay);
    }

    public void SetSceneBackgroundByPhase(int phaseCount)
    {
        AudioClip clipToPlay = null;
        switch (phaseCount)
        {
            case 1: clipToPlay = Phase_2; break;
            case 2: clipToPlay = Phase_3; break;
            case 3: clipToPlay = Phase_4; break;
            case 4: clipToPlay = Phase_5; break;
            case 5: break;
            case 6: clipToPlay = Phase_7; break;
            default: clipToPlay = MainMenu; break;
        }
        SetBackgroundAudio(clipToPlay);
    }

    // ============================================
    // 🧠 Save & restore position logic
    // ============================================
    private void OnApplicationQuit()
    {
        SaveBackgroundPositionIfNeeded(SceneManager.GetActiveScene().name);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveBackgroundPositionIfNeeded(SceneManager.GetActiveScene().name);
    }

    private void SaveBackgroundPositionIfNeeded(string sceneName)
    {
        if (sceneName == "Motel_Lobby" || sceneName == "Nancy Room")
        {
            if (backgroundAudio != null && backgroundAudio.isPlaying && backgroundAudio.clip != null)
            {
                PlayerPrefs.SetFloat(MUSIC_TIME_KEY, backgroundAudio.time);
                PlayerPrefs.SetString(MUSIC_CLIP_KEY, backgroundAudio.clip.name);
                PlayerPrefs.SetString(SCENE_KEY, sceneName);
                PlayerPrefs.Save();
            }
        }
    }

    private IEnumerator RestoreBackgroundPosition(float time, string clipName)
    {
        yield return new WaitForSeconds(0.5f);

        if (backgroundAudio != null && backgroundAudio.clip != null &&
            backgroundAudio.clip.name == clipName)
        {
            backgroundAudio.time = Mathf.Min(time, backgroundAudio.clip.length - 0.1f);
            backgroundAudio.Play();
        }
    }
}
