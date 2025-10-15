using UnityEngine;

public class GameStartReset : MonoBehaviour
{
    [SerializeField] private GlobalPuzzleStateData globalPuzzleState;
    void Awake()
    {
        globalPuzzleState.ResetAll();
    }
}
