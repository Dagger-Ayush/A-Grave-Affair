using TMPro;
using UnityEngine;

public class InteractClueManager : MonoBehaviour
{
 
    [SerializeField] private string clueName;
    
    [SerializeField] private Animator animator;

   [HideInInspector] public bool isFinished = false;

    public float HoveringRange = 0.1f;
    public float HoveringMinRange = 0.08f;
    public float HoveringMaxRange = 0.1f;
    public void ClueIndication()
    {
        animator.SetTrigger("ClueEnabled");
        ClueManager.Instance.AddClue(clueName);
    }
    private void Update()
    {
        if (isFinished)
        {
            enabled = false;
        }
    }

}
