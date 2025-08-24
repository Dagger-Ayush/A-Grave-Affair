using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GettingClueCount : MonoBehaviour
{
    public static GettingClueCount instance;
    public int clue;
    public bool isClue;


    public static int clueCount;
    public int totalClues;
    [HideInInspector] public bool WillClueCountStop = false;

    public ObjectPickReferences pickReferences;

    private void Awake()
    {
        instance = this;
    }
  
    private void Update()
    { 
        if (clueCount > 0)
            {
            pickReferences.currentClueCount.text = "Clues Found (" + clueCount.ToString() + "/" + totalClues.ToString() + ")";
        }
        else
            {
                pickReferences.currentClueCount.text = "Clue's Picked";
            }
        
    }
    public void Checking()
    {
        Debug.Log(clue);
        if (isClue)
        {
            pickReferences.currentClue.SetActive(true);

            if (!WillClueCountStop)
            {
                clueCount = clue; // Count of numbers
            }
        }
        else
        {
            pickReferences.currentClue.SetActive(false);
        }
    }
    public void storingData()
    {
        if (clueCount <= 0)
        {
            WillClueCountStop = true;
        }
        else
        {
            clue = clueCount;
        }
        pickReferences.currentClue.SetActive(false);
    }
}
