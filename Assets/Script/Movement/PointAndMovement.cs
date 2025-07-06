using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class PointAndMovement : MonoBehaviour
{
    [SerializeField] private InputAction mouseClickAction;
    [SerializeField] private float playerSpeed = 10f;

    private Rigidbody rb;
    private Animator animator;
    private Camera mainCamera;
    public NavMeshAgent agent;

    [HideInInspector] public bool isMoving;
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Animation Settings")]
    [SerializeField] private string idleAnimation = "Idle";
    [SerializeField] private string walkAnimation = "Walking";
    [SerializeField] private string bedAnimation = "bed";

    [Header("Isometric Settings")]
    [SerializeField] private Vector2 isometricForward = new Vector2(1, 1); // Default isometric direction

    private Animation anim;
    private Vector3 targetPosition;
    private bool isMovingToPoint = false;
    private bool isKeyMoving = false;
    

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play(bedAnimation);
        mainCamera = Camera.main;

        // Normalize isometric direction
        isometricForward = isometricForward.normalized;
    }

    void Update()
    {
       

        // Control walking animation
        bool isWalking = Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0 || agent.hasPath;
        animator.SetBool("IsWalking", true);
    }

    private void OnEnable()
    {
        mouseClickAction.Enable();
        mouseClickAction.performed += Move;
    }

    private void OnDisable()
    {
        mouseClickAction.performed -= Move;
        mouseClickAction.Disable();
    }

    private void Move(InputAction.CallbackContext context)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {
            if (!isMoving)
            {
                isMoving = true;
            }

            agent.ResetPath();
            if (Vector3.Distance(agent.destination, hit.point) > 0.1f)
            {
                agent.SetDestination(hit.point);
            }
            transform.LookAt(hit.point);
        }
    }

    /*void KeyMove()
    {
        float x = Input.GetAxis("Horizontal") * playerSpeed;
        float z = Input.GetAxis("Vertical") * playerSpeed;
        if (x != 0 || z != 0)
        {
            if (!isMoving)
            {
                isMoving = true;
            }
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }

        Vector3 dir = transform.right * x + transform.forward * z;
        dir.y = rb.linearVelocity.y;
        rb.linearVelocity = dir;
    }*/
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

    void LookAt(Vector3 point)
    {
        Vector3 direction = point - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
