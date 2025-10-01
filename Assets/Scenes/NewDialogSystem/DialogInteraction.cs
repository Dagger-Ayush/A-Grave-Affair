using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogSystem", menuName = "NewDialog")]
public class DialogManager : ScriptableObject
{
    public string characterName;
    //public GameObject dialogContainer;
    public string[] dialogLines;
    public DialogAudio[] dialogAudio;


    public bool changeFontSize;
    public float[] frontSize;
    public float typingSpeed = 0.02f;
}
