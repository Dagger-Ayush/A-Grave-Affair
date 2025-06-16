using Unity.Cinemachine;
using UnityEngine;

public class ObjectPickReferences : MonoBehaviour
{
    public GameObject objectContainer;
    public GameObject inspectionBackroundimage;
    public Camera inspectionCamara;
    public CinemachineCamera MainCam, FocusCam;


    public KeyCode XrayToggle;
    public GameObject XrayCamara;
    public void SwitchCam()
    {
        if (FocusCam.Priority == 0)
        {
            MainCam.Priority = 0;
            FocusCam.Priority = 1;
            MainCam.Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
            FocusCam.Lens.ModeOverride = LensSettings.OverrideModes.Perspective;
        }
        else
        {
            MainCam.Priority = 1;
            FocusCam.Priority = 0;
            MainCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
            FocusCam.Lens.ModeOverride = LensSettings.OverrideModes.Orthographic;
        }
    }
}
