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

    private void Awake()
    {
        Instance = this;

        if (audioMixer == null)
            audioMixer = Resources.Load<AudioMixer>(mixerPath);

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedBackground = PlayerPrefs.GetFloat("BackgroundMixer", 0.75f);

        SetMasterVolume(savedMaster);
        SetBackgroundVolume(savedBackground);
        SetupSliders(savedMaster, savedBackground);
        // Auto-set background based on initial scene
        TrySetSceneBackgroundStart(SceneManager.GetActiveScene().name);
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

        TrySetSceneBackgroundStart(scene.name);
    }

    private void SetupSliders(float savedMaster, float savedBackground)
    {
        if (masterSlider != null)
        {
            masterSlider.value = savedMaster;
            masterSlider.onValueChanged.RemoveAllListeners();
            masterSlider.onValueChanged.AddListener(SetMasterVolume);
        }

        if (backgroundSlider != null)
        {
            backgroundSlider.value = savedBackground;
            backgroundSlider.onValueChanged.RemoveAllListeners();
            backgroundSlider.onValueChanged.AddListener(SetBackgroundVolume);
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
        BackgroundaudioMixer.SetFloat("BackgroundMixer", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1)) * 20);
        PlayerPrefs.SetFloat("BackgroundMixer", value);
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
            return; // don't restart same clip

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeBackgroundMusic(newClip));
    }

    private IEnumerator FadeBackgroundMusic(AudioClip newClip)
    {
        float startVolume = backgroundAudio.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            backgroundAudio.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        backgroundAudio.volume = 0f;
        backgroundAudio.Stop();

        // Switch clip
        backgroundAudio.clip = newClip;
        backgroundAudio.loop = true;
        backgroundAudio.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            backgroundAudio.volume = Mathf.Lerp(0f, startVolume, t / fadeDuration);
            yield return null;
        }

        backgroundAudio.volume = startVolume;
    }


    private void TrySetSceneBackgroundStart(string sceneName)
    {
        // only change background for specific scenes
        if (sceneName == "Mainmenu")
        {
            SetBackgroundAudio(MainMenu);
        }
        else if (sceneName == "PrototypeScene")
        {
            SetBackgroundAudio(protoScene);
        }

    }
    public void SetSceneBackgroundByPhase(int phaseCount)
    {
        AudioClip clipToPlay = null;

        switch (phaseCount)
        {
            case 1:
                clipToPlay = Phase_2;
                break;
            case 2:
                clipToPlay = Phase_3;
                break;
            case 3:
                clipToPlay = Phase_4;
                break;
            case 4:
                clipToPlay = Phase_5;
                break;
            case 5:
                clipToPlay = Phase_6;
                break;
            case 6:
                clipToPlay = Phase_7;
                break;
            default:
                clipToPlay = MainMenu; // fallback
                break;
        }

        SetBackgroundAudio(clipToPlay);
    }

}
