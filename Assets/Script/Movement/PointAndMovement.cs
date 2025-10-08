using UnityEngine;
using UnityEngine.InputSystem;

public class PointAndMovement : MonoBehaviour
{
    public static PointAndMovement instance;

    [SerializeField] private Animator animator;
    private Camera mainCamera;
    private Rigidbody rb;

    [SerializeField] private float playerSpeed = 10f;

    [HideInInspector] public bool isMoving;
    private PlayerInteract playerInteract;
    private bool isWalking;

    public bool StopBedAniWork = false;

    private void Awake()
    {
        if (instance == null) instance = this;

        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        playerInteract = GetComponent<PlayerInteract>();

        if (!StopBedAniWork)
        {
            animator.SetTrigger("isBed");
        }
    }

    private void Update()
    {
        KeyMove();
        animator.SetBool("IsWalking", isWalking);
    }

    void KeyMove()
    {
        float z = Input.GetAxis("Vertical") * playerSpeed; // W/S input

        if (z > 0) // Only move forward (W)
        {
            MovementRotation(z);
            isWalking = true;

            if (!isMoving)
                isMoving = true;
        }
        else
        {
            isWalking = false;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void MovementRotation(float z)
    {
        // Rotate towards mouse position (isometric look)
        Plane playerPlane = new Plane(Vector3.up, transform.position);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

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

        // Move forward along facing direction
        Vector3 moveDir = transform.forward * z;
        moveDir.y = rb.linearVelocity.y; // preserve gravity
        rb.linearVelocity = moveDir;
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
}
