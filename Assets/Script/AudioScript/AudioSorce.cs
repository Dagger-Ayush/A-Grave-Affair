using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioSorce : MonoBehaviour
{
     private AudioSource sorce;
    [SerializeField] private AudioClip clip;
    [Range(0, 3)]
    public float volume = 1;
    [Range(0, 3)]
    public float pitch = 1;

    public bool loop;
    public bool isMute;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sorce = gameObject.AddComponent<AudioSource>();

        sorce.clip = clip;
        sorce.volume = volume;
        sorce.pitch = pitch;
        sorce.mute = isMute;
        sorce.loop = loop;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
