using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GettingClueCount : MonoBehaviour
{
    public static GettingClueCount instance;
    public int clue;
    public bool isClue;


    public static int clueCount;
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
                pickReferences.currentClueCount.text = "Clue count - " + clueCount.ToString();
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
            pickReferences.currentClueCount.gameObject.SetActive(true);

            if (!WillClueCountStop)
            {
                clueCount = clue; // Count of numbers
            }
        }
        else
        {
            pickReferences.currentClueCount.gameObject.SetActive(false);
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
        pickReferences.currentClueCount.gameObject.SetActive(false);
    }
}
