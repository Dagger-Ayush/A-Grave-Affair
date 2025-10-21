using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private AudioSource audioSource;
    private const string mixerPath = "Audio/MainAudioMixer"; // Path inside Resources (no .mixer extension)

    [Header("Audio References")]
    public AudioSource backgroundAudio;
    public AudioMixer audioMixer;
    public AudioMixer BackgroundaudioMixer;

    [Header("UI")]
    public Slider masterSlider;
    public Slider backgroundSlider;

    private void Awake()
    {
            instance = this;
          
        // Load the AudioMixer from Resources folder if not assigned
        if (audioMixer == null)
            audioMixer = Resources.Load<AudioMixer>(mixerPath);

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        // Load saved volumes
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        float savedBackground = PlayerPrefs.GetFloat("BackgroundMixer", 0.75f);

        SetMasterVolume(savedMaster);
        SetBackgroundVolume(savedBackground);

        // Set up sliders
        SetupSliders(savedMaster, savedBackground);

        // Play background music if assigned
        if (backgroundAudio != null)
            backgroundAudio.Play();
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
        // Update sliders in new scene
        SetupSliders(PlayerPrefs.GetFloat("MasterVolume", 0.75f),
                     PlayerPrefs.GetFloat("BackgroundMixer", 0.75f));
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
}
