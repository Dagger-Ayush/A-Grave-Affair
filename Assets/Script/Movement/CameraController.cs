using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public CinemachineCamera isometricCam;
    public CinemachineCamera firstPersonCam;

    public ClickToMove playerMovement;
    public MouseLook mouseLook;

    private bool inFirstPerson = false;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        SetViewMode(false); // Start in isometric
    }

    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.V))
    //    //{
    //    //    inFirstPerson = !inFirstPerson;
    //    //    SetViewMode(inFirstPerson);
    //    //}
    //}

    public void ToggleView()
    {
        inFirstPerson = !inFirstPerson;
        SetViewMode(inFirstPerson);
    }

    private void SetViewMode(bool enableFirstPerson)
    {
        isometricCam.Priority = enableFirstPerson ? 0 : 10;
        firstPersonCam.Priority = enableFirstPerson ? 10 : 0;

        if (playerMovement != null)
            playerMovement.enabled = !enableFirstPerson;

        //if (mouseLook != null)
        //    mouseLook.enabled = enableFirstPerson;

    }
}
