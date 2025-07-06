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
    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
      
        agent = GetComponent<NavMeshAgent>();

        PlayerInteract = GetComponent<PlayerInteract>();
    }
   

    void Update()
    {
       
        animator.SetBool("IsIdle", true);
        //animator.SetBool("IsIdle", true);
        KeyMove();
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            LookAt(point);
        }

        float x = Input.GetAxis("Horizontal") * playerSpeed;
        float z = Input.GetAxis("Vertical") * playerSpeed;

        bool isWalking;
        if (x != 0 || z != 0)
        {
            isWalking = true;
            animator.SetBool("IsWalking", true);
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
        }
         if (!isWalking || agent.hasPath || PlayerInteract.isPointAndMovementEnabled) 
        {

            animator.SetBool("IsWalking", false);
        }
        //transform.position = new Vector3(x, 0, z);

        Vector3 dir = transform.right * x + transform.forward * z;
        dir.y = rb.linearVelocity.y;
        rb.linearVelocity = dir;



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
