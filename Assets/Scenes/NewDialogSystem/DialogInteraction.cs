using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogSystem", menuName = "NewDialog")]
public class DialogManager : ScriptableObject
{
    public string characterName;
    public string[] dialogLines;
    public DialogAudio[] dialogAudio;

    public bool changeFontSize;
    public float[] frontSize;
    public float typingSpeed = 0.02f;

    // ----- Clues per line -----
    public int[] currentClueCount;       // number of clues for each line
    public int[] totalCount;       // number of clues for each line
  
    public void ResetClues()
    {
        if (currentClueCount != null)
        {
            for (int i = 0; i < currentClueCount.Length; i++)
                currentClueCount[i] = 0;
        }
    }

}
