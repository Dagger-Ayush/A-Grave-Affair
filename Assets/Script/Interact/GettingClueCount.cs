using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GettingClueCount : MonoBehaviour
{
    public static GettingClueCount instance;
    private int clue;
    public bool isClue;
    public bool canTik;


    public static int clueCount;
    public int totalClues;
    [HideInInspector] public bool WillClueCountStop = false;

    public ObjectPickReferences pickReferences;

    public GameObject[] pickPrefab;

    private void Awake()
    {
        instance = this;
    }
 
    private void Update()
    {
        if (canTik)
        {
            if (clueCount > 0)
            {
                if (pickPrefab != null)
                {
                    pickPrefab[clueCount - 1].SetActive(true);
                }
            }
        }
        
        /*
        if (clueCount < totalClues)
            {
            pickReferences.currentClueCount.text = "Clues Found (" + clueCount.ToString() + "/" + totalClues.ToString() + ")";
        }
        else
            {
                pickReferences.currentClueCount.text = "Clue's Picked";
            }
        */
        
    }
    public void Checking()
    {
        if (isClue)
        {
            if (!WillClueCountStop)
            {
                clueCount = clue; // Count of numbers
            }
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
       
    }
}
