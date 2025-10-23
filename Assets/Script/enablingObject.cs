using UnityEngine;

public class EnablingObject : MonoBehaviour
{
    public static EnablingObject enablingObject;
    public ObjectPickHandler pickHandler;

    private void Awake()
    {
        enablingObject = this;
    }
    private void Start()
    {
        bool dressDone = PlayerPrefs.GetInt("DressInteractionDone", 0) == 1;

        if (dressDone)
        {
            enabling();
        }
    }
    
    public void enabling()
    {

        pickHandler.gameObject.SetActive(true);
        pickHandler.enabled = true;
        pickHandler.shouldWork = true;
        FindAnyObjectByType<PuzzleProgression>()?.ActivatePuzzle8();
    }
}
