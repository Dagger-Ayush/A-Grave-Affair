using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PointAndMovement : MonoBehaviour
{

    [SerializeField]
    private InputAction mouseClickAction;

    [SerializeField]
    private float playerSpeed = 10f;
    
    //private float rotationSpeed = 1f;

    Rigidbody rb;

    private Camera mainCamara;
    private Coroutine coroutine;

    public NavMeshAgent agent;
    

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

            agent.ResetPath();
            if (Vector3.Distance(agent.destination,hit.point) > 0.1f)
            {
                agent.SetDestination(hit.point);
            }
            transform.LookAt(hit.point);
        }

    }
 
    void KeyMove()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up,Vector3.zero);
        float rayDistance;
        if (groundPlane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            LookAt(point);
        }
        
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

        

        /*
         if (Input.GetKey(KeyCode.W) )
         {
             if (!isMoving)
             {
                 isMoving = true;
             }
             if (agent.hasPath)
             {
                 agent.ResetPath(); // Stop following NavMesh path
             }
             rb.AddForce(transform.forward*playerSpeed);

         }
         */
    }

    private void LookAt(Vector3 point)
    {
        Vector3 hightCorrectedPoint = new Vector3(point.x,transform.position.y, point.z);
        transform.LookAt(hightCorrectedPoint);
    }
}
