using System;
using UnityEngine;

public class TutorialProgression: MonoBehaviour
{
    private void Update()
    {
        ProgressionHandler();
    }
    void ProgressionHandler()
    {
        if (InspectionTutorial.Instance.isInspectionComplete && InteractionTutorial.Instance.isInteractionComplete && !PlayerInteract.Instance.isPointAndMovementEnabled)
        {
                TabletTutorial.Instance.PageHandler();

            if (PlayerInteract.Instance.tabletImage != null)
            {
                PlayerInteract.Instance.tabletImage.enabled = true;
            }  
        }
    }
}
