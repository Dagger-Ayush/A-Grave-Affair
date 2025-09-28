using Unity.Cinemachine;
using UnityEngine;

public class MotelLobby : MonoBehaviour
{
    public CinemachineCamera isoCam;
    public Transform body;
    public Transform player;
    public float lerpSpeed = 3f;

    Transform currentTarget;   // where we finally want to go
    Transform followDummy;     // the point the camera actually follows

    public ObjectInteract motelStartDialogs;
    public ObjectInteract DialogsPhase_2;
    public ObjectInteract[] enablingInteract;
    public ObjectPickHandler enablingInspect;

    void Update()
    {
        FollowState();
        if (motelStartDialogs != null)
        {
            if(motelStartDialogs.isAutoComplete)
            {
                isoCam.Follow = player;
                DialogsPhase_2.enabled = true;
                motelStartDialogs.enabled = false;
            }
        }
        
        if (DialogsPhase_2 != null)
        {
            if(DialogsPhase_2.isAutoCompleteNearObject)
            {
                ImageFade.instance.FadeInOut();
                enablingInteract[0].enabled = true;
                enablingInteract[1].enabled = true;
                enablingInspect.enabled = true;
                enablingInteract[0].shouldWork = true;
                enablingInteract[1].shouldWork = true;
                enablingInspect.shouldWork = true;
                DialogsPhase_2.enabled = false; 
            }
        }
    }
    void FollowState()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isoCam.Follow = body;

            motelStartDialogs.enabled = true;
        }
    }
}
