using UnityEngine;

public class InteractCheck : MonoBehaviour
{
    private ObjectInteract greginteract;
    public Animator animator;

    private void Start()
    {
        greginteract = GetComponent<ObjectInteract>();
    }
    void Update()
    {
        if (greginteract.isInteractionStarted)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetBackgroundAudio(AudioManager.Instance.meetingWithGreg);
            }
            if(animator != null)
            {
                animator.SetBool("Rotate", true);
            }
        }
    }
}
