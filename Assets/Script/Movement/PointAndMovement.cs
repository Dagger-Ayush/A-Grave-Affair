using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
public class PointAndMovement : MonoBehaviour
{
    public static PointAndMovement instance;
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

    public bool StopBedAniWork = false;
    private void Awake()
    {
        if (instance == null) instance = this;

        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
      
        agent = GetComponent<NavMeshAgent>();

        PlayerInteract = GetComponent<PlayerInteract>();
        if (!StopBedAniWork)
        {
            animator.SetTrigger("isBed");
        }
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
       // mouseClickAction.Enable();
       // mouseClickAction.performed += Move;
    }

    private void OnDisable()
    {
       // mouseClickAction.performed -= Move;
       // mouseClickAction.Disable();
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
            Vector3 lookTarget = hit.point;
            lookTarget.y = transform.position.y; // keep same Y so no tilt
            transform.LookAt(lookTarget);

        }

    }
    /*
    void KeyMove()
    {
        float x = Input.GetAxis("Horizontal") * playerSpeed;
        float z = Input.GetAxis("Vertical") * playerSpeed;


        if (currentMovementMode == MovementMode.Keyboard)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;
            if (groundPlane.Raycast(ray, out rayDistance))
            {
                Vector3 point = ray.GetPoint(rayDistance);
                if (x != 0 || z != 0)
                {
                    LookAt(point);
                }
            }
        }

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
    */
    void KeyMove()
    {
        float z = Input.GetAxis("Vertical") * playerSpeed; // Only W or S, but we restrict it below

        if (z > 0) // Only allow forward (W)
        {
            MovementRotation(z);
            currentMovementMode = MovementMode.Keyboard;
            isWalking = true;

            if (!isMoving)
            {
                isMoving = true;
            }

            if (agent.hasPath)
            {
                agent.ResetPath(); // Stop NavMesh path if moving via keyboard
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
    void MovementRotation(float z)
    {
        // 1️⃣ Mouse look (isometric)
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (playerPlane.Raycast(ray, out float distance))
        {
            Vector3 lookPoint = ray.GetPoint(distance);

            Vector3 direction = lookPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 10f);
            }
        }

        // 2️⃣ Move forward only along XZ
        Vector3 moveDir = transform.forward * z;
        moveDir.y = rb.linearVelocity.y; // preserve gravity-driven vertical motion
        rb.linearVelocity = moveDir;
    }

}
