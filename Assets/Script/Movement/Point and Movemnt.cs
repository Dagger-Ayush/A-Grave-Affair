using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PointAndMovement : MonoBehaviour
{

    [SerializeField]
    private InputAction mouseClickAction;

    [SerializeField]
    private float playerSpeed = 10f;


    Rigidbody rb;

    private Camera mainCamara;
    private Coroutine coroutine;

    public NavMeshAgent agent;
    public Transform player;

    [HideInInspector] public bool isMoving;
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
            if (!isMoving)
            {
                isMoving = true;
            }
            agent.SetDestination(hit.point);
            transform.LookAt(hit.point);
        }

    }
 
    void KeyMove()
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
                agent.ResetPath(); // Stop following NavMesh path
            }
           
        }
        //transform.position = new Vector3(x, 0, z);

        Vector3 dir = transform.right * x + transform.forward * z;
        dir.y = rb.linearVelocity.y;
        rb.linearVelocity = dir;

    }



}
