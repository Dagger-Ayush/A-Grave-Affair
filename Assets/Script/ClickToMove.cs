using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class ClickToMove : MonoBehaviour
{

    [SerializeField]
    private InputAction mouseClickAction;

    [SerializeField]
    private float playerSpeed = 10f;

    [SerializeField]
    private float rotationSpeed = 3f;

    Rigidbody rb;

    private Camera mainCamara;
    private Coroutine coroutine;
    private Vector3 targetPosition;
    [SerializeField] private NavMeshAgent agent;

    private bool isClicked;
    private void Awake()
    {
        mainCamara = Camera.main;

        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
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

        Ray ray = mainCamara.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray: ray, hitInfo: out RaycastHit hit) && hit.collider)
        {
            
            if (coroutine != null) StopCoroutine(coroutine);

            
                agent.SetDestination(hit.point);
            transform.LookAt(hit.point);

            //coroutine = StartCoroutine(PlayerMoveTowards(hit.point));
            // targetPosition = hit.point;
        }
        
    }
    /*
    private IEnumerator PlayerMoveTowards(Vector3 target)
    {
       isTriggered = false;
        float playerDistanceToFloor = transform.position.y - target.y;
        target.y += playerDistanceToFloor;
        while (Vector3.Distance(transform.position, target) > 0.1f  && !isTriggered)
        {
            Vector3 destination = Vector3.MoveTowards(transform.position, target, playerSpeed * Time.deltaTime);
            transform.position = destination;

            Vector3 direction = target - transform.position;
            Vector3 movement = direction.normalized * playerSpeed * Time.deltaTime;

            rb.linearVelocity = direction.normalized * playerSpeed;


            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), rotationSpeed * Time.deltaTime);
            yield return null;
        }

    }
    */
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(targetPosition, 1);

    }

    void KeyMove()
    {
        float x = Input.GetAxis("Horizontal") * playerSpeed;
        float z = Input.GetAxis("Vertical") * playerSpeed;
        if (x > 0 || z > 0)
        {  
            agent.ResetPath(); // Stop following NavMesh path
        }
        
        Vector3 dir = transform.right * x + transform.forward * z;
        dir.y = rb.linearVelocity.y;
        rb.linearVelocity = dir;
        
    }
  
   

}
