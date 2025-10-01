using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAudioSystem", menuName = "NewAudio")]
public class DialogAudio : ScriptableObject
{
    
    [HideInInspector] public AudioSource[] sorce;
    public AudioClip clip;
    [Range(0, 3)]
    public float volume = 1;
    [Range(0, 3)]
    public float pitch = 1;

    public bool loop;
    public bool isMute;


   
}
