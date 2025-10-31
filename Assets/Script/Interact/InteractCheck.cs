using UnityEngine;

public class InteractCheck : MonoBehaviour
{
    private ObjectInteract greginteract;
    public Animator animator;

    private void Start()
    {
        greginteract = GetComponent<ObjectInteract>();
        if (greginteract != null)
            greginteract.OnInteractionStarted += HandleInteractionStart;
    }

    private void HandleInteractionStart()
    {
        // Flip exactly 180° once, not accumulate
        Vector3 currentRotation = transform.eulerAngles;
        currentRotation.y = (currentRotation.y + 180f) % 360f;
        transform.eulerAngles = currentRotation;

        if (animator != null)
            animator.SetBool("Rotate", true);
    }


}
