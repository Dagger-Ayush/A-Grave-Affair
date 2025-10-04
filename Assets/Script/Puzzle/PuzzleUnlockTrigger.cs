using UnityEngine;
using UnityEngine.UI;

public class PuzzleUnlockTrigger : MonoBehaviour
{
    [SerializeField] private Button puzzleButton;
    private bool hasUnlocked = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasUnlocked) return;

        if(other.CompareTag("Player"))
        {
            puzzleButton.gameObject.SetActive(true);
            hasUnlocked = true;
        }
    }
}
