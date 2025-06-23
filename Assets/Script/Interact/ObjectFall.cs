using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Transform groundCheck;       // Empty GameObject at the base of the dustbin
    public float checkDistance = 0.2f;  // How far to check
    public LayerMask groundMask;        // Assign "Ground" layer in inspector

    private bool isGrounded;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, checkDistance, groundMask);

        Debug.DrawRay(groundCheck.position, Vector3.down * checkDistance, isGrounded ? Color.green : Color.red);

        if (isGrounded)
        {
            rb.isKinematic = true;
        }
    }
}
