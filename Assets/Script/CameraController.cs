using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CinemachineCamera isometricCam;
    public CinemachineCamera firstPersonCam;

    public ClickToMove playerMovement;
    public MouseLook mouseLook;

    private bool inFirstPerson = false;

    void Start()
    {
        SetViewMode(false); // Start in isometric
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            inFirstPerson = !inFirstPerson;
            SetViewMode(inFirstPerson);
        }
    }

    private void SetViewMode(bool enableFirstPerson)
    {
        if (playerMovement != null)
            playerMovement.enabled = !enableFirstPerson;

        if (mouseLook != null)
            mouseLook.enabled = enableFirstPerson;

        isometricCam.Priority = enableFirstPerson ? 0 : 10;
        firstPersonCam.Priority = enableFirstPerson ? 10 : 0;

    }
}
