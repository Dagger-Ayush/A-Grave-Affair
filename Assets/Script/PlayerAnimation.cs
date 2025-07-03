using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animation))]
public class PlayerAnimation : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation Settings")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walking";
    [SerializeField] private InputAction mouseClickAction;

    [Header("Isometric Settings")]
    [SerializeField] private Vector2 isometricForward = new Vector2(1, 1); // Default isometric direction

    private Animation anim;
    private Vector3 targetPosition;
    private bool isMovingToPoint = false;
    private bool isKeyMoving = false;
    private Camera mainCamera;

    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play(idleAnimation);
        mainCamera = Camera.main;

        mouseClickAction.Enable();
        mouseClickAction.performed += OnMouseClick;

        // Normalize isometric direction
        isometricForward = isometricForward.normalized;
    }

    void OnDestroy()
    {
        mouseClickAction.performed -= OnMouseClick;
        mouseClickAction.Disable();
    }

    void Update()
    {
        HandleKeyboardInput();
        HandlePointAndClickMovement();
        UpdateAnimations();
    }

    private void OnMouseClick(InputAction.CallbackContext context)
    {
        if (isKeyMoving) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            targetPosition = hit.point;
            isMovingToPoint = true;
            Debug.Log($"Moving to: {targetPosition}");
        }
    }

    private void HandleKeyboardInput()
    {
        Vector3 direction = Vector3.zero;
        isKeyMoving = false;

        // Convert isometric input to world space
        if (Input.GetKey(KeyCode.W))
        {
            direction += new Vector3(isometricForward.x, 0, isometricForward.y);
            isKeyMoving = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction -= new Vector3(isometricForward.x, 0, isometricForward.y);
            isKeyMoving = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += new Vector3(-isometricForward.y, 0, isometricForward.x);
            isKeyMoving = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += new Vector3(isometricForward.y, 0, -isometricForward.x);
            isKeyMoving = true;
        }

        if (direction != Vector3.zero)
        {
            // Normalize and apply movement
            direction.Normalize();
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Rotate to face movement direction
            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        if (isKeyMoving && isMovingToPoint)
        {
            isMovingToPoint = false;
        }
    }

    private void HandlePointAndClickMovement()
    {
        if (!isMovingToPoint || isKeyMoving) return;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;

        if (direction.magnitude <= stoppingDistance)
        {
            isMovingToPoint = false;
            return;
        }

        // Convert to isometric movement direction
        Vector3 isoDirection = new Vector3(
            direction.x * isometricForward.x + direction.z * isometricForward.y,
            0,
            direction.x * isometricForward.y + direction.z * isometricForward.x
        );

        Quaternion targetRotation = Quaternion.LookRotation(isoDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Move in the forward direction (already isometric)
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void UpdateAnimations()
    {
        bool shouldWalk = isKeyMoving || isMovingToPoint;

        if (shouldWalk && !anim.IsPlaying(walkAnimation))
        {
            anim.Play(walkAnimation);
        }
        else if (!shouldWalk && !anim.IsPlaying(idleAnimation))
        {
            anim.Play(idleAnimation);
        }
    }

    private void OnDrawGizmos()
    {
        if (isMovingToPoint)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetPosition, stoppingDistance);
            Gizmos.DrawLine(transform.position, targetPosition);
        }
    }
}
