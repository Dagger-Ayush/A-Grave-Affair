using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public class PointAndMovement : MonoBehaviour
{
    [SerializeField] private InputAction mouseClickAction;
    
    private Rigidbody rb;
    [SerializeField] private Animator animator;
    private Camera mainCamera;
    public NavMeshAgent agent;

    [SerializeField]
    private float playerSpeed = 10f;

    [HideInInspector] public bool isMoving;
    private Coroutine coroutine;

    private PlayerInteract PlayerInteract;
    bool isWalking;

    private enum MovementMode { None, NavMesh, Keyboard }
    private MovementMode currentMovementMode = MovementMode.None;

    private void Awake()
    {
       
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
      
        agent = GetComponent<NavMeshAgent>();

        PlayerInteract = GetComponent<PlayerInteract>();
        animator.SetTrigger("isBed");
    }

   
    void Update()
    {
        KeyMove();

        if (currentMovementMode == MovementMode.NavMesh)
        {
            if (agent.hasPath)
            {
                animator.SetBool("IsWalking", true);

                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    agent.ResetPath();
                    animator.SetBool("IsWalking", false);
                    isMoving = false;
                    currentMovementMode = MovementMode.None;
                }
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
        else if (currentMovementMode == MovementMode.Keyboard)
        {
            animator.SetBool("IsWalking", isWalking);
        }
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

            if (coroutine != null) StopCoroutine(coroutine);
            if (!isMoving)
            {
                isMoving = true;
            }

            currentMovementMode = MovementMode.NavMesh;
            agent.ResetPath();
            if (Vector3.Distance(agent.destination, hit.point) > 0.1f)
            {
                agent.SetDestination(hit.point);
            }
            transform.LookAt(hit.point);
        }

    }

    void KeyMove()
    {
        if(currentMovementMode == MovementMode.Keyboard)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                LookAt(point);
            }
        }

        float x = Input.GetAxis("Horizontal") * playerSpeed;
        float z = Input.GetAxis("Vertical") * playerSpeed;

       
        if (x != 0 || z != 0)
        {
            currentMovementMode = MovementMode.Keyboard;
            isWalking = true;

            if (!isMoving)
            {
                isMoving = true;
            }
            if (agent.hasPath)
            {
                agent.ResetPath(); // Stop following NavMesh path
            }

        }
        else
        {
            isWalking = false;

            if (currentMovementMode == MovementMode.Keyboard)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            }
        }
        

        //transform.position = new Vector3(x, 0, z);

        Vector3 dir = transform.right * x + transform.forward * z;
        dir.y = rb.linearVelocity.y;
        rb.linearVelocity = dir;
    }

  
    void LookAt(Vector3 point)
    {
        if (!isMoving) return;
        Vector3 direction = point - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
