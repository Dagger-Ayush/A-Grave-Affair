using System;
using UnityEngine;

public class TutorialProgression: MonoBehaviour
{
    public PuzzleData tabletData;

    private void Update()
    {
        ProgressionHandler();
    }
    void ProgressionHandler()
    {
        if (!tabletData.isCompleted && InspectionTutorial.Instance.isInspectionComplete && InteractionTutorial.Instance.isInteractionComplete && !PlayerInteract.Instance.isPointAndMovementEnabled)
        {
                TabletTutorial.Instance.PageHandler();

            if (PlayerInteract.Instance.tabletImage != null)
            {
                PlayerInteract.Instance.tabletImage.enabled = true;
            }  
        }
        if (tabletData.isCompleted && !TabletTutorial.Instance.isTabletTutorialComplete
            && InspectionTutorial.Instance.isInspectionComplete && InteractionTutorial.Instance.isInteractionComplete)
        {
            TabletTutorial.Instance.EndTutorial();
        }
    }
}
