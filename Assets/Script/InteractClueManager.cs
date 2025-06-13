using TMPro;
using UnityEngine;

public class InteractClueManager : MonoBehaviour
{
 
    [SerializeField] private string clueName;
    
    [SerializeField] private Animator animator;

   [HideInInspector] public bool isFinished = false;
    public void ClueIndication()
    {
        animator.SetTrigger("ClueEnabled");
        ClueManager.Instance.AddClue(clueName);
    }
   

}
