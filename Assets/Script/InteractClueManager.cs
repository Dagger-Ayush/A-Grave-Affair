using System.Collections;
using TMPro;
using UnityEngine;

public class InteractClueManager : MonoBehaviour
{
 
   public static InteractClueManager instance;

    [SerializeField] private string clueName;
    
    [SerializeField] private Animator animator;

   [HideInInspector] public bool isFinished = false;
   public static bool isClueShowing = false;


    public float HoveringRange = 0.1f;
    public float HoveringMinRange = 0.08f;
    public float HoveringMaxRange = 0.1f;

    [SerializeField] private ParticleSystem repelEffect;
    private Coroutine repelCoroutine;
     private bool isRepelBusy;

    private void Awake()
    {
        instance = this;
    }
    public void ClueIndication()
    {
        StartCoroutine(Delay(2));
        animator.SetTrigger("ClueEnabled");
        ClueManager.Instance.AddClue(clueName);
        
    }
    IEnumerator Delay(float time)
    {

        isClueShowing = true;

        yield return new WaitForSeconds(time);
        isClueShowing = false;
    }

    public void StartRepelEffect()
    {
        if (!isRepelBusy)
        {
           Instantiate(repelEffect,transform.position,Quaternion.identity);
            StartCoroutine(RepleEffectDelay());
        }
       
    }

    public void StopRepelEffect()
    {
        if (repelCoroutine != null)
        {
            StopCoroutine(repelCoroutine);
            repelCoroutine = null;
        }
    }

   
    private IEnumerator RepleEffectDelay()
    {
        if (isRepelBusy) yield break; // Already running, skip

        isRepelBusy = true;

        // Wait before triggering (avoids instant start)
        yield return new WaitForSeconds(2f);

        isRepelBusy = false;
    }

}
