using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private AudioSource audioSource;
    private void Awake()
    {
        instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
    }
    public void PlayDialogLine(DialogManager dialog, int index)
    {
        Stop();
        if (index < 0 || index >= dialog.dialogAudio.Length) return;

        DialogAudio data = dialog.dialogAudio[index];

        audioSource.clip = data.clip;
        audioSource.volume = data.volume;
        audioSource.pitch = data.pitch;
        audioSource.loop = data.loop;
        audioSource.mute = data.isMute;

        audioSource.Play();
    }
    public void PlayDialogBigLine(DialogAudio dialog)
    {
        audioSource.clip = dialog.clip;
        audioSource.volume = dialog.volume;
        audioSource.pitch = dialog.pitch;
        audioSource.loop = dialog.loop;
        audioSource.mute = dialog.isMute;

        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }
}
