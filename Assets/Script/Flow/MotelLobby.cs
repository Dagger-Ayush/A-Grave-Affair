using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class MotelLobby : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineCamera isoCam;
    public Transform body;        // Dummy that camera follows
    public Transform player;      // Start point of the camera
    public float lerpSpeed = 2f;  // Speed of the initial pan

    [Header("Gameplay Phase_1")]
    public ObjectInteract motelStartDialogs;
    public ObjectInteract DialogsPhase_2;
    public ObjectInteract enablingInteract;
    public ObjectPickHandler[] enablingInspect;
    public ObjectInteract enablingInteractafterPhase_1;
    public ObjectInteract enablingInteractafterPhase_2;
    bool isObjectEnabled_Phase_1 = false;

    [Header("Gameplay Phase_1")]
    public PuzzleProgression puzzleProgression;
    public ObjectInteract motelGatherDialogs;

    bool isObjectEnabled_Phase_2 = false;
    // --- private fields ---
    Vector3 targetPos;
    Quaternion targetRot;
    bool introPanDone = false;

    [Header(" Final Page ")]
    bool isFinalDialogStarted = false;
    bool isFinalDialogComplete = false;
    
    public ObjectPickHandler orderBookPickHandler;
    public ObjectInteract[] LastInteractionObjects;

    [Header(" ONsentenceComplete ")]
    public ObjectInteract ONsentenceCompleteTrigger;
    public ObjectInteract ONsentenceCompleteInteract;
    public bool isSentenceDialogEnded;

    [Header("Doors to activate")]
    public GameObject doorToNancyRoom;
    public GameObject doorToOutsideMotel;

    private void OnDisable()
    {
        isoCam.Follow = player;
    }
    void Start()
    {
            // Save the real body position & rotation (final camera spot)
            targetPos = body.position;
            targetRot = body.rotation;

            // Start the dummy at the player’s location so camera starts there
            body.position = player.position;
            body.rotation = player.rotation;

            // Camera follows the dummy
            isoCam.Follow = body;

        // Check if phase 3 is completed
        bool phase3Complete = PlayerPrefs.GetInt("MotelLobby_Phase3Complete", 0) == 1;

        if(phase3Complete)
        {
            introPanDone = true;
            isFinalDialogComplete = true;
            isoCam.Follow = player;

            orderBookPickHandler.enabled = true;
            orderBookPickHandler.shouldWork = true;

            foreach (ObjectInteract obtInt in LastInteractionObjects)
            {
                obtInt.enabled = true;
                obtInt.shouldWork = true;
            }

            if(doorToNancyRoom != null) doorToNancyRoom.SetActive(true);
            //if(doorToOutsideMotel != null) doorToOutsideMotel.SetActive(true);

            PosandAnimationUpdate.Instance.UpdatePhase_3();

            puzzleProgression.puzzle6.SetActive(true);
            puzzleProgression.puzzle7.SetActive(true);
        }
        else
        {
            motelStartDialogs.enabled = true;

            if (doorToNancyRoom != null) doorToNancyRoom.SetActive(false);
            if (doorToOutsideMotel != null) doorToOutsideMotel.SetActive(false);
        }

            //motelStartDialogs.enabled = true;
    }

    void Update()
    {
        /*
        if (ONsentenceCompleteTrigger.isAutoComplete && isFinalDialogComplete && !isSentenceDialogEnded)
        {
            ObjectsEnablePhase_4();
        }
        */

        if (!isFinalDialogComplete)
        {
           
            if (motelGatherDialogs.isSecondDialog)
            {
                isFinalDialogStarted = true;
            }
            if (motelGatherDialogs.isAutoComplete)
            {
                ImageFade.instance.FadeInOut();

                StartCoroutine(ObjectsEnablePhase_3());
            }
            if (!isFinalDialogStarted)
            {
                
                if (!introPanDone)
                {
                    // Smoothly move dummy from player to body
                    body.position = Vector3.Lerp(
                        body.position,
                        targetPos,
                        Time.deltaTime * lerpSpeed
                    );

                    body.rotation = Quaternion.Lerp(
                        body.rotation,
                        targetRot,
                        Time.deltaTime * lerpSpeed
                    );

                    if (Vector3.Distance(body.position, targetPos) < 0.05f)
                    {
                        body.position = targetPos;
                        body.rotation = targetRot;
                        introPanDone = true;
                    }
                }
                if (introPanDone && motelStartDialogs && motelStartDialogs.isAutoComplete)
                {
                    // After phase 1, smoothly move camera dummy to follow player
                    body.position = Vector3.Lerp(body.position, player.position, Time.deltaTime * lerpSpeed);
                    body.rotation = Quaternion.Lerp(body.rotation, player.rotation, Time.deltaTime * lerpSpeed);

                    DialogsPhase_2.enabled = true;
                    motelStartDialogs.enabled = false;
                }


                if (puzzleProgression.isDialogEnabled && !isObjectEnabled_Phase_2)
                {
                    ImageFade.instance.FadeInOut();

                    StartCoroutine(ObjectsEnablePhase_2());
                }

                if (!isObjectEnabled_Phase_1)
                {
                    if (DialogsPhase_2 && DialogsPhase_2.isAutoCompleteNearObject)
                    {
                        ImageFade.instance.FadeInOut();
                        StartCoroutine(ObjectsEnablePhase_1());
                    }
                }
            }
            else
            {
                isoCam.Follow = player;
            }
        }
    }
    IEnumerator ObjectsEnablePhase_1()
    {
        yield return new WaitForSeconds(1.5f);
        PosandAnimationUpdate.Instance.UpdatePhase_1();
        enablingInteract.enabled = true;
        enablingInteract.shouldWork = true;

        enablingInspect[0].enabled = true;
        enablingInspect[0].shouldWork = true;
        enablingInspect[1].enabled = true;
        enablingInspect[1].shouldWork = true;

        DialogsPhase_2.enabled = false;
        isObjectEnabled_Phase_1 = true;
    }
    IEnumerator ObjectsEnablePhase_2()
    {
        yield return new WaitForSeconds(1.5f);
        PosandAnimationUpdate.Instance.UpdatePhase_2();
        motelGatherDialogs.enabled = true;
        motelGatherDialogs.shouldWork = true;

        isObjectEnabled_Phase_2 = false;

        enablingInteract.enabled = false;
        DialogsPhase_2.enabled = false;
        enablingInteractafterPhase_1.enabled = false;
        enablingInteractafterPhase_2.enabled = false;

        enablingInspect[0].enabled = false;
        enablingInspect[1].enabled = false;

       

    }
    IEnumerator ObjectsEnablePhase_3()
    {
        
        yield return new WaitForSeconds(1.5f);


        orderBookPickHandler.enabled = true;
        orderBookPickHandler.shouldWork = true;
        
        foreach(ObjectInteract objInt in LastInteractionObjects)
        {
            objInt.enabled = true;
            objInt.shouldWork = true;
        }
        PosandAnimationUpdate.Instance.UpdatePhase_3();

        motelGatherDialogs.enabled = false;
       
        isFinalDialogComplete = true;

        // Save Phase completion
        PlayerPrefs.SetInt("MotelLobby_Phase3Complete", 1);
        PlayerPrefs.Save();

        if(doorToNancyRoom != null) doorToNancyRoom.SetActive(true);
        //if(doorToOutsideMotel != null) doorToOutsideMotel.SetActive(true);
    }
    IEnumerator ObjectsEnablePhase_4()
    {

        yield return new WaitForSeconds(1.5f);

        ONsentenceCompleteInteract.enabled = true;
        ONsentenceCompleteInteract.shouldWork = true;

        PosandAnimationUpdate.Instance.UpdatePhase_2();

        isSentenceDialogEnded = true;

    }
}
