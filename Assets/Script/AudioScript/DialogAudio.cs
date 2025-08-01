using System;
using Unity.VisualScripting;
using UnityEngine;

public class DialogAudio : MonoBehaviour
{
    
    [HideInInspector] public AudioSource sorce;
    [SerializeField] private AudioClip clip;
    [Range(0, 3)]
    public float volume = 1;
    [Range(0, 3)]
    public float pitch = 1;

    public bool loop;
    public bool isMute;


    private void Awake()
    {
        if (sorce == null)
        {
            sorce = gameObject.AddComponent<AudioSource>();
        }

        sorce.clip = clip;
        sorce.volume = volume;
        sorce.pitch = pitch;
        sorce.mute = isMute;
        sorce.loop = loop;
    }
    

  
}
