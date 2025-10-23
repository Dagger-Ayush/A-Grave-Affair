using UnityEngine;

public class GameStartReset : MonoBehaviour
{
    [SerializeField] private GlobalPuzzleStateData globalPuzzleState;
    void Awake()
    {
        globalPuzzleState.ResetAll();

        PlayerPrefs.SetInt("EnteredOutsideDoor", 0);
        PlayerPrefs.SetInt("DressInteractionDone", 0);
        PlayerPrefs.Save();

    }
}
